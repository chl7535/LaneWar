using UnityEngine;
using LaneWar.Core;

namespace LaneWar.Systems
{
    // GridManager가 계산한 단일 기준(LaneLayout)을 그대로 사용해 통로(어두운 색)와 배치 가능 영역(밝은 색)을
    // 항상(배치 모드가 아닐 때도) 바닥에 표시하고, 경계선을 뚜렷하게 그려 둘을 구분한다.
    // 배치 그리드(GridView)와 반드시 같은 LaneLayout을 참조하므로 서로 어긋날 수 없다
    public class PathFloorView : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private Color laneColor = new Color(0.16f, 0.14f, 0.12f, 1f);
        [SerializeField] private Color placementColor = new Color(0.55f, 0.6f, 0.68f, 1f);
        [SerializeField] private Color borderColor = new Color(1f, 0.85f, 0.2f, 1f);
        [SerializeField] private float borderThickness = 0.08f;
        [SerializeField] private float heightOffset = 0.01f;

        private static Mesh _sharedQuadMesh;

        private void Start()
        {
            // 인스펙터 연결이 비어 있으면 씬에서 자동으로 찾아 복구를 시도한다 (연결을 놓친 경우의 방어)
            if (gridManager == null)
            {
                gridManager = FindFirstObjectByType<GridManager>();
            }

            if (gridManager == null)
            {
                Debug.LogError(
                    $"[PathFloorView] '{name}' 오브젝트의 GridManager 참조가 비어 있고, 씬에서 GridManager를 " +
                    "찾지도 못했습니다. 씬에 GridManager 오브젝트가 있는지, 있다면 PathFloorView의 " +
                    "Grid Manager 슬롯에 드래그해 연결했는지 확인하세요.",
                    this);
                return;
            }

            LaneLayout layout = gridManager.LaneLayout;
            if (layout == null)
            {
                Debug.LogError(
                    $"[PathFloorView] '{gridManager.name}'의 LaneLayout이 아직 계산되지 않았습니다. " +
                    "GridManager 컴포넌트의 Path Controller / Flow Config 참조가 비어 있지 않은지 확인하세요.",
                    gridManager);
                return;
            }

            BuildLaneFrame(layout);
            BuildPlacementFloor(layout);
            BuildBorder(layout);
        }

        // 통로 영역 = LaneOuterBounds(중심선에서 바깥쪽으로 LaneHalfWidth 확장한 경계)와
        // PlacementBounds(중심선에서 안쪽으로 LaneHalfWidth 줄인 경계) 사이의 띠.
        // 중심선 기준 양방향으로 각각 LaneHalfWidth씩 걸쳐 있으므로 띠의 폭은 항상 LaneHalfWidth*2와 같다 —
        // 이래야 EnemyFlowAgent가 실제로 좌우로 벗어날 수 있는 전체 범위를 빈틈없이 덮는다
        private void BuildLaneFrame(LaneLayout layout)
        {
            Bounds outer = layout.LaneOuterBounds;
            Bounds placement = layout.PlacementBounds;

            float topHeight = outer.max.z - placement.max.z;
            float bottomHeight = placement.min.z - outer.min.z;
            float leftWidth = placement.min.x - outer.min.x;
            float rightWidth = outer.max.x - placement.max.x;

            CreateQuad(
                "LaneStrip_Top",
                new Vector3(outer.center.x, heightOffset, placement.max.z + topHeight * 0.5f),
                new Vector2(outer.size.x, topHeight),
                laneColor,
                0);
            CreateQuad(
                "LaneStrip_Bottom",
                new Vector3(outer.center.x, heightOffset, placement.min.z - bottomHeight * 0.5f),
                new Vector2(outer.size.x, bottomHeight),
                laneColor,
                0);
            CreateQuad(
                "LaneStrip_Left",
                new Vector3(placement.min.x - leftWidth * 0.5f, heightOffset, placement.center.z),
                new Vector2(leftWidth, placement.size.z),
                laneColor,
                0);
            CreateQuad(
                "LaneStrip_Right",
                new Vector3(placement.max.x + rightWidth * 0.5f, heightOffset, placement.center.z),
                new Vector2(rightWidth, placement.size.z),
                laneColor,
                0);
        }

        // 배치 가능 영역 = PlacementBounds 전체를 밝은 색으로 정확히 덮는다 (GridSystem이 칸을 나누는 영역과 동일)
        private void BuildPlacementFloor(LaneLayout layout)
        {
            Bounds placement = layout.PlacementBounds;
            CreateQuad(
                "PlacementFloor",
                new Vector3(placement.center.x, heightOffset, placement.center.z),
                new Vector2(placement.size.x, placement.size.z),
                placementColor,
                1);
        }

        // 통로/배치 영역 경계를 따라 뚜렷한 색의 얇은 테두리를 그린다
        private void BuildBorder(LaneLayout layout)
        {
            Bounds placement = layout.PlacementBounds;
            float y = heightOffset + 0.005f;

            CreateQuad(
                "Border_Top",
                new Vector3(placement.center.x, y, placement.max.z),
                new Vector2(placement.size.x + borderThickness, borderThickness),
                borderColor,
                2);
            CreateQuad(
                "Border_Bottom",
                new Vector3(placement.center.x, y, placement.min.z),
                new Vector2(placement.size.x + borderThickness, borderThickness),
                borderColor,
                2);
            CreateQuad(
                "Border_Left",
                new Vector3(placement.min.x, y, placement.center.z),
                new Vector2(borderThickness, placement.size.z + borderThickness),
                borderColor,
                2);
            CreateQuad(
                "Border_Right",
                new Vector3(placement.max.x, y, placement.center.z),
                new Vector2(borderThickness, placement.size.z + borderThickness),
                borderColor,
                2);
        }

        private void CreateQuad(string name, Vector3 center, Vector2 size, Color color, int sortingOrder)
        {
            var quadObject = new GameObject(name);
            quadObject.transform.SetParent(transform, false);
            quadObject.transform.position = center;
            quadObject.transform.localScale = new Vector3(size.x, 1f, size.y);

            MeshFilter filter = quadObject.AddComponent<MeshFilter>();
            filter.sharedMesh = GetSharedQuadMesh();

            MeshRenderer renderer = quadObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = new Material(Shader.Find("Sprites/Default")) { color = color };
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingOrder = sortingOrder;
        }

        private static Mesh GetSharedQuadMesh()
        {
            if (_sharedQuadMesh != null)
            {
                return _sharedQuadMesh;
            }

            _sharedQuadMesh = new Mesh { name = "LaneStripQuad" };
            _sharedQuadMesh.vertices = new[]
            {
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(-0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, -0.5f)
            };
            _sharedQuadMesh.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f)
            };
            _sharedQuadMesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            _sharedQuadMesh.RecalculateBounds();
            return _sharedQuadMesh;
        }
    }
}

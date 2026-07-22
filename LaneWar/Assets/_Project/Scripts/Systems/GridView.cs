using UnityEngine;
using LaneWar.Core;

namespace LaneWar.Systems
{
    // 배치 모드(대기열에서 유닛 선택 시)에만 배치 그리드를 반투명하게 펄스 표시하고,
    // 마우스가 올라간 칸과 배치 불가 칸(점유/통로)을 색으로 구분해 보여주는 뷰.
    // 칸 상태 판정은 GridManager.GridSystem에, 마우스 입력/호버 판정은 UnitPlacementController에 위임한다
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private UnitPlacementController placementController;

        [Header("셀 시각 스타일")]
        [SerializeField] private float cellFillRatio = 0.9f;
        [SerializeField] private float heightOffset = 0.02f;
        [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.18f);
        [SerializeField] private Color blockedColor = new Color(0.9f, 0.15f, 0.15f, 0.35f);
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.4f, 0.55f);
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseMinAlpha = 0.35f;
        [SerializeField] private float pulseMaxAlpha = 1f;

        private Transform _cellContainer;
        private Material[,] _cellMaterials;
        private static Mesh _sharedQuadMesh;

        private void Start()
        {
            if (gridManager == null)
            {
                gridManager = FindFirstObjectByType<GridManager>();
            }

            if (gridManager == null || gridManager.GridSystem == null)
            {
                Debug.LogError(
                    $"[GridView] '{name}'이 사용할 GridManager(또는 그 GridSystem)를 찾지 못했습니다. " +
                    "GridView의 Grid Manager 슬롯이 비어있지 않은지, GridManager 쪽 참조들이 올바른지 확인하세요.",
                    this);
                enabled = false;
                return;
            }

            BuildCells();
            _cellContainer.gameObject.SetActive(false);
        }

        private void Update()
        {
            bool active = placementController != null && placementController.IsPlacingUnit;
            _cellContainer.gameObject.SetActive(active);

            if (!active)
            {
                return;
            }

            RefreshCellColors(placementController.HoveredCell);
        }

        private void BuildCells()
        {
            GridSystem grid = gridManager.GridSystem;
            _cellContainer = new GameObject("GridCells").transform;
            _cellContainer.SetParent(transform, false);

            _cellMaterials = new Material[grid.Rows, grid.Columns];

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    var coord = new GridCoord(row, column);
                    var cell = new GameObject($"Cell_{row}_{column}");
                    cell.transform.SetParent(_cellContainer, false);
                    cell.transform.position = grid.GetCellCenter(coord) + Vector3.up * heightOffset;
                    cell.transform.localScale = new Vector3(grid.CellWidth * cellFillRatio, 1f, grid.CellHeight * cellFillRatio);

                    MeshFilter filter = cell.AddComponent<MeshFilter>();
                    filter.sharedMesh = GetSharedQuadMesh();

                    var material = new Material(Shader.Find("Sprites/Default"));
                    MeshRenderer renderer = cell.AddComponent<MeshRenderer>();
                    renderer.sharedMaterial = material;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                    renderer.sortingOrder = 3;

                    _cellMaterials[row, column] = material;
                }
            }
        }

        private void RefreshCellColors(GridCoord? hovered)
        {
            GridSystem grid = gridManager.GridSystem;
            float pulse = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    var coord = new GridCoord(row, column);
                    bool isHovered = hovered.HasValue && hovered.Value == coord;
                    bool placeable = grid.IsPlaceable(coord);

                    Color color;
                    if (isHovered && placeable)
                    {
                        color = hoverColor;
                    }
                    else if (!placeable)
                    {
                        color = blockedColor;
                    }
                    else
                    {
                        color = emptyColor;
                        color.a *= pulse;
                    }

                    _cellMaterials[row, column].color = color;
                }
            }
        }

        private static Mesh GetSharedQuadMesh()
        {
            if (_sharedQuadMesh != null)
            {
                return _sharedQuadMesh;
            }

            _sharedQuadMesh = new Mesh { name = "GridCellQuad" };
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

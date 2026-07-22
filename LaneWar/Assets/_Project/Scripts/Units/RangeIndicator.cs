using UnityEngine;

namespace LaneWar.Units
{
    // 유닛의 사거리를 맵 바닥에 반투명 원(디스크)으로 표시하는 런타임 뷰(에디터 Gizmo 아님).
    // 유닛 선택 시 표시와 배치 미리보기 표시 양쪽에서 재사용된다
    public class RangeIndicator : MonoBehaviour
    {
        [SerializeField] private int segments = 48;
        [SerializeField] private Color fillColor = new Color(1f, 1f, 1f, 0.2f);
        [SerializeField] private float heightOffset = 0.03f;

        private static Mesh _sharedUnitDiscMesh;

        private Transform _visual;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            var visualObject = new GameObject("RangeIndicatorVisual");
            _visual = visualObject.transform;
            _visual.SetParent(transform, false);
            _visual.localPosition = new Vector3(0f, heightOffset, 0f);

            MeshFilter filter = visualObject.AddComponent<MeshFilter>();
            filter.sharedMesh = GetSharedUnitDiscMesh(segments);

            _meshRenderer = visualObject.AddComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default")) { color = fillColor };
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.sortingOrder = 4;
            _meshRenderer.enabled = false;
        }

        // 사거리(반지름)와 정확히 일치하도록 디스크 크기를 조절한다 (메시는 반지름 0.5로 제작되어 2배 스케일이 필요하다)
        public void SetRadius(float radius)
        {
            _visual.localScale = new Vector3(radius * 2f, 1f, radius * 2f);
        }

        public void SetVisible(bool visible)
        {
            _meshRenderer.enabled = visible;
        }

        private static Mesh GetSharedUnitDiscMesh(int segmentCount)
        {
            if (_sharedUnitDiscMesh != null)
            {
                return _sharedUnitDiscMesh;
            }

            var vertices = new Vector3[segmentCount + 1];
            var uvs = new Vector2[segmentCount + 1];
            var triangles = new int[segmentCount * 3];

            vertices[0] = Vector3.zero;
            uvs[0] = new Vector2(0.5f, 0.5f);

            for (int i = 0; i < segmentCount; i++)
            {
                float angle = i * Mathf.PI * 2f / segmentCount;
                float x = Mathf.Cos(angle) * 0.5f;
                float z = Mathf.Sin(angle) * 0.5f;
                vertices[i + 1] = new Vector3(x, 0f, z);
                uvs[i + 1] = new Vector2(x + 0.5f, z + 0.5f);
            }

            for (int i = 0; i < segmentCount; i++)
            {
                int current = i + 1;
                int next = i + 2 > segmentCount ? 1 : i + 2;
                int triIndex = i * 3;
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = current;
                triangles[triIndex + 2] = next;
            }

            _sharedUnitDiscMesh = new Mesh { name = "UnitRangeDisc" };
            _sharedUnitDiscMesh.vertices = vertices;
            _sharedUnitDiscMesh.uv = uvs;
            _sharedUnitDiscMesh.triangles = triangles;
            _sharedUnitDiscMesh.RecalculateBounds();
            return _sharedUnitDiscMesh;
        }
    }
}

using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;

namespace LaneWar.Systems
{
    // PathSystem+EnemyFlowConfig로부터 통로/배치 가능 영역(LaneLayout)을 계산해 GridSystem을 만들고,
    // 이 LaneLayout 하나만을 GridView/PathFloorView 등 씬의 다른 컴포넌트에 공유하는 매니저.
    // 세 시스템(경로, 바닥 시각 표시, 배치 그리드)이 서로 다른 기준으로 어긋나지 않도록 하는 단일 진입점이다
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridConfig gridConfig;
        [SerializeField] private PathController pathController;
        [SerializeField] private EnemyFlowConfig flowConfig;

        [Header("에디터 확인용 Gizmo (Play 없이도 표시됨)")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color outerBoundsGizmoColor = new Color(0.9f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color placementBoundsGizmoColor = new Color(0.2f, 0.9f, 0.3f, 1f);

        public LaneLayout LaneLayout { get; private set; }
        public GridSystem GridSystem { get; private set; }

        private void Awake()
        {
            if (!ValidateReferences())
            {
                return;
            }

            LaneLayout = new LaneLayout(pathController.PathSystem, flowConfig.LaneHalfWidth);
            GridSystem = new GridSystem(gridConfig, LaneLayout);
        }

        // 참조가 비어 있으면 어떤 슬롯이 비었는지 이름을 찍고 빌드를 건너뛴다 (예외로 죽는 대신 원인을 바로 알 수 있게)
        private bool ValidateReferences()
        {
            if (gridConfig == null)
            {
                Debug.LogError($"[GridManager] '{name}'의 Grid Config 참조가 비어 있습니다. GridConfig.asset을 연결하세요.", this);
                return false;
            }

            if (pathController == null)
            {
                Debug.LogError($"[GridManager] '{name}'의 Path Controller 참조가 비어 있습니다. PathWaypoints 오브젝트를 연결하세요.", this);
                return false;
            }

            if (flowConfig == null)
            {
                Debug.LogError($"[GridManager] '{name}'의 Flow Config 참조가 비어 있습니다. EnemyFlowConfig 에셋을 연결하세요.", this);
                return false;
            }

            return true;
        }

        // Play 모드가 아니어도(에디터에서 씬만 열어도) 통로 안쪽/바깥쪽 경계를 확인할 수 있도록
        // 캐시된 GridSystem에 의존하지 않고 그때그때 다시 계산한다.
        // 안쪽 경계(초록) = PlacementBounds, 바깥쪽 경계(빨강) = LaneOuterBounds(적이 실제로 벗어날 수 있는 최대 범위)
        private void OnDrawGizmos()
        {
            if (!drawGizmos || pathController == null || flowConfig == null)
            {
                return;
            }

            var layout = new LaneLayout(pathController.PathSystem, flowConfig.LaneHalfWidth);
            float y = gridConfig != null ? gridConfig.FloorHeight : 0f;

            Gizmos.color = outerBoundsGizmoColor;
            DrawFlatBoundsGizmo(layout.LaneOuterBounds, y);

            Gizmos.color = placementBoundsGizmoColor;
            DrawFlatBoundsGizmo(layout.PlacementBounds, y);
        }

        private static void DrawFlatBoundsGizmo(Bounds bounds, float y)
        {
            Vector3 center = new Vector3(bounds.center.x, y, bounds.center.z);
            Vector3 size = new Vector3(bounds.size.x, 0.01f, bounds.size.z);
            Gizmos.DrawWireCube(center, size);
        }
    }
}

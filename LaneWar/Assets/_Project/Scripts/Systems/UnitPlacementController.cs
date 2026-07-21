using UnityEngine;
using UnityEngine.InputSystem;
using LaneWar.Core;
using LaneWar.Data;
using LaneWar.Units;

namespace LaneWar.Systems
{
    // 임시 배치 UI: 대기열(UnitInventoryManager)에서 유닛을 골라 맵 클릭으로 배치/선택/재배치한다.
    // 통로(적 경로) 위에는 배치할 수 없다. 정식 UI/경제 시스템으로 교체될 때까지의 임시 컨트롤러
    public class UnitPlacementController : MonoBehaviour
    {
        [SerializeField] private GameObject fallbackUnitPrefab;
        [SerializeField] private PathController pathController;
        [SerializeField] private EnemyFlowConfig flowConfig;
        [SerializeField] private EnemyRegistry enemyRegistry;
        [SerializeField] private UnitInventoryManager inventoryManager;
        [SerializeField] private Camera raycastCamera;
        [SerializeField] private Transform unitContainer;

        private UnitData _pendingUnitData;
        private Unit _selectedUnit;

        // 대기열에서 골라 배치를 기다리는 중인 유닛 (임시 UI가 대기열 버튼 클릭 시 설정한다)
        public UnitData PendingUnitData => _pendingUnitData;

        public Unit SelectedUnit => _selectedUnit;

        // 임시 대기열 UI가 유닛 버튼을 클릭했을 때 호출한다. 배치된 유닛 선택은 해제된다
        public void SelectQueuedUnit(UnitData data)
        {
            _pendingUnitData = data;
            SetSelectedUnit(null);
        }

        private void Update()
        {
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (!TryRaycast(out Vector3 hitPoint, out Unit clickedUnit))
            {
                return;
            }

            if (clickedUnit != null)
            {
                SetSelectedUnit(clickedUnit);
                return;
            }

            if (!IsPlaceablePosition(hitPoint))
            {
                return;
            }

            if (_selectedUnit != null)
            {
                _selectedUnit.SetPosition(hitPoint);
                return;
            }

            TryPlacePendingUnit(hitPoint);
        }

        private bool TryRaycast(out Vector3 hitPoint, out Unit clickedUnit)
        {
            hitPoint = default;
            clickedUnit = null;

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Ray ray = raycastCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                return false;
            }

            hitPoint = hit.point;
            clickedUnit = hit.collider.GetComponentInParent<Unit>();
            return true;
        }

        // 경로 중심선으로부터 통로 폭(EnemyFlowConfig.LaneHalfWidth) 밖에 있어야 배치 가능하다
        private bool IsPlaceablePosition(Vector3 position)
        {
            return pathController.PathSystem.DistanceToPath(position) >= flowConfig.LaneHalfWidth;
        }

        // 대기열에서 제거에 성공한 경우에만 실제로 필드에 생성한다
        private void TryPlacePendingUnit(Vector3 position)
        {
            if (_pendingUnitData == null)
            {
                return;
            }

            UnitData data = _pendingUnitData;
            if (!inventoryManager.Remove(data))
            {
                return;
            }

            _pendingUnitData = null;
            PlaceUnit(data, position);
        }

        private void PlaceUnit(UnitData data, Vector3 position)
        {
            GameObject prefab = data.UnitPrefab != null ? data.UnitPrefab : fallbackUnitPrefab;
            GameObject instance = Instantiate(prefab, position, Quaternion.identity, unitContainer);

            Unit unit = instance.GetComponent<Unit>();
            unit.Initialize(data, enemyRegistry);
        }

        private void SetSelectedUnit(Unit unit)
        {
            if (_selectedUnit != null)
            {
                _selectedUnit.IsSelected = false;
            }

            _selectedUnit = unit;

            if (_selectedUnit != null)
            {
                _selectedUnit.IsSelected = true;
                _pendingUnitData = null;
            }
        }
    }
}

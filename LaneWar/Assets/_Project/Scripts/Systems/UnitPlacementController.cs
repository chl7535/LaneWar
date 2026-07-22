using UnityEngine;
using UnityEngine.InputSystem;
using LaneWar.Core;
using LaneWar.Data;
using LaneWar.Units;

namespace LaneWar.Systems
{
    // 임시 배치 UI: 대기열(UnitInventoryManager)에서 유닛을 골라 그리드 칸에 배치/선택/재배치한다.
    // 칸 상태(점유/배치불가) 판정은 GridManager.GridSystem에 위임한다. 정식 UI/경제 시스템으로 교체될 때까지의 임시 컨트롤러
    public class UnitPlacementController : MonoBehaviour
    {
        [SerializeField] private GameObject fallbackUnitPrefab;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private EnemyRegistry enemyRegistry;
        [SerializeField] private UnitInventoryManager inventoryManager;
        [SerializeField] private Camera raycastCamera;
        [SerializeField] private Transform unitContainer;

        private UnitData _pendingUnitData;
        private Unit _selectedUnit;
        private GridCoord? _hoveredCell;
        private RangeIndicator _previewIndicator;

        // 대기열에서 골라 배치를 기다리는 중인 유닛 (임시 UI가 대기열 버튼 클릭 시 설정한다)
        public UnitData PendingUnitData => _pendingUnitData;

        public Unit SelectedUnit => _selectedUnit;

        // 대기열에서 유닛을 선택해 배치를 기다리는 중인지 여부. GridView가 그리드 오버레이 표시 여부를 결정할 때 참조한다
        public bool IsPlacingUnit => _pendingUnitData != null;

        // 현재 마우스가 올라간 그리드 칸. GridView가 호버 하이라이트에 사용한다
        public GridCoord? HoveredCell => _hoveredCell;

        private void Start()
        {
            // GridManager.Awake()가 먼저 끝난다는 보장 하에 여기(Start)서 확인해야 오탐(false negative)이 없다.
            // Awake에서 확인하면 GridManager의 Awake 실행 순서가 이 컴포넌트보다 늦을 경우 아직 초기화 전인
            // GridSystem을 "없다"고 잘못 판단할 수 있다
            if (gridManager == null)
            {
                gridManager = FindFirstObjectByType<GridManager>();
            }

            if (gridManager == null || gridManager.GridSystem == null)
            {
                Debug.LogError(
                    $"[UnitPlacementController] '{name}'이 사용할 GridManager(또는 그 GridSystem)를 찾지 못했습니다. " +
                    "Grid Manager 슬롯이 비어있지 않은지, GridManager 쪽 참조들이 올바른지 확인하세요.",
                    this);
                enabled = false;
                return;
            }

            _previewIndicator = CreatePreviewIndicator();
        }

        // 임시 대기열 UI가 유닛 버튼을 클릭했을 때 호출한다. 배치된 유닛 선택은 해제된다
        public void SelectQueuedUnit(UnitData data)
        {
            _pendingUnitData = data;
            SetSelectedUnit(null);
        }

        private void Update()
        {
            UpdateHoveredCell();
            UpdatePreviewIndicator();
            HandleMouseInput();
        }

        // 마우스 화면 좌표를 그리드 높이의 바닥 평면에 투영해 호버 중인 칸을 계산한다 (콜라이더 의존 없이 항상 계산 가능)
        private void UpdateHoveredCell()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                _hoveredCell = null;
                return;
            }

            GridSystem grid = gridManager.GridSystem;
            Ray ray = raycastCamera.ScreenPointToRay(mouse.position.ReadValue());
            var groundPlane = new Plane(Vector3.up, new Vector3(0f, grid.Origin.y, 0f));

            if (!groundPlane.Raycast(ray, out float enter))
            {
                _hoveredCell = null;
                return;
            }

            Vector3 point = ray.GetPoint(enter);
            _hoveredCell = grid.TryWorldToCell(point, out GridCoord coord) ? coord : (GridCoord?)null;
        }

        // 대기열에서 유닛을 놓기 전, 호버 중인 칸이 배치 가능하면 사거리 미리보기를 보여준다
        private void UpdatePreviewIndicator()
        {
            if (_pendingUnitData == null || !_hoveredCell.HasValue || !gridManager.GridSystem.IsPlaceable(_hoveredCell.Value))
            {
                _previewIndicator.SetVisible(false);
                return;
            }

            _previewIndicator.transform.position = gridManager.GridSystem.GetCellCenter(_hoveredCell.Value);
            _previewIndicator.SetRadius(_pendingUnitData.Range);
            _previewIndicator.SetVisible(true);
        }

        private void HandleMouseInput()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (TryRaycastUnit(out Unit clickedUnit) && clickedUnit != null)
            {
                SetSelectedUnit(clickedUnit);
                return;
            }

            if (!_hoveredCell.HasValue)
            {
                return;
            }

            GridCoord coord = _hoveredCell.Value;

            if (_selectedUnit != null)
            {
                TryMoveSelectedUnit(coord);
                return;
            }

            TryPlacePendingUnit(coord);
        }

        private bool TryRaycastUnit(out Unit clickedUnit)
        {
            clickedUnit = null;

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Ray ray = raycastCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                return false;
            }

            clickedUnit = hit.collider.GetComponentInParent<Unit>();
            return true;
        }

        // 선택된(이미 배치된) 유닛을 새 칸으로 옮긴다. 원래 칸을 비우고 새 칸을 점유한다
        private void TryMoveSelectedUnit(GridCoord coord)
        {
            GridSystem grid = gridManager.GridSystem;
            if (!_selectedUnit.Cell.HasValue || !grid.IsPlaceable(coord))
            {
                return;
            }

            grid.Free(_selectedUnit.Cell.Value);
            grid.TryOccupy(coord, _selectedUnit);
            _selectedUnit.SetPlacedCell(coord, grid.GetCellCenter(coord));
        }

        // 대기열에서 제거에 성공한 경우에만 실제로 필드에 생성한다
        private void TryPlacePendingUnit(GridCoord coord)
        {
            if (_pendingUnitData == null || !gridManager.GridSystem.IsPlaceable(coord))
            {
                return;
            }

            UnitData data = _pendingUnitData;
            if (!inventoryManager.Remove(data))
            {
                return;
            }

            _pendingUnitData = null;
            PlaceUnit(data, coord);
        }

        private void PlaceUnit(UnitData data, GridCoord coord)
        {
            GridSystem grid = gridManager.GridSystem;
            Vector3 position = grid.GetCellCenter(coord);

            GameObject prefab = data.UnitPrefab != null ? data.UnitPrefab : fallbackUnitPrefab;
            GameObject instance = Instantiate(prefab, position, Quaternion.identity, unitContainer);

            Unit unit = instance.GetComponent<Unit>();
            unit.Initialize(data, enemyRegistry);
            unit.SetPlacedCell(coord, position);
            grid.TryOccupy(coord, unit);
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

        private RangeIndicator CreatePreviewIndicator()
        {
            var previewObject = new GameObject("PlacementPreviewRange");
            previewObject.transform.SetParent(transform, false);
            RangeIndicator indicator = previewObject.AddComponent<RangeIndicator>();
            indicator.SetVisible(false);
            return indicator;
        }
    }
}

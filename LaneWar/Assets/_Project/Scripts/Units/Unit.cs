using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;
using LaneWar.Systems;

namespace LaneWar.Units
{
    // UnitData 기반으로 그리드 칸에 배치되어 고정된 채 사거리 내 적을 자동 타겟팅/공격하는 유닛 뷰 컴포넌트
    public class Unit : MonoBehaviour
    {
        [SerializeField] private UnitData unitData;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private RangeIndicator rangeIndicator;

        private EnemyRegistry _enemyRegistry;
        private UnitCombat _combat;
        private bool _isSelected;

        public UnitData Data => unitData;
        public GridCoord? Cell { get; private set; }

        // 선택 시 사거리 원(RangeIndicator)을 표시하고, 해제 시 감춘다
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (rangeIndicator != null)
                {
                    rangeIndicator.SetVisible(_isSelected);
                }
            }
        }

        // 배치 컨트롤러가 생성 직후 데이터와 타겟팅 대상 목록을 주입할 때 사용한다
        public void Initialize(UnitData data, EnemyRegistry enemyRegistry)
        {
            unitData = data;
            _enemyRegistry = enemyRegistry;
            _combat = new UnitCombat(unitData, transform.position);
            ApplyDisplayColor();

            if (rangeIndicator != null)
            {
                rangeIndicator.SetRadius(unitData.Range);
            }
        }

        // 배치/재배치 시 소속 그리드 칸과 칸 중앙 월드 좌표를 함께 갱신한다
        public void SetPlacedCell(GridCoord coord, Vector3 worldPosition)
        {
            Cell = coord;
            transform.position = worldPosition;
            if (_combat != null)
            {
                _combat.Position = worldPosition;
            }
        }

        private void Update()
        {
            if (_combat == null || _enemyRegistry == null)
            {
                return;
            }

            _combat.Tick(Time.deltaTime, _enemyRegistry.ActiveEnemies);
        }

        private void ApplyDisplayColor()
        {
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = unitData.DisplayColor;
            }
        }
    }
}

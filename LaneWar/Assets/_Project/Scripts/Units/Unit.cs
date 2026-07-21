using UnityEngine;
using LaneWar.Data;
using LaneWar.Systems;

namespace LaneWar.Units
{
    // UnitData 기반으로 배치되어 고정된 채 사거리 내 적을 자동 타겟팅/공격하는 유닛 뷰 컴포넌트
    public class Unit : MonoBehaviour
    {
        [SerializeField] private UnitData unitData;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private float selectionIndicatorRadius = 0.6f;

        private EnemyRegistry _enemyRegistry;
        private UnitCombat _combat;

        public UnitData Data => unitData;
        public bool IsSelected { get; set; }

        // 스포너/배치 컨트롤러가 생성 직후 데이터와 타겟팅 대상 목록을 주입할 때 사용한다
        public void Initialize(UnitData data, EnemyRegistry enemyRegistry)
        {
            unitData = data;
            _enemyRegistry = enemyRegistry;
            _combat = new UnitCombat(unitData, transform.position);
            ApplyDisplayColor();
        }

        // 재배치(드래그/클릭 이동) 시 뷰와 전투 로직 양쪽에 새 위치를 반영한다
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            if (_combat != null)
            {
                _combat.Position = position;
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

        // 사거리는 항상 원으로 표시하고, 선택된 유닛은 추가로 표시한다 (아트 나오기 전 임시 시각화)
        private void OnDrawGizmos()
        {
            if (unitData == null)
            {
                return;
            }

            Gizmos.color = new Color(unitData.DisplayColor.r, unitData.DisplayColor.g, unitData.DisplayColor.b, 0.25f);
            Gizmos.DrawWireSphere(transform.position, unitData.Range);

            if (IsSelected)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, selectionIndicatorRadius);
            }
        }
    }
}

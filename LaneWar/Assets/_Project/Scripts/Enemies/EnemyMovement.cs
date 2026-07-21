using UnityEngine;
using LaneWar.Systems;

namespace LaneWar.Enemies
{
    // EnemyFlowManager가 계산한 위치(경로 추종 + 자유 회피)를 매 프레임 반영하는 적 이동 뷰 컴포넌트
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private EnemyFlowManager flowManager;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float speedMultiplier = 1f;

        private EnemyFlowAgent _agent;

        // 최종 이동 속도 배율 훅. 스턴(0)/슬로우(0.5 등) 스킬이 실시간으로 설정해 정체를 유발할 수 있다. 스킬 로직은 아직 없다
        public float SpeedMultiplier
        {
            get => speedMultiplier;
            set
            {
                speedMultiplier = value;
                if (_agent != null)
                {
                    _agent.SpeedMultiplier = speedMultiplier;
                }
            }
        }

        private void Start()
        {
            if (_agent == null && flowManager != null)
            {
                Initialize(flowManager);
            }
        }

        // 스포너 등 외부에서 생성 직후 회피 시스템에 등록할 때 사용한다
        public void Initialize(EnemyFlowManager manager)
        {
            flowManager = manager;
            _agent = flowManager.RegisterEnemy(moveSpeed);
            _agent.SpeedMultiplier = speedMultiplier;
            transform.position = _agent.Position;
        }

        private void LateUpdate()
        {
            if (_agent == null)
            {
                return;
            }

            transform.position = _agent.Position;
        }

        private void OnDestroy()
        {
            if (_agent != null && flowManager != null)
            {
                flowManager.UnregisterEnemy(_agent);
            }
        }
    }
}

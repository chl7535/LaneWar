using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;
using LaneWar.Enemies;

namespace LaneWar.Systems
{
    // PathController의 경로 위에서 모든 적의 자유 회피(분리) 이동을 계산하는 EnemyFlowSystem을 매 프레임 구동한다
    public class EnemyFlowManager : MonoBehaviour
    {
        [SerializeField] private PathController pathController;
        [SerializeField] private EnemyFlowConfig flowConfig;

        private EnemyFlowSystem _flowSystem;

        private void Awake()
        {
            _flowSystem = new EnemyFlowSystem(pathController.PathSystem, flowConfig);
        }

        private void Update()
        {
            _flowSystem.Tick(Time.deltaTime);
        }

        // 스포너가 새 적을 생성할 때 호출해 회피 시스템에 등록하고 개별 에이전트를 받는다
        public EnemyFlowAgent RegisterEnemy(float moveSpeed)
        {
            return _flowSystem.CreateAgent(moveSpeed);
        }

        public void UnregisterEnemy(EnemyFlowAgent agent)
        {
            _flowSystem.RemoveAgent(agent);
        }
    }
}

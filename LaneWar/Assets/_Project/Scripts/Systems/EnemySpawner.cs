using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;
using LaneWar.Enemies;

namespace LaneWar.Systems
{
    // Spawning 상태일 때 RoundConfig에 정의된 간격/수량으로 경로 시작점(왼쪽 위)에 적을 생성한다
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private RoundConfig roundConfig;
        [SerializeField] private PathController pathController;
        [SerializeField] private EnemyFlowManager flowManager;
        [SerializeField] private EnemyRegistry enemyRegistry;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform enemyContainer;

        private float _spawnTimer;
        private int _spawnedThisRound;

        private void Update()
        {
            if (roundManager.CurrentState != RoundState.Spawning)
            {
                _spawnTimer = 0f;
                _spawnedThisRound = 0;
                return;
            }

            if (_spawnedThisRound >= roundConfig.EnemiesPerRound)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < roundConfig.SpawnInterval)
            {
                return;
            }

            _spawnTimer -= roundConfig.SpawnInterval;
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            Vector3 spawnPosition = pathController.PathSystem.GetWaypoint(0);
            GameObject instance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemyContainer);

            EnemyMovement movement = instance.GetComponent<EnemyMovement>();
            movement.Initialize(flowManager);

            Enemy enemy = instance.GetComponent<Enemy>();
            enemy.Died += HandleEnemyDied;
            enemyRegistry.Register(enemy);

            _spawnedThisRound++;
            roundManager.NotifyEnemySpawned();
        }

        // 사망한 적을 타겟팅 레지스트리에서 제거하고, 라운드 생존 카운트 감소 + 처치 보상 지급을 처리한다
        private void HandleEnemyDied(Enemy enemy)
        {
            enemy.Died -= HandleEnemyDied;
            enemyRegistry.Unregister(enemy);
            roundManager.NotifyEnemyDied();
            currencyManager.AddGold(enemy.KillReward);
        }
    }
}

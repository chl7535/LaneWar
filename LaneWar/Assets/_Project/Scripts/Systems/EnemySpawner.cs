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
            movement.Initialize(pathController);

            _spawnedThisRound++;
            roundManager.NotifyEnemySpawned();
        }
    }
}

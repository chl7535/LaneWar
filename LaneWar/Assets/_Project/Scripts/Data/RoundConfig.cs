using UnityEngine;

namespace LaneWar.Data
{
    // 라운드 진행/스폰/게임오버 관련 고정 수치를 데이터로 정의하는 ScriptableObject
    [CreateAssetMenu(fileName = "RoundConfig", menuName = "LaneWar/Round Config")]
    public class RoundConfig : ScriptableObject
    {
        [Header("라운드 타이밍 (초)")]
        [SerializeField] private float spawnPhaseDuration = 40f;
        [SerializeField] private float intermissionDuration = 20f;
        [SerializeField] private float countdownDuration = 5f;

        [Header("스폰")]
        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private int enemiesPerRound = 40;

        [Header("게임오버")]
        [SerializeField] private int maxAliveEnemies = 100;

        [Header("보스")]
        [SerializeField] private int roundsPerBoss = 4;

        public float SpawnPhaseDuration => spawnPhaseDuration;
        public float IntermissionDuration => intermissionDuration;
        public float CountdownDuration => countdownDuration;
        public float SpawnInterval => spawnInterval;
        public int EnemiesPerRound => enemiesPerRound;
        public int MaxAliveEnemies => maxAliveEnemies;
        public int RoundsPerBoss => roundsPerBoss;
    }
}

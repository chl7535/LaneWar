using UnityEngine;
using LaneWar.Data;

namespace LaneWar.Core
{
    // RoundStateMachine을 씬에서 구동하고 RoundConfig 데이터를 연결하는 라운드 매니저
    public class RoundManager : MonoBehaviour
    {
        [SerializeField] private RoundConfig roundConfig;

        private RoundStateMachine _stateMachine;

        public event System.Action<RoundState> StateChanged;
        public event System.Action<int> RoundStarted;
        public event System.Action<int> RoundEnded;
        public event System.Action GameOverTriggered;

        public RoundState CurrentState => _stateMachine.CurrentState;
        public int CurrentRound => _stateMachine.CurrentRound;
        public bool IsBossRound => _stateMachine.IsBossRound;
        public int AliveEnemyCount => _stateMachine.AliveEnemyCount;
        public int MaxAliveEnemies => roundConfig.MaxAliveEnemies;
        public float TimeRemainingInState => _stateMachine.TimeRemainingInState;

        private void Awake()
        {
            _stateMachine = new RoundStateMachine(
                roundConfig.SpawnPhaseDuration,
                roundConfig.IntermissionDuration,
                roundConfig.CountdownDuration,
                roundConfig.MaxAliveEnemies,
                roundConfig.RoundsPerBoss);

            _stateMachine.StateChanged += HandleStateChanged;
            _stateMachine.RoundStarted += HandleRoundStarted;
            _stateMachine.RoundEnded += HandleRoundEnded;
            _stateMachine.GameOverTriggered += HandleGameOverTriggered;
        }

        private void Update()
        {
            _stateMachine.Tick(Time.deltaTime);
        }

        public void NotifyEnemySpawned()
        {
            _stateMachine.NotifyEnemySpawned();
        }

        public void NotifyEnemyDied()
        {
            _stateMachine.NotifyEnemyDied();
        }

        private void HandleStateChanged(RoundState state)
        {
            StateChanged?.Invoke(state);
        }

        private void HandleRoundStarted(int round)
        {
            RoundStarted?.Invoke(round);

            if (IsBossRound)
            {
                // TODO: 보스 스폰/연출 로직 (추후 구현). 지금은 훅만 남겨둔다
                Debug.Log($"[RoundManager] Round {round} is a boss round.");
            }
        }

        private void HandleRoundEnded(int round)
        {
            RoundEnded?.Invoke(round);
        }

        private void HandleGameOverTriggered()
        {
            GameOverTriggered?.Invoke();
        }
    }
}

using System;

namespace LaneWar.Core
{
    // 라운드 진행(스폰/정비/카운트다운)과 게임오버 조건을 관리하는 순수 상태 기계. MonoBehaviour에 의존하지 않는다
    public class RoundStateMachine
    {
        private readonly float _spawnPhaseDuration;
        private readonly float _intermissionDuration;
        private readonly float _countdownDuration;
        private readonly int _maxAliveEnemies;
        private readonly int _roundsPerBoss;

        private float _elapsedInState;

        public RoundState CurrentState { get; private set; }
        public int CurrentRound { get; private set; }
        public bool IsBossRound { get; private set; }
        public int AliveEnemyCount { get; private set; }

        public event Action<RoundState> StateChanged;
        public event Action<int> RoundStarted;
        public event Action<int> RoundEnded;
        public event Action GameOverTriggered;

        public RoundStateMachine(
            float spawnPhaseDuration,
            float intermissionDuration,
            float countdownDuration,
            int maxAliveEnemies,
            int roundsPerBoss)
        {
            _spawnPhaseDuration = spawnPhaseDuration;
            _intermissionDuration = intermissionDuration;
            _countdownDuration = countdownDuration;
            _maxAliveEnemies = maxAliveEnemies;
            _roundsPerBoss = roundsPerBoss;

            CurrentRound = 1;
            IsBossRound = _roundsPerBoss > 0 && CurrentRound % _roundsPerBoss == 0;
            CurrentState = RoundState.Spawning;
            _elapsedInState = 0f;
        }

        public float TimeRemainingInState
        {
            get
            {
                float duration = GetDurationForState(CurrentState);
                float remaining = duration - _elapsedInState;
                return remaining < 0f ? 0f : remaining;
            }
        }

        public void Tick(float deltaTime)
        {
            if (CurrentState == RoundState.GameOver)
            {
                return;
            }

            _elapsedInState += deltaTime;
            float duration = GetDurationForState(CurrentState);

            if (_elapsedInState < duration)
            {
                return;
            }

            switch (CurrentState)
            {
                case RoundState.Spawning:
                    TransitionTo(RoundState.Intermission);
                    RoundEnded?.Invoke(CurrentRound);
                    break;
                case RoundState.Intermission:
                    TransitionTo(RoundState.Countdown);
                    break;
                case RoundState.Countdown:
                    CurrentRound++;
                    IsBossRound = _roundsPerBoss > 0 && CurrentRound % _roundsPerBoss == 0;
                    TransitionTo(RoundState.Spawning);
                    RoundStarted?.Invoke(CurrentRound);
                    break;
            }
        }

        // 적 1마리가 스폰되었음을 알린다. 생존 수가 한도에 도달하면 즉시 GameOver로 전환한다
        public void NotifyEnemySpawned()
        {
            if (CurrentState == RoundState.GameOver)
            {
                return;
            }

            AliveEnemyCount++;

            if (AliveEnemyCount >= _maxAliveEnemies)
            {
                TransitionTo(RoundState.GameOver);
                GameOverTriggered?.Invoke();
            }
        }

        // 적 1마리가 사망했음을 알린다. 생존 수를 감소시켜 게임오버(누적 100) 판정에 반영한다
        public void NotifyEnemyDied()
        {
            if (CurrentState == RoundState.GameOver)
            {
                return;
            }

            AliveEnemyCount = Math.Max(0, AliveEnemyCount - 1);
        }

        private float GetDurationForState(RoundState state)
        {
            switch (state)
            {
                case RoundState.Spawning:
                    return _spawnPhaseDuration;
                case RoundState.Intermission:
                    return _intermissionDuration;
                case RoundState.Countdown:
                    return _countdownDuration;
                default:
                    return 0f;
            }
        }

        private void TransitionTo(RoundState next)
        {
            CurrentState = next;
            _elapsedInState = 0f;
            StateChanged?.Invoke(next);
        }
    }
}

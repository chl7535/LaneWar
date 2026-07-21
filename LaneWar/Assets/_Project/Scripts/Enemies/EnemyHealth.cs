using System;

namespace LaneWar.Enemies
{
    // 적의 체력 증감과 사망 판정을 담당하는 순수 로직. MonoBehaviour에 의존하지 않아 서버 로직 분리/유닛 테스트가 쉽다
    public class EnemyHealth
    {
        public EnemyHealth(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public float MaxHealth { get; }
        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        public event Action Died;

        // HP 0 이하 도달 시 1회에 한해 사망 이벤트를 발생시킨다
        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth -= amount;

            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                IsDead = true;
                Died?.Invoke();
            }
        }
    }
}

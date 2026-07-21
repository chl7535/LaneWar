using UnityEngine;
using LaneWar.Data;

namespace LaneWar.Enemies
{
    // 적의 체력/피격/사망 진입점. 사망 시 사망 이벤트를 방송(재화/포인트용)하고 자신을 제거한다
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;

        private EnemyHealth _health;

        public bool IsDead => _health != null && _health.IsDead;
        public float CurrentHealth => _health != null ? _health.CurrentHealth : 0f;
        public float MaxHealth => _health != null ? _health.MaxHealth : 0f;
        public int KillReward => enemyData != null ? enemyData.KillReward : 0;

        // 사망 시 방송되는 이벤트. 리스너(스포너 등)가 라운드 생존 카운트 감소, 재화 지급 등을 구독해 처리한다
        public event System.Action<Enemy> Died;

        private void Awake()
        {
            _health = new EnemyHealth(enemyData.MaxHealth);
            _health.Died += HandleDied;
        }

        public void TakeDamage(float amount)
        {
            _health.TakeDamage(amount);
        }

        private void HandleDied()
        {
            Died?.Invoke(this);
            Destroy(gameObject);
        }
    }
}

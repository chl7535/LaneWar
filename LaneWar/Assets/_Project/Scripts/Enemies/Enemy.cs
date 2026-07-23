using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;

namespace LaneWar.Enemies
{
    // 적의 체력/방어력/피격/사망 진입점. 사망 시 사망 이벤트를 방송(재화/포인트용)하고 자신을 제거한다
    public class Enemy : MonoBehaviour
    {
        // InitializeStats가 호출되지 않은 경우에만 쓰이는 안전장치 값 (DifficultyConfig의 기본 방어 효율 상수와 동일)
        private const float FallbackArmorEfficiencyConstant = 100f;

        [SerializeField] private EnemyData enemyData;

        private EnemyHealth _health;
        private float _armor;
        private float _armorEfficiencyConstant;
        private bool _logDamageCalculations;

        public EnemyData Data => enemyData;
        public bool IsDead => _health != null && _health.IsDead;
        public float CurrentHealth => _health != null ? _health.CurrentHealth : 0f;
        public float MaxHealth => _health != null ? _health.MaxHealth : 0f;
        public float Armor => _armor;
        public int KillReward => enemyData != null ? enemyData.KillReward : 0;

        // 사망 시 방송되는 이벤트. 리스너(스포너 등)가 라운드 생존 카운트 감소, 재화 지급 등을 구독해 처리한다
        public event System.Action<Enemy> Died;

        private void Awake()
        {
            _armor = enemyData.BaseArmor;
            _armorEfficiencyConstant = FallbackArmorEfficiencyConstant;
            SetHealth(enemyData.MaxHealth);
        }

        // 스포너가 라운드 난이도 스케일링이 적용된 체력/방어력으로 스폰 시점에 1회 재설정한다.
        // 스폰 이후 이미 필드에 있는 적에게는 소급 적용되지 않는다
        public void InitializeStats(float maxHealth, float armor, float armorEfficiencyConstant, bool logDamageCalculations)
        {
            _armor = armor;
            _armorEfficiencyConstant = armorEfficiencyConstant;
            _logDamageCalculations = logDamageCalculations;
            SetHealth(maxHealth);
        }

        // attackPower(공격력 원본)를 받아 CombatCalculator로 방어력 감소를 적용한 뒤 체력에 반영한다
        public void TakeDamage(float attackPower)
        {
            float actualDamage = CombatCalculator.CalculateDamage(attackPower, _armor, _armorEfficiencyConstant);
            _health.TakeDamage(actualDamage);

            if (_logDamageCalculations)
            {
                Debug.Log($"[Combat] {name}: 공격력 {attackPower:0.0}, 방어력 {_armor:0.0} → 실제데미지 {actualDamage:0.0}");
            }
        }

        private void HandleDied()
        {
            Died?.Invoke(this);
            Destroy(gameObject);
        }

        private void SetHealth(float maxHealth)
        {
            _health = new EnemyHealth(maxHealth);
            _health.Died += HandleDied;
        }
    }
}

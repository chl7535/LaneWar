using UnityEngine;
using LaneWar.Core;

namespace LaneWar.Data
{
    // 라운드가 오를수록 적 체력/방어력이 상승하는 규칙을 정의하는 ScriptableObject.
    // 체력 상승률과 방어력 상승률을 독립적으로 조절할 수 있어, 동시 복리 상승으로 인한 급격한 난이도 상승을 인스펙터에서 바로 조정할 수 있다
    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "LaneWar/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        [Header("상승 방식 (복리: 곱연산, 단리: 덧셈)")]
        [SerializeField] private GrowthMode growthMode = GrowthMode.Compound;

        [Header("체력 스케일링")]
        [SerializeField] private float healthBaseMultiplier = 1f;
        [SerializeField] [Range(0f, 2f)] private float healthGrowthRatePerRound = 0.25f;

        [Header("방어력 스케일링")]
        [SerializeField] private float armorBaseMultiplier = 1f;
        [SerializeField] [Range(0f, 2f)] private float armorGrowthRatePerRound = 0.25f;

        [Header("방어력 데미지 공식: 실제데미지 = 공격력 x (K / (K + 방어력))")]
        [SerializeField] private float armorEfficiencyConstant = 100f;

        [Header("보스 배율 (자리만 마련, 아직 미적용)")]
        [SerializeField] private float bossHealthMultiplier = 2f;
        [SerializeField] private float bossArmorMultiplier = 2f;

        [Header("디버그")]
        [SerializeField] private bool logDamageCalculations = false;

        public GrowthMode GrowthMode => growthMode;
        public float ArmorEfficiencyConstant => armorEfficiencyConstant;
        public float BossHealthMultiplier => bossHealthMultiplier;
        public float BossArmorMultiplier => bossArmorMultiplier;
        public bool LogDamageCalculations => logDamageCalculations;

        // 스폰 시점의 라운드 번호로 체력 배율을 계산한다 (스폰 이후에는 소급 적용되지 않음)
        public float GetHealthMultiplier(int round)
        {
            return DifficultyScaler.GetMultiplier(round, healthBaseMultiplier, healthGrowthRatePerRound, growthMode);
        }

        // 스폰 시점의 라운드 번호로 방어력 배율을 계산한다 (스폰 이후에는 소급 적용되지 않음)
        public float GetArmorMultiplier(int round)
        {
            return DifficultyScaler.GetMultiplier(round, armorBaseMultiplier, armorGrowthRatePerRound, growthMode);
        }
    }
}

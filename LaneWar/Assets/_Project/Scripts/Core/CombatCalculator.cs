namespace LaneWar.Core
{
    // 방어력에 의한 데미지 감소(비율 감소 공식)를 계산하는 순수 로직. 모든 공격 판정은 이 계산을 거친다
    public static class CombatCalculator
    {
        // 실제데미지 = 공격력 x (armorEfficiencyConstant / (armorEfficiencyConstant + 방어력))
        // 방어력이 커져도 분모가 함께 커질 뿐이라 데미지가 0으로 수렴하지 않는다
        public static float CalculateDamage(float attackPower, float armor, float armorEfficiencyConstant)
        {
            if (attackPower <= 0f)
            {
                return 0f;
            }

            float safeArmor = armor < 0f ? 0f : armor;
            float safeConstant = armorEfficiencyConstant <= 0f ? 1f : armorEfficiencyConstant;
            float mitigation = safeConstant / (safeConstant + safeArmor);
            return attackPower * mitigation;
        }
    }
}

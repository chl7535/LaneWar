using LaneWar.Data;

namespace LaneWar.Units
{
    // AttackStyle 값에 해당하는 공격 전략(IAttackBehavior) 인스턴스를 만들어주는 팩토리.
    // 새 스타일을 추가할 때는 이 매핑에 분기만 추가하면 된다
    public static class AttackBehaviorFactory
    {
        public static IAttackBehavior Create(AttackStyle style)
        {
            switch (style)
            {
                case AttackStyle.AoeRanged:
                    return new AoeAttackBehavior();
                case AttackStyle.SingleMelee:
                case AttackStyle.SingleRanged:
                default:
                    return new SingleTargetAttackBehavior();
            }
        }
    }
}

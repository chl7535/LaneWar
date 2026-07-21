namespace LaneWar.Units
{
    // 유닛의 공격 스타일(타겟팅 + 타격 방식)을 정의하는 전략 인터페이스.
    // 새 스타일(스킬 등)은 이 인터페이스의 구현체를 추가하는 것만으로 확장할 수 있다
    public interface IAttackBehavior
    {
        // 이번 공속 타이밍에 사거리 내 적에게 데미지를 적용한다
        void Attack(UnitAttackContext context);
    }
}

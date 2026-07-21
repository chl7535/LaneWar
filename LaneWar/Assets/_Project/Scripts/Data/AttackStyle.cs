namespace LaneWar.Data
{
    // 유닛의 타겟팅/연출 방식을 구분하는 공격 스타일. DPS 자체는 스타일과 무관하게 damage x attacksPerSecond로 통일한다
    public enum AttackStyle
    {
        SingleMelee,
        SingleRanged,
        AoeRanged
    }
}

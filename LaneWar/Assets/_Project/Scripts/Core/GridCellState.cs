namespace LaneWar.Core
{
    // 배치 그리드 한 칸의 상태: 비어있음 / 유닛이 점유함 / 배치 불가(적 통로와 겹침)
    public enum GridCellState
    {
        Empty,
        Occupied,
        Blocked
    }
}

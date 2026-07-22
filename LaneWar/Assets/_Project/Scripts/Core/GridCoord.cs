using System;

namespace LaneWar.Core
{
    // 배치 그리드의 한 칸을 가리키는 (행, 열) 좌표. 인접 칸 탐색 등 향후 조합 시스템 확장에 재사용된다
    public readonly struct GridCoord : IEquatable<GridCoord>
    {
        public GridCoord(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }
        public int Column { get; }

        public bool Equals(GridCoord other) => Row == other.Row && Column == other.Column;
        public override bool Equals(object obj) => obj is GridCoord other && Equals(other);
        public override int GetHashCode() => (Row * 397) ^ Column;

        public static bool operator ==(GridCoord a, GridCoord b) => a.Equals(b);
        public static bool operator !=(GridCoord a, GridCoord b) => !a.Equals(b);

        public override string ToString() => $"({Row}, {Column})";
    }
}

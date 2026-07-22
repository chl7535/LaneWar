using System.Collections.Generic;
using UnityEngine;
using LaneWar.Data;

namespace LaneWar.Core
{
    // 배치 그리드의 칸 상태(비어있음/점유됨/배치불가)를 관리하고 월드좌표<->그리드좌표 변환을 제공하는 순수 로직.
    // 칸의 크기와 위치는 GridConfig의 행/열 수만으로 LaneLayout.PlacementBounds를 정확히 채우도록 역산되므로,
    // 통로 폭/경로가 바뀌어도 그리드가 배치 가능 영역 밖으로 삐져나가거나 안쪽에 빈틈을 남기지 않는다.
    // MonoBehaviour에 의존하지 않아 유닛 테스트 및 향후 서버 로직 분리가 가능하다
    public class GridSystem
    {
        private readonly GridCellState[,] _baseStates;
        private readonly object[,] _occupants;

        public GridSystem(GridConfig config, LaneLayout laneLayout)
        {
            Rows = config.Rows;
            Columns = config.Columns;
            PlacementBounds = laneLayout.PlacementBounds;

            CellWidth = PlacementBounds.size.x / Columns;
            CellHeight = PlacementBounds.size.z / Rows;
            Origin = new Vector3(PlacementBounds.min.x, config.FloorHeight, PlacementBounds.min.z);

            _baseStates = new GridCellState[Rows, Columns];
            _occupants = new object[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    // 그리드는 PlacementBounds를 정확히 채우도록 만들어지므로 통상 항상 Empty다.
                    // 향후 비사각형 경로 등으로 확장될 경우에 대비한 방어적 검사
                    Vector3 center = GetCellCenter(new GridCoord(row, column));
                    bool insidePlacementArea =
                        center.x >= PlacementBounds.min.x && center.x <= PlacementBounds.max.x &&
                        center.z >= PlacementBounds.min.z && center.z <= PlacementBounds.max.z;

                    _baseStates[row, column] = insidePlacementArea ? GridCellState.Empty : GridCellState.Blocked;
                }
            }
        }

        public int Rows { get; }
        public int Columns { get; }
        public float CellWidth { get; }
        public float CellHeight { get; }
        public Vector3 Origin { get; }
        public Bounds PlacementBounds { get; }

        public bool IsInside(GridCoord coord)
        {
            return coord.Row >= 0 && coord.Row < Rows && coord.Column >= 0 && coord.Column < Columns;
        }

        public Vector3 GetCellCenter(GridCoord coord)
        {
            return new Vector3(
                Origin.x + (coord.Column + 0.5f) * CellWidth,
                Origin.y,
                Origin.z + (coord.Row + 0.5f) * CellHeight);
        }

        // 월드 좌표(XZ)가 속한 칸을 계산한다. 그리드 밖이면 false를 반환한다
        public bool TryWorldToCell(Vector3 worldPosition, out GridCoord coord)
        {
            int column = Mathf.FloorToInt((worldPosition.x - Origin.x) / CellWidth);
            int row = Mathf.FloorToInt((worldPosition.z - Origin.z) / CellHeight);
            coord = new GridCoord(row, column);
            return IsInside(coord);
        }

        public GridCellState GetState(GridCoord coord)
        {
            if (!IsInside(coord))
            {
                return GridCellState.Blocked;
            }

            if (_occupants[coord.Row, coord.Column] != null)
            {
                return GridCellState.Occupied;
            }

            return _baseStates[coord.Row, coord.Column];
        }

        public bool IsPlaceable(GridCoord coord)
        {
            return GetState(coord) == GridCellState.Empty;
        }

        // occupant를 칸에 배치한다. 이미 점유되었거나 배치 불가 칸이면 실패한다
        public bool TryOccupy(GridCoord coord, object occupant)
        {
            if (!IsPlaceable(coord))
            {
                return false;
            }

            _occupants[coord.Row, coord.Column] = occupant;
            return true;
        }

        public void Free(GridCoord coord)
        {
            if (!IsInside(coord))
            {
                return;
            }

            _occupants[coord.Row, coord.Column] = null;
        }

        // 향후 조합 시스템에서 인접 칸을 탐색할 때 사용할 4방향 이웃 좌표
        public IEnumerable<GridCoord> GetOrthogonalNeighbors(GridCoord coord)
        {
            yield return new GridCoord(coord.Row + 1, coord.Column);
            yield return new GridCoord(coord.Row - 1, coord.Column);
            yield return new GridCoord(coord.Row, coord.Column + 1);
            yield return new GridCoord(coord.Row, coord.Column - 1);
        }
    }
}

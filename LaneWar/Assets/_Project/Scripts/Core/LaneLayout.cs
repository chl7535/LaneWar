using UnityEngine;

namespace LaneWar.Core
{
    // 경로 웨이포인트(통로 중심선) + 통로 폭(EnemyFlowConfig.LaneHalfWidth — EnemyFlowAgent가 좌우 회피에 쓰는
    // 값과 반드시 동일한 값을 받아야 한다)으로부터 통로 영역과 배치 가능 영역을 계산하는 단일 기준(SSOT).
    // GridSystem(배치 그리드)과 바닥 시각 표시(PathFloorView)가 반드시 이 계산 결과 하나만을 공유해야 서로 어긋나지 않는다.
    //
    // 통로는 중심선을 기준으로 안쪽/바깥쪽 "양방향"으로 각각 LaneHalfWidth만큼 걸쳐 있다
    // (EnemyFlowAgent의 lateralOffset이 -LaneHalfWidth ~ +LaneHalfWidth 범위로 양쪽에 다 적용되기 때문).
    // 따라서 통로 전체 폭은 LaneHalfWidth*2이며, PlacementBounds(안쪽 경계)와 LaneOuterBounds(바깥쪽 경계)
    // 양쪽 모두 중심선에서 파생시켜야 실제 적 이동 범위와 어긋나지 않는다
    public class LaneLayout
    {
        public LaneLayout(PathSystem pathSystem, float laneHalfWidth)
        {
            LaneHalfWidth = laneHalfWidth;
            CenterlineBounds = ComputeWaypointBounds(pathSystem);

            Vector3 halfWidthExtent = new Vector3(laneHalfWidth, 0f, laneHalfWidth);
            PlacementBounds = new Bounds(CenterlineBounds.center, CenterlineBounds.size - halfWidthExtent * 2f);
            LaneOuterBounds = new Bounds(CenterlineBounds.center, CenterlineBounds.size + halfWidthExtent * 2f);
        }

        // 경로 웨이포인트를 그대로 잇는 통로 중심선의 바운딩 박스 (적의 회피 이동 오프셋이 0일 때의 위치)
        public Bounds CenterlineBounds { get; }

        // 중심선에서 안쪽으로 LaneHalfWidth만큼 줄인, 유닛을 배치할 수 있는 사각 영역 (통로의 안쪽 경계)
        public Bounds PlacementBounds { get; }

        // 중심선에서 바깥쪽으로 LaneHalfWidth만큼 넓힌, 적이 회피 이동으로 벗어날 수 있는 최대 범위 (통로의 바깥쪽 경계)
        public Bounds LaneOuterBounds { get; }

        public float LaneHalfWidth { get; }

        private static Bounds ComputeWaypointBounds(PathSystem pathSystem)
        {
            var bounds = new Bounds(pathSystem.GetWaypoint(0), Vector3.zero);
            for (int i = 1; i < pathSystem.WaypointCount; i++)
            {
                bounds.Encapsulate(pathSystem.GetWaypoint(i));
            }
            return bounds;
        }
    }
}

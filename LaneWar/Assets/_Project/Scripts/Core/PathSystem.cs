using System.Collections.Generic;
using UnityEngine;

namespace LaneWar.Core
{
    // 웨이포인트 목록을 기반으로 진행도(구간 인덱스 + 보간값)에 따른 월드 좌표를 계산하는 순수 경로 로직
    public class PathSystem
    {
        private readonly IReadOnlyList<Vector3> _waypoints;

        public PathSystem(IReadOnlyList<Vector3> waypoints)
        {
            _waypoints = waypoints;
        }

        public int WaypointCount => _waypoints.Count;

        public int WrapIndex(int index)
        {
            int count = _waypoints.Count;
            return ((index % count) + count) % count;
        }

        public Vector3 GetWaypoint(int index)
        {
            return _waypoints[WrapIndex(index)];
        }

        public float GetSegmentLength(int fromIndex)
        {
            return Vector3.Distance(GetWaypoint(fromIndex), GetWaypoint(fromIndex + 1));
        }

        // fromIndex 지점에서 다음 지점까지 t(0~1)만큼 보간한 월드 좌표를 반환한다
        public Vector3 GetPositionAtProgress(int fromIndex, float t)
        {
            Vector3 from = GetWaypoint(fromIndex);
            Vector3 to = GetWaypoint(fromIndex + 1);
            return Vector3.Lerp(from, to, Mathf.Clamp01(t));
        }

        // 임의의 지점에서 경로(폴리라인)까지의 최단 거리를 계산한다 (유닛 배치 가능 영역 판정 등에 사용)
        public float DistanceToPath(Vector3 point)
        {
            float minDistance = float.MaxValue;
            for (int i = 0; i < _waypoints.Count; i++)
            {
                float distance = DistanceToSegment(point, GetWaypoint(i), GetWaypoint(i + 1));
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
            return minDistance;
        }

        private static float DistanceToSegment(Vector3 point, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            float lengthSquared = ab.sqrMagnitude;
            if (lengthSquared <= Mathf.Epsilon)
            {
                return Vector3.Distance(point, a);
            }

            float t = Mathf.Clamp01(Vector3.Dot(point - a, ab) / lengthSquared);
            Vector3 projection = a + ab * t;
            return Vector3.Distance(point, projection);
        }
    }
}

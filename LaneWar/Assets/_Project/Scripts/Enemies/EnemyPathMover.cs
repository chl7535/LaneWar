using UnityEngine;
using LaneWar.Core;

namespace LaneWar.Enemies
{
    // 경로 위에서의 진행 상태(구간 인덱스 + 구간 내 보간값)를 계산하는 순수 이동 로직. MonoBehaviour에 의존하지 않는다
    public class EnemyPathMover
    {
        private readonly PathSystem _pathSystem;
        private int _currentIndex;
        private float _segmentT;

        public EnemyPathMover(PathSystem pathSystem, int startIndex = 0)
        {
            _pathSystem = pathSystem;
            _currentIndex = startIndex;
            _segmentT = 0f;
        }

        public Vector3 CurrentPosition => _pathSystem.GetPositionAtProgress(_currentIndex, _segmentT);
        public int CurrentIndex => _currentIndex;
        public float SegmentT => _segmentT;

        // speed(단위/초)만큼 진행도를 갱신하고 새 월드 좌표를 반환한다. 마지막 지점 도달 시 처음으로 순환한다
        public Vector3 Advance(float speed, float deltaTime)
        {
            float distanceRemaining = speed * deltaTime;

            while (distanceRemaining > 0f)
            {
                float segmentLength = _pathSystem.GetSegmentLength(_currentIndex);
                if (segmentLength <= Mathf.Epsilon)
                {
                    _currentIndex = _pathSystem.WrapIndex(_currentIndex + 1);
                    _segmentT = 0f;
                    continue;
                }

                float remainingOnSegment = (1f - _segmentT) * segmentLength;
                if (distanceRemaining < remainingOnSegment)
                {
                    _segmentT += distanceRemaining / segmentLength;
                    distanceRemaining = 0f;
                }
                else
                {
                    distanceRemaining -= remainingOnSegment;
                    _currentIndex = _pathSystem.WrapIndex(_currentIndex + 1);
                    _segmentT = 0f;
                }
            }

            return CurrentPosition;
        }
    }
}

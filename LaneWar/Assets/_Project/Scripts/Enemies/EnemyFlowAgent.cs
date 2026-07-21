using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;

namespace LaneWar.Enemies
{
    // 경로 추종(전진) + 분리력(회피) + 통로 이탈 복귀력을 합산해 실제 월드 위치를 계산하는 순수 이동 로직.
    // 물리 충돌(Collider/Rigidbody) 없이 위치 계산만으로 회피를 표현하며, MonoBehaviour에 의존하지 않는다
    public class EnemyFlowAgent
    {
        private readonly PathSystem _pathSystem;
        private readonly EnemyFlowConfig _config;
        private readonly EnemyPathMover _mover;
        private readonly float _moveSpeed;

        private float _lateralOffset;
        private float _pendingLateralForce;

        public EnemyFlowAgent(PathSystem pathSystem, EnemyFlowConfig config, float moveSpeed)
        {
            _pathSystem = pathSystem;
            _config = config;
            _moveSpeed = moveSpeed;
            _mover = new EnemyPathMover(pathSystem);
            Position = _mover.CurrentPosition;
        }

        public Vector3 Position { get; private set; }

        // 최종 이동 속도에 곱해지는 배율 훅. 스턴(0)/슬로우(0.5 등) 스킬이 실시간으로 조작해 정체를 유발할 수 있다. 기본값 1(정상 속도)
        public float SpeedMultiplier { get; set; } = 1f;

        // 현재 진행 방향 기준 좌우 회피에 쓰이는 수평 방향 (진행 방향을 Y축 기준으로 90도 회전한 벡터)
        public Vector3 Right => Vector3.Cross(Vector3.up, ComputeTangent()).normalized;

        // 다른 에이전트로부터 밀려나는 분리력(좌우 성분)을 이번 프레임 값에 누적한다. 프레임당 여러 번 호출될 수 있다
        public void AddSeparationForce(float lateralForceAmount)
        {
            _pendingLateralForce += lateralForceAmount;
        }

        // 경로를 따라 전진하고, 누적된 분리력 + 통로 이탈 시 복귀력을 좌우 오프셋에 반영해 최종 월드 위치를 계산한다
        public void Tick(float deltaTime)
        {
            float halfWidth = _config.LaneHalfWidth;
            float excess = Mathf.Abs(_lateralOffset) - halfWidth;
            float returnForce = excess > 0f
                ? -Mathf.Sign(_lateralOffset) * excess * _config.CenterlineReturnStrength
                : 0f;

            _lateralOffset += (_pendingLateralForce + returnForce) * deltaTime;
            _lateralOffset = Mathf.Clamp(_lateralOffset, -halfWidth, halfWidth);
            _pendingLateralForce = 0f;

            _mover.Advance(_moveSpeed * SpeedMultiplier, deltaTime);

            Position = _mover.CurrentPosition + Right * _lateralOffset;
        }

        // 현재 구간의 진행 방향을 다음 구간 방향으로 코너 부근에서 부드럽게 보간한다 (보간 구간 길이 = 통로 폭)
        private Vector3 ComputeTangent()
        {
            int index = _mover.CurrentIndex;
            Vector3 currentTangent = (_pathSystem.GetWaypoint(index + 1) - _pathSystem.GetWaypoint(index)).normalized;
            Vector3 nextTangent = (_pathSystem.GetWaypoint(index + 2) - _pathSystem.GetWaypoint(index + 1)).normalized;

            float segmentLength = _pathSystem.GetSegmentLength(index);
            float distanceRemaining = (1f - _mover.SegmentT) * segmentLength;
            float blendDistance = Mathf.Max(_config.LaneHalfWidth, 0.01f);
            float blendFactor = distanceRemaining >= blendDistance ? 0f : 1f - (distanceRemaining / blendDistance);

            return Vector3.Slerp(currentTangent, nextTangent, blendFactor).normalized;
        }
    }
}

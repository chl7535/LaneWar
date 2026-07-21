using UnityEngine;

namespace LaneWar.Data
{
    // 적의 자유 회피(분리) 이동에 필요한 통로 폭/회피 감지 반경/회피 강도/복귀 강도를 정의하는 ScriptableObject
    [CreateAssetMenu(fileName = "EnemyFlowConfig", menuName = "LaneWar/Enemy Flow Config")]
    public class EnemyFlowConfig : ScriptableObject
    {
        [Header("통로 폭 (경로 중심선 기준 좌우 허용 범위)")]
        [SerializeField] private float enemyWidth = 1f;
        [SerializeField] private float laneWidthInEnemyUnits = 4f;

        [Header("회피 (분리력)")]
        [SerializeField] private float avoidanceRadius = 1.5f;
        [SerializeField] private float avoidanceStrength = 6f;

        [Header("중심선 복귀 (통로 이탈 시)")]
        [SerializeField] private float centerlineReturnStrength = 8f;

        // 경로 중심선에서 좌우로 벗어날 수 있는 최대 거리 (통로 폭의 절반)
        public float LaneHalfWidth => enemyWidth * laneWidthInEnemyUnits * 0.5f;
        public float AvoidanceRadius => avoidanceRadius;
        public float AvoidanceStrength => avoidanceStrength;
        public float CenterlineReturnStrength => centerlineReturnStrength;
    }
}

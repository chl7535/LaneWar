using UnityEngine;

namespace LaneWar.Data
{
    // 적 1종의 고정 스탯(체력)을 정의하는 ScriptableObject
    [CreateAssetMenu(fileName = "EnemyData", menuName = "LaneWar/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private float maxHealth = 30f;
        [SerializeField] private int killReward = 5;

        public float MaxHealth => maxHealth;
        public int KillReward => killReward;
    }
}

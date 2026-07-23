using UnityEngine;

namespace LaneWar.Data
{
    // 적 1종의 고정 스탯(체력/방어력)을 정의하는 ScriptableObject. 라운드별 실제 적용치는 DifficultyConfig가 이 값에 배율을 곱해 계산한다
    [CreateAssetMenu(fileName = "EnemyData", menuName = "LaneWar/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private float maxHealth = 30f;
        [SerializeField] private float baseArmor = 5f;
        [SerializeField] private int killReward = 5;

        public float MaxHealth => maxHealth;
        public float BaseArmor => baseArmor;
        public int KillReward => killReward;
    }
}

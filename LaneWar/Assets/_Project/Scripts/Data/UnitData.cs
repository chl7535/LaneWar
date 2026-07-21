using UnityEngine;

namespace LaneWar.Data
{
    // 유닛 1종의 고정 스탯을 정의하는 ScriptableObject.
    // 등급(tier)은 자리만 마련해두고, 기초 5종은 전부 Tier 1이며 같은 등급끼리는 DPS(damage x attacksPerSecond)가 동일하다
    [CreateAssetMenu(fileName = "UnitData", menuName = "LaneWar/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField] private string unitName;
        [SerializeField] private int tier = 1;
        [SerializeField] private AttackStyle attackStyle;

        [Header("전투 스탯 (DPS = damage x attacksPerSecond)")]
        [SerializeField] private float range = 2f;
        [SerializeField] private float damage = 12f;
        [SerializeField] private float attacksPerSecond = 1f;

        [Header("경제 (다음 단계에서 사용)")]
        [SerializeField] private int cost = 100;

        [Header("표시 (아트 나오기 전 임시)")]
        [SerializeField] private Color displayColor = Color.white;
        [SerializeField] private GameObject unitPrefab;

        public string UnitName => unitName;
        public int Tier => tier;
        public AttackStyle AttackStyle => attackStyle;
        public float Range => range;
        public float Damage => damage;
        public float AttacksPerSecond => attacksPerSecond;
        public float Dps => damage * attacksPerSecond;
        public int Cost => cost;
        public Color DisplayColor => displayColor;
        public GameObject UnitPrefab => unitPrefab;
    }
}

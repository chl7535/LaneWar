using System.Collections.Generic;
using UnityEngine;

namespace LaneWar.Data
{
    // 뽑기 비용과 뽑기 대상 풀을 정의하는 ScriptableObject. 풀에 유닛을 추가하면 나중에 해금 유닛도 쉽게 등장시킬 수 있다
    [CreateAssetMenu(fileName = "GachaConfig", menuName = "LaneWar/Gacha Config")]
    public class GachaConfig : ScriptableObject
    {
        [SerializeField] private int gachaCost = 100;
        [SerializeField] private UnitData[] pool;

        public int GachaCost => gachaCost;
        public IReadOnlyList<UnitData> Pool => pool;
    }
}

using UnityEngine;

namespace LaneWar.Data
{
    // 시작 골드 등 경제 관련 초기값을 정의하는 ScriptableObject
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "LaneWar/Economy Config")]
    public class EconomyConfig : ScriptableObject
    {
        [SerializeField] private int startingGold = 300;

        public int StartingGold => startingGold;
    }
}

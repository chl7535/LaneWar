using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;

namespace LaneWar.Systems
{
    // CurrencyManager에서 뽑기 비용을 차감하고, 성공 시 GachaConfig 풀에서 뽑은 유닛을 대기열(UnitInventoryManager)에 추가한다
    public class GachaSystem : MonoBehaviour
    {
        [SerializeField] private GachaConfig gachaConfig;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private UnitInventoryManager inventoryManager;

        private GachaRoller _roller;

        public int Cost => gachaConfig.GachaCost;

        // 뽑기 성공 시 뽑힌 유닛을 방송한다
        public event System.Action<UnitData> GachaRolled;

        // 골드 부족 등으로 뽑기가 실패했을 때 방송한다
        public event System.Action GachaFailed;

        private void Awake()
        {
            _roller = new GachaRoller(gachaConfig);
        }

        public void Roll()
        {
            if (!currencyManager.TrySpendGold(gachaConfig.GachaCost))
            {
                GachaFailed?.Invoke();
                return;
            }

            UnitData result = _roller.RollUnit();
            inventoryManager.Add(result);
            GachaRolled?.Invoke(result);
        }
    }
}

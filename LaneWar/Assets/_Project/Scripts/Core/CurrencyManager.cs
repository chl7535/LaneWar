using UnityEngine;
using LaneWar.Data;

namespace LaneWar.Core
{
    // 씬에서 CurrencyState(골드)를 구동하고 EconomyConfig(시작 골드)를 연결하는 매니저
    public class CurrencyManager : MonoBehaviour
    {
        [SerializeField] private EconomyConfig economyConfig;

        private CurrencyState _currency;

        public event System.Action<int> GoldChanged;

        public int CurrentGold => _currency.CurrentGold;

        private void Awake()
        {
            _currency = new CurrencyState(economyConfig.StartingGold);
            _currency.GoldChanged += HandleGoldChanged;
        }

        public void AddGold(int amount)
        {
            _currency.Add(amount);
        }

        public bool TrySpendGold(int amount)
        {
            return _currency.TrySpend(amount);
        }

        private void HandleGoldChanged(int newAmount)
        {
            GoldChanged?.Invoke(newAmount);
        }
    }
}

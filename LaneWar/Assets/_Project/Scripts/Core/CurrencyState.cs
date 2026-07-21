using System;

namespace LaneWar.Core
{
    // 골드 증감을 담당하는 순수 로직. MonoBehaviour에 의존하지 않는다
    public class CurrencyState
    {
        public CurrencyState(int startingGold)
        {
            CurrentGold = startingGold;
        }

        public int CurrentGold { get; private set; }

        public event Action<int> GoldChanged;

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentGold += amount;
            GoldChanged?.Invoke(CurrentGold);
        }

        // 골드가 충분하면 차감하고 true를 반환한다. 부족하면 아무 변화 없이 false를 반환한다
        public bool TrySpend(int amount)
        {
            if (amount <= 0 || CurrentGold < amount)
            {
                return false;
            }

            CurrentGold -= amount;
            GoldChanged?.Invoke(CurrentGold);
            return true;
        }
    }
}

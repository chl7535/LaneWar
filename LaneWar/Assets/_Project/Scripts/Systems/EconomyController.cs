using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;

namespace LaneWar.Systems
{
    // 라운드 종료(RoundManager.RoundEnded) 시 RoundConfig에 정의된 정산 보너스를 CurrencyManager에 지급한다
    public class EconomyController : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private RoundConfig roundConfig;
        [SerializeField] private CurrencyManager currencyManager;

        private void OnEnable()
        {
            roundManager.RoundEnded += HandleRoundEnded;
        }

        private void OnDisable()
        {
            roundManager.RoundEnded -= HandleRoundEnded;
        }

        private void HandleRoundEnded(int roundNumber)
        {
            currencyManager.AddGold(roundConfig.GetRoundClearBonus(roundNumber));
        }
    }
}

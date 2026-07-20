using UnityEngine;
using LaneWar.Core;

namespace LaneWar.UI
{
    // 라운드/웨이브 상태를 화면에 표시하는 임시 디버그 HUD (OnGUI 기반, 추후 정식 UI로 교체 예정)
    public class RoundDebugHud : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;

        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            if (roundManager == null)
            {
                return;
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.fontSize = 20;
                _labelStyle.normal.textColor = Color.white;
            }

            string bossTag = roundManager.IsBossRound ? " (BOSS)" : "";
            string gameOverTag = roundManager.CurrentState == RoundState.GameOver ? "\nGAME OVER" : "";

            string text =
                $"Round {roundManager.CurrentRound}{bossTag}\n" +
                $"State: {roundManager.CurrentState}\n" +
                $"Time Left: {roundManager.TimeRemainingInState:0.0}s\n" +
                $"Enemies: {roundManager.AliveEnemyCount}/{roundManager.MaxAliveEnemies}" +
                gameOverTag;

            GUI.Label(new Rect(20, 20, 400, 160), text, _labelStyle);
        }
    }
}

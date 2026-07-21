using UnityEngine;
using LaneWar.Core;
using LaneWar.Systems;

namespace LaneWar.UI
{
    // 라운드/웨이브 상태 + 선택된 유닛 종류를 화면에 표시하는 임시 디버그 HUD (OnGUI 기반, 추후 정식 UI로 교체 예정)
    public class RoundDebugHud : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private UnitPlacementController placementController;

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

            string placementText = GetPlacementText();

            string text =
                $"Round {roundManager.CurrentRound}{bossTag}\n" +
                $"State: {roundManager.CurrentState}\n" +
                $"Time Left: {roundManager.TimeRemainingInState:0.0}s\n" +
                $"Enemies: {roundManager.AliveEnemyCount}/{roundManager.MaxAliveEnemies}" +
                gameOverTag +
                placementText;

            GUI.Label(new Rect(20, 20, 400, 200), text, _labelStyle);
        }

        private string GetPlacementText()
        {
            if (placementController == null)
            {
                return string.Empty;
            }

            string selectedUnitName = placementController.SelectedUnit != null
                ? placementController.SelectedUnit.Data.UnitName
                : "-";

            return $"\nSelected Unit: {selectedUnitName}";
        }
    }
}

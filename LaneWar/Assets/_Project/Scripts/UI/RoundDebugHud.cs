using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;
using LaneWar.Systems;

namespace LaneWar.UI
{
    // 라운드/웨이브 상태 + 선택된 유닛 종류 + 현재 라운드의 적 체력/방어력 스케일링 수치를 화면에 표시하는 임시 디버그 HUD (OnGUI 기반, 추후 정식 UI로 교체 예정)
    public class RoundDebugHud : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private UnitPlacementController placementController;
        [SerializeField] private DifficultyConfig difficultyConfig;
        [SerializeField] private EnemyData debugEnemyData;

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
            string difficultyText = GetDifficultyDebugText();

            string text =
                $"Round {roundManager.CurrentRound}{bossTag}\n" +
                $"State: {roundManager.CurrentState}\n" +
                $"Time Left: {roundManager.TimeRemainingInState:0.0}s\n" +
                $"Enemies: {roundManager.AliveEnemyCount}/{roundManager.MaxAliveEnemies}" +
                gameOverTag +
                placementText +
                difficultyText;

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

        // 현재 라운드에 스폰되는 적의 스케일링된 체력/방어력을 보여준다 (난이도 상승이 실제로 적용되는지 확인용)
        private string GetDifficultyDebugText()
        {
            if (difficultyConfig == null || debugEnemyData == null)
            {
                return string.Empty;
            }

            int round = roundManager.CurrentRound;
            float scaledHealth = debugEnemyData.MaxHealth * difficultyConfig.GetHealthMultiplier(round);
            float scaledArmor = debugEnemyData.BaseArmor * difficultyConfig.GetArmorMultiplier(round);

            return $"\nEnemy HP: {scaledHealth:0} / Armor: {scaledArmor:0}";
        }
    }
}

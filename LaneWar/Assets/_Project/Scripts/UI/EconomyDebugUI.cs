using System.Collections.Generic;
using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;
using LaneWar.Systems;

namespace LaneWar.UI
{
    // 골드 표시 + 뽑기 버튼 + 대기열(미배치 유닛) 목록을 보여주는 임시 디버그 UI (OnGUI 기반, 추후 정식 UI로 교체 예정).
    // 골드/대기열 표시는 폴링이 아니라 CurrencyManager/UnitInventoryManager의 이벤트 구독으로 갱신한다
    public class EconomyDebugUI : MonoBehaviour
    {
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private GachaSystem gachaSystem;
        [SerializeField] private UnitInventoryManager inventoryManager;
        [SerializeField] private UnitPlacementController placementController;

        private int _cachedGold;
        private IReadOnlyList<UnitData> _cachedItems = new List<UnitData>();
        private string _feedback = "";

        private void OnEnable()
        {
            currencyManager.GoldChanged += HandleGoldChanged;
            inventoryManager.InventoryChanged += HandleInventoryChanged;
            gachaSystem.GachaRolled += HandleGachaRolled;
            gachaSystem.GachaFailed += HandleGachaFailed;
        }

        // Start는 씬의 모든 오브젝트의 Awake가 끝난 뒤 호출되므로, 다른 오브젝트(Economy)의
        // Awake가 구성한 초기 상태를 여기서 안전하게 읽는다. OnEnable에서 읽으면 오브젝트 간
        // Awake/OnEnable 순서가 보장되지 않아 CurrencyManager._currency가 아직 null일 수 있다
        private void Start()
        {
            _cachedGold = currencyManager.CurrentGold;
            _cachedItems = inventoryManager.Items;
        }

        private void OnDisable()
        {
            currencyManager.GoldChanged -= HandleGoldChanged;
            inventoryManager.InventoryChanged -= HandleInventoryChanged;
            gachaSystem.GachaRolled -= HandleGachaRolled;
            gachaSystem.GachaFailed -= HandleGachaFailed;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 240, 260, 400));

            GUILayout.Label($"Gold: {_cachedGold}");

            if (GUILayout.Button($"뽑기 ({gachaSystem.Cost} G)"))
            {
                gachaSystem.Roll();
            }

            if (!string.IsNullOrEmpty(_feedback))
            {
                GUILayout.Label(_feedback);
            }

            GUILayout.Space(10f);
            GUILayout.Label("대기열 (클릭 후 맵 안쪽 클릭으로 배치):");

            for (int i = 0; i < _cachedItems.Count; i++)
            {
                UnitData data = _cachedItems[i];
                bool isPending = placementController.PendingUnitData == data;
                string label = (isPending ? "> " : "") + data.UnitName;

                if (GUILayout.Button(label))
                {
                    placementController.SelectQueuedUnit(data);
                }
            }

            GUILayout.EndArea();
        }

        private void HandleGoldChanged(int newGold)
        {
            _cachedGold = newGold;
        }

        private void HandleInventoryChanged()
        {
            _cachedItems = inventoryManager.Items;
        }

        private void HandleGachaRolled(UnitData data)
        {
            _feedback = $"{data.UnitName} 획득!";
        }

        private void HandleGachaFailed()
        {
            _feedback = "골드가 부족합니다.";
        }
    }
}

using System;
using System.Collections.Generic;
using LaneWar.Data;

namespace LaneWar.Core
{
    // 뽑았지만 아직 배치하지 않은 유닛 목록을 보관하는 순수 로직. MonoBehaviour에 의존하지 않는다
    public class UnitInventory
    {
        private readonly List<UnitData> _items = new List<UnitData>();

        public IReadOnlyList<UnitData> Items => _items;

        public event Action InventoryChanged;

        public void Add(UnitData unitData)
        {
            _items.Add(unitData);
            InventoryChanged?.Invoke();
        }

        // 대기열에서 해당 유닛 1개(첫 일치 항목)를 제거한다. 배치 시 호출한다. 존재하지 않으면 false
        public bool Remove(UnitData unitData)
        {
            if (!_items.Remove(unitData))
            {
                return false;
            }

            InventoryChanged?.Invoke();
            return true;
        }
    }
}

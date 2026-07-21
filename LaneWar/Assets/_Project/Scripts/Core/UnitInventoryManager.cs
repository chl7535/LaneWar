using System.Collections.Generic;
using UnityEngine;
using LaneWar.Data;

namespace LaneWar.Core
{
    // 씬에서 UnitInventory(대기열)를 구동하고 다른 컴포넌트에 참조를 제공하는 매니저
    public class UnitInventoryManager : MonoBehaviour
    {
        private UnitInventory _inventory;

        public event System.Action InventoryChanged;

        public IReadOnlyList<UnitData> Items => _inventory.Items;

        private void Awake()
        {
            _inventory = new UnitInventory();
            _inventory.InventoryChanged += HandleInventoryChanged;
        }

        public void Add(UnitData unitData)
        {
            _inventory.Add(unitData);
        }

        public bool Remove(UnitData unitData)
        {
            return _inventory.Remove(unitData);
        }

        private void HandleInventoryChanged()
        {
            InventoryChanged?.Invoke();
        }
    }
}

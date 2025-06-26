using System;
using System.Collections.Generic;
using System.Linq;

namespace Jang
{
    public class InventoryManager
    {
        public List<ItemInstance> ResourceList { get; private set; }
        public List<ItemInstance> WeaponList { get; private set; }
        public List<ItemInstance> GemList { get; private set; }

        public Dictionary<int, ItemInstance> EquippedWeaponDict { get; private set; }

        public event Action OnItemAdded;
        public event Action<int, ItemInstance> OnItemEquipped;
        public event Action<int, ItemInstance> OnItemUnEquipped;

        public InventoryManager()
        {
            ResourceList = new();
            WeaponList = new();
            GemList = new();
            EquippedWeaponDict = new();

            for (int i = 0; i < 10; i++)
            {
                EquippedWeaponDict[i] = null;
            }
        }

        public void AddItem(ItemInstance item, int amount = 1)
        {
            switch (item.Data.ItemType)
            {
                case ItemType.Weapon:
                    WeaponList.Add(item);
                    break;

                case ItemType.Gem:
                    AddOrMergeItem(GemList, item, amount);
                    break;

                case ItemType.Resource:
                    AddOrMergeItem(ResourceList, item, amount);
                    break;
            }

            OnItemAdded?.Invoke();
        }

        private void AddOrMergeItem(List<ItemInstance> list, ItemInstance newItem, int amount)
        {
            var existItem = list.Find(i => i.ItemKey == newItem.ItemKey);

            if (existItem != null)
            {
                existItem.AddItem(amount);
            }
            else
            {
                list.Add(newItem);
            }
        }

        public void UseItem(ItemInstance item)
        {
            if (item.Data.ItemType == ItemType.Weapon)
            {
                return;
            }

            item.UseItem();

            if (item.Quantity <= 0)
            {
                RemoveItem(item);
            }
        }

        public void RemoveItem(ItemInstance item)
        {
            switch (item.Data.ItemType)
            {
                case ItemType.Gem:
                    GemList.Remove(item);
                    break;

                case ItemType.Resource:
                    ResourceList.Remove(item);
                    break;
            }
        }

        public void EquipItem(ItemInstance item)
        {
            if (item.Data.ItemType != ItemType.Weapon)
                return;

            if (EquippedWeaponDict.ContainsValue(item))
                return;

            for (int i = 0; i < EquippedWeaponDict.Count; i++)
            {
                if (EquippedWeaponDict[i] == null)
                {
                    EquippedWeaponDict[i] = item;
                    OnItemEquipped?.Invoke(i, item);
                    return;
                }
            }

            // 빈 슬롯 없음
            UnityEngine.Debug.LogWarning("장착 가능한 슬롯이 없습니다.");
        }

        public void UnEquipItem(ItemInstance item)
        {
            foreach (var key in EquippedWeaponDict.Keys)
            {
                if (EquippedWeaponDict[key] == item)
                {
                    EquippedWeaponDict[key] = null;
                    OnItemUnEquipped?.Invoke(key, item);
                    return;
                }
            }
        }

        public List<ItemInstance> GetEquippedWeapons()
        {
            return EquippedWeaponDict.Values.ToList();
        }
    }
}

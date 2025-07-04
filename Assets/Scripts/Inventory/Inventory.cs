using System;
using System.Collections.Generic;
using System.Linq;

namespace Jang
{
    public class InventoryManager
    {
        private GameManager gameManager;

        public List<ItemInstance> ResourceList { get; private set; }
        public List<ItemInstance> WeaponList { get; private set; }
        public List<ItemInstance> GemList { get; private set; }

        public Dictionary<int, ItemInstance> EquippedWeaponDict { get; private set; }

        public event Action OnItemAdded;
        public event Action<int, ItemInstance> OnItemEquipped;
        public event Action<int, ItemInstance> OnItemUnEquipped;

        public InventoryManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            ResourceList = new();
            WeaponList = new();
            GemList = new();
            EquippedWeaponDict = new();

            for (int i = 0; i < 10; i++)
            {
                EquippedWeaponDict[i] = null;
            }
        }

        public void AddItem(ItemData item, int amount = 1)
        {
            switch (item.ItemType)
            {
                case ItemType.Weapon:
                    ItemInstance weapon = new ItemInstance(item.ItemKey, item, gameManager.DataManager.CraftingLoader.GetDataByKey(item.ItemKey));
                    WeaponList.Add(weapon);
                    break;

                case ItemType.Gem:
                    AddOrMergeItem(GemList, item, amount);
                    break;

                case ItemType.Resource:
                    AddOrMergeItem(ResourceList, item, amount);
                    break;

                case ItemType.Ingot: 
                    AddOrMergeItem(ResourceList, item, amount); 
                    break;
            }

            OnItemAdded?.Invoke();
        }

        private void AddOrMergeItem(List<ItemInstance> list, ItemData newItem, int amount)
        {
            var existItem = list.Find(i => i.ItemKey == newItem.ItemKey);

            if (existItem != null)
            {
                existItem.AddItem(amount);
            }
            else
            {
                ItemInstance item = new ItemInstance(newItem.ItemKey, newItem);
                item.Quantity = amount;

                list.Add(item);
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
                case ItemType.Weapon:
                    WeaponList.Remove(item);
                    break;
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
                    item.EquipItem();
                    OnItemEquipped?.Invoke(i, item);
                    return;
                }
            }

            // ���� ĭ ����
            UnityEngine.Debug.LogWarning("���� ������ ���� ĭ�� �����ϴ�.");
        }

        public void UnEquipItem(ItemInstance item)
        {
            foreach (var key in EquippedWeaponDict.Keys)
            {
                if (EquippedWeaponDict[key] == item)
                {
                    EquippedWeaponDict[key] = null;
                    item.UnEquipItem();
                    OnItemUnEquipped?.Invoke(key, item);
                    return;
                }
            }
        }

        public List<ItemInstance> GetEquippedWeapons()
        {
            return EquippedWeaponDict.Values.ToList();
        }

        // ���� ��� ���� ���� �Լ� �߰�
        public bool UseCraftingMaterials(List<(string resourceKey, int amount)> requiredList)
        {
            // 1. ��ü ��� ���� Ȯ��
            foreach (var req in requiredList)
            {
                int have = ResourceList.Where(x => x.ItemKey == req.resourceKey).Sum(x => x.Quantity);
                if (have < req.amount)
                    return false;
            }

            // 2. ������ ����
            foreach (var req in requiredList)
            {
                int remain = req.amount;
                // ���� ���ÿ��� ����
                foreach (var inst in ResourceList.Where(x => x.ItemKey == req.resourceKey && x.Quantity > 0).ToList())
                {
                    int consume = Math.Min(remain, inst.Quantity);
                    inst.Quantity -= consume;
                    remain -= consume;
                    if (inst.Quantity <= 0)
                        RemoveItem(inst);
                    if (remain <= 0) break;
                }
            }
            OnItemAdded?.Invoke();
            return true;
        }

        public List<ItemData> GetItemDataListByType(CustomerJob jobType)
        {
            List<ItemData> itemDatas = new List<ItemData>();

            foreach (var weapon in WeaponList)
            {
                if (weapon.CraftingData.jobType == jobType)
                {
                    itemDatas.Add(weapon.Data);
                }
            }

            return itemDatas;
        }
    }
}


using System;
using System.Collections.Generic;

namespace Jang
{
    public class InventoryManager
    {
        public List<ItemInstance> ResourceList { get; private set; }
        public List<ItemInstance> EquipmentList { get; private set; }
        public List<ItemInstance> GemList { get; private set; }

        public event Action onItemAdded;

        public InventoryManager()
        {
            ResourceList = new();
            EquipmentList = new();
            GemList = new();
        }

        public void AddItem(ItemInstance item, int amount = 1)
        {
            switch (item.Data.ItemType)
            {
                case ItemType.Equipment:
                    EquipmentList.Add(item);
                    break;

                case ItemType.Gem:
                    AddOrMergeItem(GemList, item, amount);
                    break;

                case ItemType.Resource:
                    AddOrMergeItem(ResourceList, item, amount);
                    break;
            }

            onItemAdded?.Invoke();
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
            if (item.Data.ItemType == ItemType.Equipment)
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
    }
}

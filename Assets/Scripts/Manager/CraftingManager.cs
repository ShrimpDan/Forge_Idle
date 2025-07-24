using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ���� ����/���� ���� ���� ����
public class CraftingManager : MonoBehaviour
{
    public const int CraftSlotCount = 6;
    
    public class CraftTask
    {
        public bool isCrafting = false;
        public float totalTime = 0f;
        public float timeLeft = 0f;
        public CraftingData data;
        public ItemData itemData;
        public bool rewardGiven = false;

        public void Reset()
        {
            isCrafting = false;
            totalTime = 0f;
            timeLeft = 0f;
            data = null;
            itemData = null;
            rewardGiven = false;
        }
    }

    private List<CraftTask> craftTasks = new List<CraftTask>();
    private List<Coroutine> coroutines = new List<Coroutine>();

    public event Action isCrafingDone;
 
    private InventoryManager inventory;
    private Forge forge;

    void Awake()
    {
        for (int i = 0; i < CraftSlotCount; i++)
        {
            craftTasks.Add(new CraftTask());
            coroutines.Add(null);
        }
    }

    public void Init(InventoryManager inventory, Forge forge)
    {
        this.inventory = inventory;
        this.forge = forge;
    }

    public void StartCrafting(int slotIndex, CraftingData data, ItemData itemData)
    {
        if (slotIndex < 0 || slotIndex >= CraftSlotCount) return;
        var task = craftTasks[slotIndex];
        if (task.isCrafting) return;

        task.isCrafting = true;
        task.totalTime = data.craftTime;
        task.timeLeft = data.craftTime;
        task.data = data;
        task.itemData = itemData;
        task.rewardGiven = false;

        if (coroutines[slotIndex] != null)
            StopCoroutine(coroutines[slotIndex]);
        coroutines[slotIndex] = StartCoroutine(CraftingCoroutine(slotIndex));
    }

    private IEnumerator CraftingCoroutine(int idx)
    {
        var task = craftTasks[idx];
        while (task.timeLeft > 0f)
        {
            yield return null;
            task.timeLeft -= Time.deltaTime;
        }
        task.timeLeft = 0f;
        task.isCrafting = false;
        if (!task.rewardGiven && task.itemData != null)
        {
            inventory.AddItem(task.itemData, 1);
            task.rewardGiven = true;
            isCrafingDone?.Invoke(); //여기서 이벤트 발생
            GameManager.Instance.DailyQuestManager.ProgressQuest("MakeWeapon", 1);
        }
        
    }

    public CraftTask GetCraftTask(int idx)
    {
        if (idx < 0 || idx >= CraftSlotCount) return null;
        return craftTasks[idx];
    }

    public void ResetAllTasks()
    {
        for (int i = 0; i < CraftSlotCount; i++)
        {
            if (coroutines[i] != null)
                StopCoroutine(coroutines[i]);
            craftTasks[i].Reset();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UpgradeWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("Gold Icon")]
    [SerializeField] private Sprite goldIconSprite;

    [Header("Window Settings")]
    [SerializeField] private Button exitButton;

    [Header("Tab")]
    [SerializeField] private Button upgradeTabBtn;
    [SerializeField] private Button gemTabBtn;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject gemSystemPanel;

    [Header("LackPopup")]
    [SerializeField] private LackPopup lackPopupPrefab;
    [SerializeField] private Transform popupParent;

    [Header("Upgrade UI")]
    [SerializeField] private Button inputWeaponSlotBtn;
    [SerializeField] private Image inputWeaponIcon;
    [SerializeField] private TMP_Text inputWeaponName;
    [SerializeField] private Transform currentUpgradeRoot;
    [SerializeField] private GameObject starFilledPrefab;
    [SerializeField] private GameObject starEmptyPrefab;
    [SerializeField] private Transform inputItemRoot;
    [SerializeField] private GameObject inputItemPrefab;
    [SerializeField] private TMP_Text successRateText;
    [SerializeField] private TMP_Text beforeText;
    [SerializeField] private TMP_Text afterText;
    [SerializeField] private Image progressBar;
    [SerializeField] private Button upgradeButton;

    [Header("Advanced Upgrade UI")]
    [SerializeField] private Button advancedUpgradeButton;

    [Header("Automatic Upgrade UI")]
    [SerializeField] private Button automaticUpgradeButton;
    [SerializeField] private GameObject automaticUpgradePanel;
    [SerializeField] private Button[] automaticUpgradeLevelButtons;

    private readonly int[] autoUpgradeTargetLevels = { 1, 5, 10, 13, 15 };
    private Coroutine autoUpgradeCoroutine;

    [Header("Gem System UI")]
    [SerializeField] private Button inputWeaponSlotBtnGem;
    [SerializeField] private Image inputWeaponIconGem;
    [SerializeField] private TMP_Text inputWeaponNameGem;
    [SerializeField] private Transform[] gemSlots;
    [SerializeField] private Image[] gemSlotIcons;
    [SerializeField] private TMP_Text gemBeforeText;
    [SerializeField] private TMP_Text gemAfterText;

    private ItemInstance selectedWeapon;
    private Coroutine upgradeCoroutine;
    private ItemInstance selectedGemWeapon;
    private ItemInstance[] equippedGems = new ItemInstance[4];

    private DataManager dataManager;
    private ForgeManager forgeManager;

    private int CalcEnhanceGoldCost(ItemInstance weapon)
    {
        int level = weapon.CurrentEnhanceLevel + 1;
        float baseAtk = weapon.Data?.WeaponStats?.Attack ?? 1;
        int cost = Mathf.RoundToInt(1000 * level * level * level);
        return Mathf.Max(cost, 1000);
    }

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        dataManager = gameManager.DataManager;
        forgeManager = gameManager.ForgeManager;

        // 종료
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.UpgradeWeaponWindow));

        // 탭
        upgradeTabBtn.onClick.RemoveAllListeners();
        upgradeTabBtn.onClick.AddListener(ShowUpgradePanel);
        gemTabBtn.onClick.RemoveAllListeners();
        gemTabBtn.onClick.AddListener(ShowGemPanel);

        // 무기선택(강화)
        inputWeaponSlotBtn.onClick.RemoveAllListeners();
        inputWeaponSlotBtn.onClick.AddListener(OpenWeaponInventory);

        // 강화
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(StartUpgrade);

        // --- 고급강화 이벤트 연결 ---
        if (advancedUpgradeButton != null)
        {
            advancedUpgradeButton.onClick.RemoveAllListeners();
            advancedUpgradeButton.onClick.AddListener(StartAdvancedUpgrade);
        }

        // 자동강화
        if (automaticUpgradeButton != null)
        {
            automaticUpgradeButton.onClick.RemoveAllListeners();
            automaticUpgradeButton.onClick.AddListener(ShowAutomaticUpgradePanel);
        }
        if (automaticUpgradeLevelButtons != null && automaticUpgradeLevelButtons.Length == autoUpgradeTargetLevels.Length)
        {
            for (int i = 0; i < automaticUpgradeLevelButtons.Length; i++)
            {
                int idx = i;
                automaticUpgradeLevelButtons[i].onClick.RemoveAllListeners();
                automaticUpgradeLevelButtons[i].onClick.AddListener(() => StartAutomaticUpgrade(autoUpgradeTargetLevels[idx]));
            }
        }
        if (automaticUpgradePanel != null)
            automaticUpgradePanel.SetActive(false);

        // 무기선택(젬)
        inputWeaponSlotBtnGem.onClick.RemoveAllListeners();
        inputWeaponSlotBtnGem.onClick.AddListener(OpenGemWeaponPopup);

        // 젬 슬롯 클릭 연결
        for (int i = 0; i < gemSlots.Length; i++)
        {
            int idx = i;
            var btn = gemSlots[i].GetComponent<Button>();
            if (btn == null) btn = gemSlots[i].gameObject.AddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnGemSlotClicked(idx));
        }

        ShowUpgradePanel();
        ResetUpgradePanel();
        ResetGemSystemPanel();
    }

    //--- 강화 탭 ---
    private void ShowUpgradePanel()
    {
        upgradePanel.SetActive(true);
        gemSystemPanel.SetActive(false);
        ResetUpgradePanel();
        HideAutomaticUpgradePanel();
    }

    //--- 젬 탭 ---
    private void ShowGemPanel()
    {
        upgradePanel.SetActive(false);
        gemSystemPanel.SetActive(true);
        HideAutomaticUpgradePanel();
        if (GameManager.Instance.TutorialManager != null)
        {
            GameManager.Instance.TutorialManager.ForceStepClear();
        }
        ResetGemSystemPanel();
    }

    //--- 무기 선택 ---
    private void OpenWeaponInventory()
    {
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        if (selectedWeapon != null && selectedWeapon.Data == null)
            selectedWeapon.Data = dataManager.ItemLoader.GetItemByKey(selectedWeapon.ItemKey);
        RefreshUpgradePanel();
        RefreshAutomaticUpgradeButtons();
    }

    private void ResetUpgradePanel()
    {
        selectedWeapon = null;
        inputWeaponIcon.enabled = false;
        inputWeaponName.text = "";
        beforeText.text = "-";
        afterText.text = "-";
        successRateText.text = "-";
        progressBar.fillAmount = 0;
        foreach (Transform child in currentUpgradeRoot) Destroy(child.gameObject);
        foreach (Transform child in inputItemRoot) Destroy(child.gameObject);
        HideAutomaticUpgradePanel();
    }

    private void RefreshUpgradePanel()
    {
        if (selectedWeapon == null || selectedWeapon.Data == null)
        {
            ResetUpgradePanel();
            return;
        }
        inputWeaponIcon.sprite = IconLoader.GetIconByKey(selectedWeapon.Data.ItemKey);
        inputWeaponIcon.enabled = true;
        inputWeaponName.text = selectedWeapon.Data.Name;

        int curLevel = selectedWeapon.CurrentEnhanceLevel;
        int maxLevel = selectedWeapon.Data.UpgradeInfo.MaxEnhanceLevel;
        foreach (Transform child in currentUpgradeRoot) Destroy(child.gameObject);
        for (int i = 0; i < maxLevel; i++)
            Instantiate(i < curLevel ? starFilledPrefab : starEmptyPrefab, currentUpgradeRoot);

        foreach (Transform child in inputItemRoot) Destroy(child.gameObject);
        int needGold = CalcEnhanceGoldCost(selectedWeapon);
        var goldGo = Instantiate(inputItemPrefab, inputItemRoot);
        TMP_Text amountText = goldGo.transform.Find("Amount_Text")?.GetComponent<TMP_Text>();
        Image icon = goldGo.transform.Find("Icon")?.GetComponent<Image>();
        if (amountText != null) amountText.text = needGold.ToString("N0");
        if (icon != null) icon.sprite = goldIconSprite;
        goldGo.SetActive(true);

        int rate = CalcEnhanceSuccessRate(selectedWeapon);
        successRateText.text = $"{rate}%";
        float beforeAtk = selectedWeapon.GetTotalAttack();
        float afterAtk = CalcNextAttack(selectedWeapon);
        beforeText.text = $"{beforeAtk:F0}";
        afterText.text = $"{afterAtk:F0}";
        progressBar.fillAmount = 0;

        RefreshAutomaticUpgradeButtons();
    }

    private int CalcEnhanceSuccessRate(ItemInstance weapon)
    {
        if (weapon == null || weapon.Data == null) return 0;
        int cur = weapon.CurrentEnhanceLevel;
        int max = weapon.Data.UpgradeInfo.MaxEnhanceLevel;
        if (cur >= max) return 0;
        return Mathf.Max(60, 100 - (cur * 10));
    }

    private int CalcAdvancedEnhanceSuccessRate(ItemInstance weapon)
    {
        return CalcEnhanceSuccessRate(weapon) + 10;
    }

    private float CalcNextAttack(ItemInstance weapon)
    {
        if (weapon == null || weapon.Data == null) return 0;
        int nextLv = weapon.CurrentEnhanceLevel + 1;
        if (nextLv > weapon.Data.UpgradeInfo.MaxEnhanceLevel) nextLv = weapon.Data.UpgradeInfo.MaxEnhanceLevel;
        float baseAtk = weapon.Data.WeaponStats.Attack;
        float mul = weapon.Data.UpgradeInfo.AttackMultiplier;
        return (nextLv == 0) ? baseAtk : baseAtk * (nextLv * mul);
    }

    //--- 일반강화 ---
    private void StartUpgrade()
    {
        if (selectedWeapon == null || selectedWeapon.Data == null) return;
        int goldCost = CalcEnhanceGoldCost(selectedWeapon);

        if (!forgeManager.UseGold(goldCost))
        {
            if (lackPopupPrefab != null)
            {
                LackPopup popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
                popup.Init(GameManager.Instance, uIManager);
                popup.Show(LackType.Gold);
            }
            return;
        }

        if (upgradeCoroutine != null) StopCoroutine(upgradeCoroutine);
        upgradeCoroutine = StartCoroutine(UpgradeRoutine(false));
    }

    //--- 고급강화 ---
    private void StartAdvancedUpgrade()
    {
        if (selectedWeapon == null || selectedWeapon.Data == null) return;

        int goldCost = CalcEnhanceGoldCost(selectedWeapon);

        if (!forgeManager.UseGold(goldCost))
        {
            if (lackPopupPrefab != null)
            {
                LackPopup popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
                popup.Init(GameManager.Instance, uIManager);
                popup.Show(LackType.Gold);
            }
            return;
        }

        if (!forgeManager.UseDia(200))
        {
            if (lackPopupPrefab != null)
            {
                LackPopup popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
                popup.Init(GameManager.Instance, uIManager);
                popup.Show(LackType.Dia);
            }
            return;
        }
        if (upgradeCoroutine != null) StopCoroutine(upgradeCoroutine);
        upgradeCoroutine = StartCoroutine(UpgradeRoutine(true));
    }

    private IEnumerator UpgradeRoutine(bool isAdvanced)
    {
        upgradeButton.interactable = false;
        if (advancedUpgradeButton != null) advancedUpgradeButton.interactable = false;
        if (automaticUpgradeButton != null) automaticUpgradeButton.interactable = false;

        float duration = 2.0f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            progressBar.fillAmount = Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }

        int successRate = isAdvanced ? CalcAdvancedEnhanceSuccessRate(selectedWeapon) : CalcEnhanceSuccessRate(selectedWeapon);
        bool isSuccess = UnityEngine.Random.Range(0, 100) < successRate;
        if (isSuccess)
        {
            progressBar.fillAmount = 1f;
            selectedWeapon.EnhanceItem();
            SoundManager.Instance.Play("Successound");
        }
        else
        {
            float failTime = 0.5f;
            float failTimer = 0f;
            float start = progressBar.fillAmount;
            while (failTimer < failTime)
            {
                failTimer += Time.deltaTime;
                float t = Mathf.Clamp01(failTimer / failTime);
                progressBar.fillAmount = Mathf.Lerp(start, 0, t);
                yield return null;
            }
            SoundManager.Instance.Play("FailSound");
        }
        RefreshUpgradePanel();
        upgradeButton.interactable = true;
        if (advancedUpgradeButton != null) advancedUpgradeButton.interactable = true;
        if (automaticUpgradeButton != null) automaticUpgradeButton.interactable = true;
    }

    // ===== 자동강화 =======
    private void ShowAutomaticUpgradePanel()
    {
        if (selectedWeapon == null || selectedWeapon.Data == null) return;
        if (automaticUpgradePanel != null) automaticUpgradePanel.SetActive(true);
        RefreshAutomaticUpgradeButtons();
    }
    private void HideAutomaticUpgradePanel()
    {
        if (automaticUpgradePanel != null) automaticUpgradePanel.SetActive(false);
    }

    private void RefreshAutomaticUpgradeButtons()
    {
        if (automaticUpgradeLevelButtons == null || automaticUpgradeLevelButtons.Length != autoUpgradeTargetLevels.Length) return;
        int maxLevel = selectedWeapon?.Data?.UpgradeInfo.MaxEnhanceLevel ?? 0;
        for (int i = 0; i < autoUpgradeTargetLevels.Length; i++)
        {
            bool valid = autoUpgradeTargetLevels[i] <= maxLevel;
            automaticUpgradeLevelButtons[i].interactable = valid;
            TMP_Text btnText = automaticUpgradeLevelButtons[i].GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.color = valid ? Color.white : Color.gray;
        }
    }

    private void StartAutomaticUpgrade(int targetLevel)
    {
        if (selectedWeapon == null || selectedWeapon.Data == null) return;
        if (targetLevel > selectedWeapon.Data.UpgradeInfo.MaxEnhanceLevel) return;

        if (autoUpgradeCoroutine != null) StopCoroutine(autoUpgradeCoroutine);
        autoUpgradeCoroutine = StartCoroutine(AutoUpgradeRoutine(targetLevel));
    }

    private IEnumerator AutoUpgradeRoutine(int targetLevel)
    {
        if (upgradeButton != null) upgradeButton.interactable = false;
        if (advancedUpgradeButton != null) advancedUpgradeButton.interactable = false;
        if (automaticUpgradeButton != null) automaticUpgradeButton.interactable = false;
        if (automaticUpgradeLevelButtons != null)
            foreach (var btn in automaticUpgradeLevelButtons)
                btn.interactable = false;

        while (selectedWeapon != null &&
               selectedWeapon.Data != null &&
               selectedWeapon.CurrentEnhanceLevel < targetLevel &&
               selectedWeapon.CanEnhance)
        {
            int goldCost = CalcEnhanceGoldCost(selectedWeapon);
            if (!forgeManager.UseGold(goldCost))
            {
                if (lackPopupPrefab != null)
                {
                    LackPopup popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
                    popup.Init(GameManager.Instance, uIManager);
                    popup.Show(LackType.Gold);
                }
                break;
            }

            yield return UpgradeRoutine(false);
            yield return new WaitForSeconds(0.15f);
        }

        if (upgradeButton != null) upgradeButton.interactable = true;
        if (advancedUpgradeButton != null) advancedUpgradeButton.interactable = true;
        if (automaticUpgradeButton != null) automaticUpgradeButton.interactable = true;
        RefreshAutomaticUpgradeButtons();
        HideAutomaticUpgradePanel();
        autoUpgradeCoroutine = null;
    }

    // =========== 젬 시스템 ===========
    private void OpenGemWeaponPopup()
    {
        var popup = uIManager.OpenUI<Gem_Weapon_Popup>(UIName.Gem_Weapon_Popup);
        popup.SetWeaponSelectCallback(OnGemWeaponSelected);
    }

    private void OnGemWeaponSelected(ItemInstance weapon)
    {
        selectedGemWeapon = weapon;
        if (selectedGemWeapon != null && selectedGemWeapon.Data == null)
            selectedGemWeapon.Data = dataManager.ItemLoader.GetItemByKey(selectedGemWeapon.ItemKey);

        if (selectedGemWeapon.GemSockets == null || selectedGemWeapon.GemSockets.Count < 4)
        {
            selectedGemWeapon.GemSockets = new List<ItemInstance>() { null, null, null, null };
        }
        for (int i = 0; i < equippedGems.Length; i++)
            equippedGems[i] = selectedGemWeapon.GemSockets.Count > i ? selectedGemWeapon.GemSockets[i] : null;

        RefreshGemSystemPanel();
    }

    private void ResetGemSystemPanel()
    {
        selectedGemWeapon = null;
        for (int i = 0; i < equippedGems.Length; i++)
            equippedGems[i] = null;

        inputWeaponIconGem.enabled = false;
        inputWeaponNameGem.text = "";

        for (int i = 0; i < gemSlots.Length; i++)
        {
            if (gemSlots[i] != null)
                gemSlots[i].gameObject.SetActive(true);

            if (gemSlotIcons != null && gemSlotIcons.Length > i && gemSlotIcons[i] != null)
            {
                gemSlotIcons[i].sprite = null;
                gemSlotIcons[i].enabled = false;
            }
        }
        gemBeforeText.text = "-";
        gemAfterText.text = "-";
    }

    private void RefreshGemSystemPanel()
    {
        if (selectedGemWeapon == null || selectedGemWeapon.Data == null)
        {
            ResetGemSystemPanel();
            return;
        }

        inputWeaponIconGem.sprite = IconLoader.GetIconByKey(selectedGemWeapon.Data.ItemKey);
        inputWeaponIconGem.enabled = true;
        inputWeaponNameGem.text = selectedGemWeapon.Data.Name;

        for (int i = 0; i < gemSlots.Length; i++)
        {
            var gem = equippedGems[i];
            if (gemSlotIcons != null && gemSlotIcons.Length > i && gemSlotIcons[i] != null)
            {
                if (gem != null && gem.Data != null)
                {
                    gemSlotIcons[i].sprite = IconLoader.GetIcon(ItemType.Gem, gem.Data.ItemKey);
                    gemSlotIcons[i].enabled = true;
                }
                else
                {
                    gemSlotIcons[i].sprite = null;
                    gemSlotIcons[i].enabled = false;
                }
            }
        }

        CalcGemPreviewStats();
    }

    private void OnGemSlotClicked(int idx)
    {
        if (selectedGemWeapon == null) return;

        if (equippedGems[idx] == null)
        {
            var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
            popup.SetGemSelectCallback((gem) => OnGemSelected(idx, gem));
        }
        else
        {
            var gem = equippedGems[idx];
            if (gem != null)
            {
                gem.Quantity += 1;
                if (!GameManager.Instance.Inventory.GemList.Contains(gem))
                    GameManager.Instance.Inventory.GemList.Add(gem);
            }
            equippedGems[idx] = null;
            selectedGemWeapon.GemSockets[idx] = null;
            SoundManager.Instance.Play("SFX_UIUnequip");
            RefreshGemSystemPanel();

            InventorySaveSystem.SaveInventory(GameManager.Instance.Inventory);
        }
    }

    private void OnGemSelected(int idx, ItemInstance gem)
    {
        if (gem == null) return;
        GameManager.Instance.Inventory.UseItem(gem);
        equippedGems[idx] = gem;
        selectedGemWeapon.GemSockets[idx] = gem;
        SoundManager.Instance.Play("SFX_UIEquip");
        RefreshGemSystemPanel();

        InventorySaveSystem.SaveInventory(GameManager.Instance.Inventory);
    }

    private void CalcGemPreviewStats()
    {
        if (selectedGemWeapon == null || selectedGemWeapon.Data == null)
        {
            gemBeforeText.text = "-";
            gemAfterText.text = "-";
            return;
        }
        var bak = new List<ItemInstance>(selectedGemWeapon.GemSockets);
        selectedGemWeapon.GemSockets = new List<ItemInstance>() { null, null, null, null };
        float before = selectedGemWeapon.GetTotalAttack();
        selectedGemWeapon.GemSockets = new List<ItemInstance>(equippedGems);
        float after = selectedGemWeapon.GetTotalAttack();

        selectedGemWeapon.GemSockets = bak;
        gemBeforeText.text = $"{before:F0}";
        gemAfterText.text = $"{after:F0}";
    }
}

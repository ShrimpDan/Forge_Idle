using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UpgradeWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("Window Settings")]
    [SerializeField] private Button exitButton;

    [Header("Tab")]
    [SerializeField] private Button upgradeTabBtn;
    [SerializeField] private Button gemTabBtn;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject gemSystemPanel;

    // --- 강화 UI ---
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

    // --- 젬 시스템 UI ---
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
    private UIManager uiManager;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.uiManager = uiManager;
        dataManager = gameManager.DataManager;

        // 종료
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uiManager.CloseUI(UIName.UpgradeWeaponWindow));

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

        // 초기 상태
        ShowUpgradePanel();
        ResetUpgradePanel();
        ResetGemSystemPanel();
    }

    //강화 탭
    private void ShowUpgradePanel()
    {
        upgradePanel.SetActive(true);
        gemSystemPanel.SetActive(false);
        ResetUpgradePanel();
    }

    //젬 탭
    private void ShowGemPanel()
    {
        upgradePanel.SetActive(false);
        gemSystemPanel.SetActive(true);
        ResetGemSystemPanel();
    }

    //강화 시스템
    private void OpenWeaponInventory()
    {
        var popup = uiManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        if (selectedWeapon != null && selectedWeapon.Data == null)
            selectedWeapon.Data = dataManager.ItemLoader.GetItemByKey(selectedWeapon.ItemKey);
        RefreshUpgradePanel();
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
    }

    private void RefreshUpgradePanel()
    {
        if (selectedWeapon == null || selectedWeapon.Data == null)
        {
            ResetUpgradePanel();
            return;
        }
        inputWeaponIcon.sprite = IconLoader.GetIconByPath(selectedWeapon.Data.IconPath);
        inputWeaponIcon.enabled = true;
        inputWeaponName.text = selectedWeapon.Data.Name;

        // 별 UI
        int curLevel = selectedWeapon.CurrentEnhanceLevel;
        int maxLevel = selectedWeapon.Data.UpgradeInfo.MaxEnhanceLevel;
        foreach (Transform child in currentUpgradeRoot) Destroy(child.gameObject);
        for (int i = 0; i < maxLevel; i++)
            Instantiate(i < curLevel ? starFilledPrefab : starEmptyPrefab, currentUpgradeRoot);

        foreach (Transform child in inputItemRoot) Destroy(child.gameObject);

        int rate = CalcEnhanceSuccessRate(selectedWeapon);
        successRateText.text = $"{rate}%";

        float beforeAtk = selectedWeapon.GetTotalAttack();
        float afterAtk = CalcNextAttack(selectedWeapon);
        beforeText.text = $"{beforeAtk:F0}";
        afterText.text = $"{afterAtk:F0}";
        progressBar.fillAmount = 0;
    }

    private int CalcEnhanceSuccessRate(ItemInstance weapon)
    {
        if (weapon == null || weapon.Data == null) return 0;
        int cur = weapon.CurrentEnhanceLevel;
        int max = weapon.Data.UpgradeInfo.MaxEnhanceLevel;
        if (cur >= max) return 0;
        return Mathf.Max(60, 100 - (cur * 10));
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

    private void StartUpgrade()
    {
        if (selectedWeapon == null || selectedWeapon.Data == null) return;
        if (upgradeCoroutine != null) StopCoroutine(upgradeCoroutine);
        upgradeCoroutine = StartCoroutine(UpgradeRoutine());
    }

    private IEnumerator UpgradeRoutine()
    {
        upgradeButton.interactable = false;
        float duration = 2.0f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            progressBar.fillAmount = Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }
        int successRate = CalcEnhanceSuccessRate(selectedWeapon);
        bool isSuccess = Random.Range(0, 100) < successRate;
        if (isSuccess)
        {
            progressBar.fillAmount = 1f;
            selectedWeapon.EnhanceItem();
            // TODO: 성공 애니메이션
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
        }
        RefreshUpgradePanel();
        upgradeButton.interactable = true;
    }

    //젬 시스템
    private void OpenGemWeaponPopup()
    {
        var popup = uiManager.OpenUI<Gem_Weapon_Popup>(UIName.Gem_Weapon_Popup);
        popup.SetWeaponSelectCallback(OnGemWeaponSelected);
    }

    private void OnGemWeaponSelected(ItemInstance weapon)
    {
        selectedGemWeapon = weapon;
        if (selectedGemWeapon != null && selectedGemWeapon.Data == null)
            selectedGemWeapon.Data = dataManager.ItemLoader.GetItemByKey(selectedGemWeapon.ItemKey);

        // 젬 소켓 세팅
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

        inputWeaponIconGem.sprite = IconLoader.GetIconByPath(selectedGemWeapon.Data.IconPath);
        inputWeaponIconGem.enabled = true;
        inputWeaponNameGem.text = selectedGemWeapon.Data.Name;

        // 젬 슬롯 UI (직접 등록한 아이콘 배열로)
        for (int i = 0; i < gemSlots.Length; i++)
        {
            var gem = equippedGems[i];
            if (gemSlotIcons != null && gemSlotIcons.Length > i && gemSlotIcons[i] != null)
            {
                if (gem != null && gem.Data != null)
                {
                    gemSlotIcons[i].sprite = IconLoader.GetIconByPath(gem.Data.IconPath);
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
            var popup = uiManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
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

    // 미리보기 (Before: 젬 적용X / After: 젬 적용O)
    private void CalcGemPreviewStats()
    {
        if (selectedGemWeapon == null || selectedGemWeapon.Data == null)
        {
            gemBeforeText.text = "-";
            gemAfterText.text = "-";
            return;
        }
        // Before: 젬 모두 제외
        var bak = new List<ItemInstance>(selectedGemWeapon.GemSockets);
        selectedGemWeapon.GemSockets = new List<ItemInstance>() { null, null, null, null };
        float before = selectedGemWeapon.GetTotalAttack();
        // After: 젬 반영
        selectedGemWeapon.GemSockets = new List<ItemInstance>(equippedGems);
        float after = selectedGemWeapon.GetTotalAttack();

        selectedGemWeapon.GemSockets = bak;
        gemBeforeText.text = $"{before:F0}";
        gemAfterText.text = $"{after:F0}";
    }
}

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

    private ItemInstance selectedWeapon;
    private DataManager dataManager;
    private Coroutine upgradeCoroutine;
    private UIManager uiManager;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.uiManager = uiManager;
        dataManager = gameManager.DataManager;

        // 나가기
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uiManager.CloseUI(UIName.UpgradeWeaponWindow));

        // 탭 버튼 세팅
        upgradeTabBtn.onClick.RemoveAllListeners();
        upgradeTabBtn.onClick.AddListener(ShowUpgradePanel);
        gemTabBtn.onClick.RemoveAllListeners();
        gemTabBtn.onClick.AddListener(ShowGemPanel);

        // 무기선택
        inputWeaponSlotBtn.onClick.RemoveAllListeners();
        inputWeaponSlotBtn.onClick.AddListener(OpenWeaponInventory);

        // 강화 버튼
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(StartUpgrade);

        // 초기상태
        ShowUpgradePanel();
        ResetUpgradePanel();
    }

    private void ShowUpgradePanel()
    {
        upgradePanel.SetActive(true);
        gemSystemPanel.SetActive(false);
    }

    private void ShowGemPanel()
    {
        upgradePanel.SetActive(false);
        gemSystemPanel.SetActive(true);
    }

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

        // 무기 아이콘/이름
        inputWeaponIcon.sprite = IconLoader.GetIcon(selectedWeapon.Data.IconPath);
        inputWeaponIcon.enabled = true;
        inputWeaponName.text = selectedWeapon.Data.Name;

        // 별 UI(강화 등급)
        int curLevel = selectedWeapon.CurrentEnhanceLevel;
        int maxLevel = selectedWeapon.Data.UpgradeInfo.MaxEnhanceLevel;
        foreach (Transform child in currentUpgradeRoot) Destroy(child.gameObject);
        for (int i = 0; i < maxLevel; i++)
        {
            var star = Instantiate(i < curLevel ? starFilledPrefab : starEmptyPrefab, currentUpgradeRoot);
        }

        // 재화
        foreach (Transform child in inputItemRoot) Destroy(child.gameObject);
        // TODO: 강화에 필요한 재화 표시 구현 예정

        // 성공률(예시값)
        int rate = CalcEnhanceSuccessRate(selectedWeapon);
        successRateText.text = $"{rate}%";

        // Before/After 스탯
        float beforeAtk = selectedWeapon.GetTotalAttack();
        float afterAtk = CalcNextAttack(selectedWeapon);
        beforeText.text = $"{beforeAtk:F0}";
        afterText.text = $"{afterAtk:F0}";

        progressBar.fillAmount = 0;
    }

    // 예시 성공률 계산 (추후 재화, 장비 등 영향 반영)
    private int CalcEnhanceSuccessRate(ItemInstance weapon)
    {
        if (weapon == null || weapon.Data == null) return 0;
        int cur = weapon.CurrentEnhanceLevel;
        int max = weapon.Data.UpgradeInfo.MaxEnhanceLevel;
        if (cur >= max) return 0;
        // 예: 100%~60% 점감 (연출용, 실제 확률 로직은 추후 커스텀)
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
        float[] fillStages = { 0f, 0.5f, 0.8f, 1.0f };
        int stage = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            // 중간 fill 단계를 연출 (0 > 0.5 > 0.8 > 1.0)
            if (t > 0.8f && stage < 3) { stage = 3; }
            else if (t > 0.5f && stage < 2) { stage = 2; }
            else if (t > 0.2f && stage < 1) { stage = 1; }
            progressBar.fillAmount = Mathf.Lerp(fillStages[stage], fillStages[Mathf.Min(stage + 1, 3)], (t * 4) % 1f);
            yield return null;
        }

        // 성공/실패 판정
        int successRate = CalcEnhanceSuccessRate(selectedWeapon);
        bool isSuccess = Random.Range(0, 100) < successRate;

        if (isSuccess)
        {
            progressBar.fillAmount = 1f;
            selectedWeapon.EnhanceItem();
            // 강화 성공 효과 등 추가 가능
        }
        else
        {
            // 실패 시 fill이 0.8에서 0으로 감소
            float failTime = 0.5f;
            float start = progressBar.fillAmount;
            float t = 0;
            while (t < failTime)
            {
                t += Time.deltaTime;
                progressBar.fillAmount = Mathf.Lerp(start, 0, t / failTime);
                yield return null;
            }
        }

        RefreshUpgradePanel();
        upgradeButton.interactable = true;
    }
}

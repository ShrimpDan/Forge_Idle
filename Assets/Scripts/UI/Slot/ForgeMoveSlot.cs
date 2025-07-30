using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeMoveSlot : MonoBehaviour
{
    [SerializeField] ForgeType forgeType;
    [SerializeField] GameObject lockedIndicator;
    [SerializeField] TextMeshProUGUI lockedText;
    [SerializeField] Button moveBtn;

    private const string CurrentForge = "현재 대장간";
    private const string NeedToUpgrade = "이전 대장간\n[업그레이드 필요]";
    public void SetSlot(ForgeManager forgeManager)
    {
        moveBtn.onClick.RemoveAllListeners();

        if (forgeManager.CurrentForge.ForgeType == forgeType)
        {
            lockedIndicator.SetActive(true);
            moveBtn.interactable = false;
            lockedText.text = CurrentForge;
            return;
        }

        foreach (var type in forgeManager.UnlockedForge)
        {
            if (type == forgeType)
            {
                lockedIndicator.SetActive(false);
                moveBtn.interactable = true;
                moveBtn.onClick.AddListener(ClickMoveBtn);
                return;
            }
        }

        lockedIndicator.SetActive(true);
        moveBtn.interactable = false;
        lockedText.text = NeedToUpgrade;
    }

    private void ClickMoveBtn()
    {
        LoadSceneManager.Instance.LoadSceneAsync(GetSceneType(forgeType), true);
    }

    private SceneType GetSceneType(ForgeType type)
    {
        return type switch
        {
            ForgeType.Weapon => SceneType.Forge_Weapon,
            ForgeType.Armor => SceneType.Forge_Armor,
            ForgeType.Magic => SceneType.Forge_Magic,
            _ => SceneType.Forge_Weapon
        };
    }
}

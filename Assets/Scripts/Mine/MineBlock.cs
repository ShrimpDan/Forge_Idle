using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MineBlock : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Button unlockBtn;
    private int _mineIndex;
    private MineSceneManager _manager;

    public void Setup(int mineIndex, int needGold, int currentGold, MineSceneManager manager)
    {
        _mineIndex = mineIndex;
        _manager = manager;
        goldText.text = $"{UIManager.FormatNumber(currentGold)} / {UIManager.FormatNumber(needGold)}";
        unlockBtn.interactable = currentGold >= needGold;

        unlockBtn.onClick.RemoveAllListeners();
        unlockBtn.onClick.AddListener(OnClickUnlock);
    }

    private void OnClickUnlock()
    {
        _manager.TryUnlockMine(_mineIndex);
    }
}

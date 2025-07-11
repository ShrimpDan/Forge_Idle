using UnityEngine;
using UnityEngine.UI;

public class WindowButton : MonoBehaviour
{
    [SerializeField] ButtonType type;
    private Button myBtn;

    void Start()
    {
        myBtn = GetComponent<Button>();
        myBtn.onClick.AddListener(OpenWindowUI);
    }

    private void OpenWindowUI()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
        GameManager.Instance.UIManager.OpenUI<BaseUI>(UIName.GetUINameByType(type));
    }
}

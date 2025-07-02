using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionButton : MonoBehaviour
{
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenCollectionPopup);
    }

    private void OpenCollectionPopup()
    {
        GameManager.Instance.UIManager.OpenUI<BaseUI>(UIName.CollectionPopup);
    }
}

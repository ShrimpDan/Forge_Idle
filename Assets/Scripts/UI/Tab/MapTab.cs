using UnityEngine;

public class MapTab : BaseTab
{
    [SerializeField] GameObject villageMap;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        villageMap.SetActive(false);
    }

    public override void OpenTab()
    {
        base.OpenTab();
        villageMap.SetActive(true);
    }

    public override void CloseTab()
    {
        base.CloseTab();
        villageMap.SetActive(false);
    }
}

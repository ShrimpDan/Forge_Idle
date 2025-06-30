using UnityEngine;

public class ForgeTabPanelController : MonoBehaviour
{
    public GameObject gemPanel;
    public GameObject craftPanel;
    public GameObject upgradePanel;
    public GameObject refinePanel;

    public void ShowGemPanel()
    {
        gemPanel.SetActive(true);
        craftPanel.SetActive(false);
        upgradePanel.SetActive(false);
        refinePanel.SetActive(false);
    }
    public void ShowCraftPanel()
    {
        gemPanel.SetActive(false);
        craftPanel.SetActive(true);
        upgradePanel.SetActive(false);
        refinePanel.SetActive(false);
    }
    public void ShowUpgradePanel()
    {
        gemPanel.SetActive(false);
        craftPanel.SetActive(false);
        upgradePanel.SetActive(true);
        refinePanel.SetActive(false);
    }
    public void ShowRefinePanel()
    {
        gemPanel.SetActive(false);
        craftPanel.SetActive(false);
        upgradePanel.SetActive(false);
        refinePanel.SetActive(true);
    }
}

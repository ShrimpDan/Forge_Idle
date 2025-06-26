using UnityEngine;

public class BaseTab : MonoBehaviour
{
    protected GameManager gameManager;
    protected UIManager uIManager;

    public virtual void Init(GameManager gameManager, UIManager uIManager)
    {
        this.gameManager = gameManager;
        this.uIManager = uIManager;
    }

    public virtual void OpenTab() => gameObject.SetActive(true);
    public virtual void CloseTab() => gameObject.SetActive(false);
}

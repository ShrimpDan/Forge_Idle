using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public abstract UIType UIType { get; }
    protected UIManager uIManager;
    protected GameManager gameManager;

    public virtual void Init(GameManager gameManager, UIManager uIManager)
    {
        if(this.uIManager == null)
            this.uIManager = uIManager;
        if(this.gameManager == null)
            this.gameManager = gameManager;
    }

    public virtual void Open() => gameObject.SetActive(true);
    public virtual void Close() => gameObject.SetActive(false);
}

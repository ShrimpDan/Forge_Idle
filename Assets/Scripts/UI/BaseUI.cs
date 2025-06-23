using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public abstract UIType UIType { get; }
    private UIManager uIManager;

    public virtual void Init(UIManager uIManager)
    {
        this.uIManager = uIManager;
    }

    public virtual void Open() => gameObject.SetActive(true);
    public virtual void Close() => gameObject.SetActive(false);
}

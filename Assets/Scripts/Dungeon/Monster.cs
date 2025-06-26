using UnityEngine;

public class Monster : MonoBehaviour
{
    public System.Action OnDeath;

    private float maxHp;
    private float currentHp;
    private bool isBoss;

    public void Init(float hp, bool isBoss = false)
    {
        maxHp = hp;
        currentHp = hp;
        this.isBoss = isBoss;
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}

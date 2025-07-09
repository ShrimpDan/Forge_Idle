using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    private Animator animator;
    private readonly int hitHash = Animator.StringToHash("Hit");
    private readonly int deathHash = Animator.StringToHash("Death");

    [Header("HpBar UI")]
    [SerializeField] Image hpFill;

    private float maxHp;
    private float currentHp;
    private bool isBoss;

    public System.Action OnDeath;

    public void Init(float hp, bool isBoss = false)
    {
        maxHp = hp;
        currentHp = hp;
        this.isBoss = isBoss;

        animator = GetComponent<Animator>();

        SetHpBar();
    }

    public void TakeDamage(float amount)
    {
        animator.SetTrigger(hitHash);

        currentHp -= amount;
        SetHpBar();

        if (currentHp <= 0)
        {
            StartCoroutine(DeathCoroutine());
        }
    }

    private void SetHpBar()
    {
        hpFill.fillAmount = currentHp / maxHp;
    }

    IEnumerator DeathCoroutine()
    {
        animator.SetTrigger(deathHash);

        yield return WaitForSecondsCache.Wait(0.5f);

        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}

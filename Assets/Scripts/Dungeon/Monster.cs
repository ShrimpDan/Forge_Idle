using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    private MonsterHandler monsterHandler;

    private Animator animator;
    private readonly int hitHash = Animator.StringToHash("Hit");
    private readonly int deathHash = Animator.StringToHash("Death");

    [Header("HpBar UI")]
    [SerializeField] Image hpFill;

    private float maxHp;
    private float currentHp;
    private bool isBoss;

    public System.Action OnDeath;

    public void Init(MonsterHandler monsterHandler, float hp, bool isBoss = false)
    {
        this.monsterHandler = monsterHandler;
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
            monsterHandler.ClearCurrentMonster();
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

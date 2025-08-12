using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    private MonsterHandler monsterHandler;

    private Animator animator;
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int hitHash = Animator.StringToHash("Hit");
    private readonly int deathHash = Animator.StringToHash("Death");

    [Header("HpBar UI")]
    [SerializeField] Image hpFill;

    private float maxHp;
    private float currentHp;
    private bool isBoss;
    public bool IsDie { get; private set; } = false;

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
        currentHp -= amount;
        SetHpBar();

        SoundManager.Instance.Play("SFX_BattleMonsterHit");
        
        if (currentHp <= 0 && !IsDie)
        {
            IsDie = true;
            monsterHandler.ClearCurrentMonster();
            animator.SetTrigger(deathHash);
            SoundManager.Instance.Play("SFX_BattleMonsterDie");
        }
        else
        {
            animator.SetTrigger(hitHash);
        }
    }

    private void SetHpBar()
    {
        hpFill.fillAmount = currentHp / maxHp;
    }

    public void PlayAttackAnim(float animSpeed)
    {
        animator.speed = animSpeed;
        animator.SetTrigger(attackHash);
    }

    public void EndDeathAnim()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}

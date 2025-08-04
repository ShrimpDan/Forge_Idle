using DG.Tweening;
using TMPro;
using UnityEngine;

public class BlackSmith : MonoBehaviour
{
    private readonly int craftingHash = Animator.StringToHash("Crafting");
    private readonly int buyHash = Animator.StringToHash("Buy");

    private SoundManager soundManager;

    [Header("BlackSmith Setting")]
    [SerializeField] private Animator blackSmithAnim;
    [SerializeField] private ParticleSystem craftingParticle;

    [Header("CashRegister Setting")]
    [SerializeField] private Animator cashAnim;
    [SerializeField] private TextMeshPro goldTextPrefab;

    [Header("Skill Effect Setting")]
    [SerializeField] private Transform skillTextRoot;
    [SerializeField] private TextMeshPro skillTextPrefab;

    public bool IsEnable { get; private set; }

    public void Init()
    {
        blackSmithAnim = GetComponent<Animator>();
        soundManager = SoundManager.Instance;
    }

    public void SetCraftingAnimation(bool isCrafting)
    {
        blackSmithAnim.SetBool(craftingHash, isCrafting);
    }

    public void PlayBuyEffect(int cost, Vector3 pos)
    {
        if (!IsEnable) return;

        soundManager.Play("SFX_CoinGain");
        cashAnim.SetTrigger(buyHash);

        TextMeshPro goldText = Instantiate(goldTextPrefab, pos, Quaternion.identity);
        goldText.text = $"+{UIManager.FormatNumber(cost)}";

        Sequence seq = DOTween.Sequence();
        seq.Append(goldText.transform.DOMoveY(pos.y + 1.5f, 0.5f).SetEase(Ease.OutFlash));
        seq.Join(goldText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.3f);
        seq.Append(goldText.DOFade(0f, 0.5f));
        seq.AppendCallback(() => Destroy(goldText.gameObject));
    }

    public void PlayCraftingEffect()
    {
        if (!IsEnable) return;
        craftingParticle.Play();
        soundManager.Play("SFX_ForgeCraft");
    }

    public void PlayTextEffect(string text)
    {
        TextMeshPro skillText = Instantiate(skillTextPrefab, skillTextRoot);
        skillText.text = $"{text}!!";

        Vector3 initialScale = skillText.transform.localScale;
        Vector3 targetScale = initialScale * 1.5f;

        float duration = 1.0f;
        float targetAlpha = 0f;

        Color initialColor = skillText.color;

        skillText.transform.DOScale(targetScale, duration)
            .SetEase(Ease.OutBack);

        skillText.DOColor(new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha), duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(skillText.gameObject));
    }

    public void SetEnable(bool isEnable)
    {
        IsEnable = isEnable;
    }
}

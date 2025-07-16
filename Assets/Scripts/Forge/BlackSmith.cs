using DG.Tweening;
using TMPro;
using UnityEngine;

public class BlackSmith : MonoBehaviour
{
    private readonly int craftingHash = Animator.StringToHash("Crafting");
    private readonly int buyHash = Animator.StringToHash("Buy");

    [Header("BlackSmith Setting")]
    [SerializeField] private Animator blackSmithAnim;
    [SerializeField] private ParticleSystem craftingParticle;

    [Header("CashRegister Setting")]
    [SerializeField] private Animator cashAnim;
    [SerializeField] private TextMeshPro goldTextPrefab;

    [Header("BlackSmith UI Reference")]
    [SerializeField] private GameObject blackSmithUI;

    public void Init()
    {
        blackSmithAnim = GetComponent<Animator>();
    }

    public void SetCraftingAnimation(bool isCrafting)
    {
        blackSmithAnim.SetBool(craftingHash, isCrafting);
    }

    public void PlayBuyEffect(int cost, Vector3 pos)
    {
        if (blackSmithUI != null && blackSmithUI.activeInHierarchy)
        {
            SoundManager.Instance.Play("SFX_CoinGain");
        }

        cashAnim.SetTrigger(buyHash);
        TextMeshPro goldText = Instantiate(goldTextPrefab, pos, Quaternion.identity);
        goldText.text = $"+{cost}G";

        Sequence seq = DOTween.Sequence();
        seq.Append(goldText.transform.DOMoveY(pos.y + 1.5f, 0.5f).SetEase(Ease.OutFlash));
        seq.Join(goldText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.3f);
        seq.Append(goldText.DOFade(0f, 0.5f));
        seq.AppendCallback(() => Destroy(goldText.gameObject));
    }

    public void PlayCraftingParticle()
    {
        craftingParticle.Play();
    }
}

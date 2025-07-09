using UnityEngine;

public class BlackSmith : MonoBehaviour
{
    private Animator animator;
    private readonly int craftingHash = Animator.StringToHash("Crafting");

    [SerializeField] private ParticleSystem craftingParticle;

    public void Init()
    {
        animator = GetComponent<Animator>();
    }

    public void SetCraftingAnimation(bool isCrafting)
    {
        animator.SetBool(craftingHash, isCrafting);
    }

    public void PlayCraftingParticle()
    {
        craftingParticle.Play();
    }
}

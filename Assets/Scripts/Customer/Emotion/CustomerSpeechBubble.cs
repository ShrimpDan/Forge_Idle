using UnityEngine;

public class CustomerSpeechBubble : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        gameObject.SetActive(false);

    }



    public void Show(string aniName)
    {
        gameObject.SetActive(true);
        animator.Play(aniName);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }



}

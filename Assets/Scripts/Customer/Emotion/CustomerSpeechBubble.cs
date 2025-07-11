using UnityEngine;

public class CustomerSpeechBubble : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        gameObject.SetActive(false);

    }



    public void Show(CustomerEmotion emotion)
    {
        gameObject.SetActive(true);
        animator.SetInteger("MoodState", (int)emotion);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }



}

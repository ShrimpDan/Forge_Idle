using UnityEngine;

public class TraineeSummonEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem summonParticle;

    public void PlayEffect(Vector3 position)
    {
        transform.position = position;
        summonParticle.Play();
        Destroy(gameObject, 1.5f);
    }
}
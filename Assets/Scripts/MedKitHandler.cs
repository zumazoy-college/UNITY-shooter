using UnityEngine;

public class MedKitHandler : MonoBehaviour
{
    public ParticleSystem DestroyParticles;
    public AudioSource HealingSound;
    public int HealthPointCount = 20;
    private GameManager _GameManager;

    void Start()
    {
        _GameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        DestroyParticles.transform.parent = null;
        HealingSound.transform.parent = null;
        
        DestroyParticles.Play();
        HealingSound.Play();

        Destroy(gameObject);

        _GameManager.Healing(HealthPointCount);
    }
}

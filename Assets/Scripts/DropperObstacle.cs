using UnityEngine;

public class DropperObstacle : MonoBehaviour
{
    public Transform startPoint;
    public AudioSource LoseSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LoseSound.Play();

            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                collision.gameObject.transform.position = startPoint.position;
            }
        }
    }
}

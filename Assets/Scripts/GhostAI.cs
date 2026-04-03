using UnityEngine;

public class GhostAI : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Настройки движения")]
    public float speed = 3.0f;       // Скорость полета
    public float rotationSpeed = 5.0f; // Скорость поворота к игроку
    public float floatAmplitude = 0.5f; // Высота покачивания вверх-вниз
    public float floatFrequency = 1.0f;  // Частота покачивания

    void Start()
    {
        // Ищем игрока по тегу Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. Движение к игроку (сквозь всё)
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // 2. Плавный поворот в сторону игрока
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // 3. Эффект парения (левитация)
        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.Translate(Vector3.up * newY * Time.deltaTime, Space.World);
    }
}

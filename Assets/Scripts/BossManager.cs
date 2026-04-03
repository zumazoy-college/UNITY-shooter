using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("Характеристики")]
    public int maxHealth = 500;
    private int currentHealth;
    public float speed = 3.5f;
    public int damageToPlayer = 20;
    public float attackDistance = 2.5f;
    private float lastAttackTime;

    [Header("Интерфейс и Эффекты")]
    public Slider healthBar;
    public ParticleSystem hitParticle;

    [Header("События после смерти")]
    public AnimationManager bossAnimator; // Ссылка на AnimationManager босса
    public GameObject[] obstaclesToRemove; // Массив стен, которые исчезнут
    public GameObject teleportToActivate;  // Телепорт, который появится

    private Transform player;
    private GameManager _GameManager;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        _GameManager = FindObjectOfType<GameManager>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (GameManager.ManagerInstance != null && GameManager.ManagerInstance.Player != null)
            player = GameManager.ManagerInstance.Player.transform;
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // В начале уровня прячем телепорт, если он назначен
        if (teleportToActivate != null)
            teleportToActivate.SetActive(false);
    }

    void Update()
    {
        // Если босс мертв или игрока нет - ничего не делаем
        if (player == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.LookAt(targetPosition);
        }
        else if (Time.time > lastAttackTime + 1f)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }

    private void AttackPlayer()
    {
        if (_GameManager != null)
        {
            _GameManager.DamagePlayer(damageToPlayer);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; // Мертвый босс не получает урон

        currentHealth -= damageAmount;

        if (hitParticle != null) hitParticle.Play();

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true; // Блокируем передвижение и атаки

        // 1. Запускаем анимацию смерти
        if (bossAnimator != null) bossAnimator.SetBossDie();

        // 2. Отключаем полоску здоровья босса
        if (healthBar != null) healthBar.gameObject.SetActive(false);

        // 3. Отключаем коллайдер, чтобы игрок мог пройти сквозь труп, а пули не застревали
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4. Убираем стены, освобождая проход
        foreach (GameObject wall in obstaclesToRemove)
        {
            if (wall != null) wall.SetActive(false);
        }

        // 5. Включаем телепорт
        if (teleportToActivate != null) teleportToActivate.SetActive(true);

        // 6. Удаляем объект босса со сцены через 5 секунд (чтобы он успел проиграть анимацию лежания)
        Destroy(gameObject, 5f);
    }
}

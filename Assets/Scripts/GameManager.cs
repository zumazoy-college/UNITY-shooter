using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int Health;
    public int MaxHealth = 100;
    private int Stamina = 100;
    public bool IsStaminaRestoring = false;
    public GameObject Player;
    public static GameManager ManagerInstance;
    public Slider HealthBar;
    public Slider StaminaBar;
    public Image GameOverScreen;

    private void Awake()
    {
        ManagerInstance = this;
    }

    void Start()
    {
        Health = 50;
        HealthBar.value = Health;
        StaminaBar.value = Stamina;
    }

    void FixedUpdate()
    {
        StaminaCheck();
    }

    private void StaminaCheck()
    {
        if (Stamina <= 0) StartCoroutine(StaminaRestore());
    }

    private IEnumerator StaminaRestore()
    {
        IsStaminaRestoring = true;

        yield return new WaitForSeconds(3);

        Stamina = 100;

        StaminaBar.value = Stamina;

        IsStaminaRestoring = false;
    }

    public void SpendStamina()
    {
        Stamina -= 1;
        StaminaBar.value = Stamina;
    }

    public void Healing(int HealthPointCount)
    {
        if (Health + HealthPointCount >= MaxHealth) Health = MaxHealth;
        else Health += HealthPointCount;

        HealthBar.value = Health;
        Debug.Log("HP: " + Health);
    }

    public void DamagePlayer(int Count)
    {
        //если колво жизнеё больше нуля
        if (Health > 0)
        {
            //то мы от жизней отнимаем количество параметр каунт и в лог урон выводим
            Health -= Count;
            HealthBar.value = Health;
            Debug.Log("Вам нанесли урон" + Count);
        } //елс нелф меньше либо равно нулю то смерть в консоль просто пока
        else if (Health <= 0) GameOver();
    }

    public void GameOver()
    {
        GameOverScreen.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

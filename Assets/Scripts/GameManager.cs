using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int Health;
    public int MaxHealth = 100;
    private int Stamina = 100;
    public bool IsStaminaRestoring = false;


    void Start()
    {
        Health = 50;
    }

    void FixedUpdate()
    {
        StaminaCheck();
    }

    private void StaminaCheck()
    {
        //Debug.Log("Стамина: " + Stamina);
        if (Stamina <= 0) StartCoroutine(StaminaRestore());
    }

    private IEnumerator StaminaRestore()
    {
        IsStaminaRestoring = true;

        yield return new WaitForSeconds(3);

        Stamina = 100;

        IsStaminaRestoring = false;
    }

    public void SpendStamina()
    {
        Stamina -= 1;

    }

    public void Healing(int HealthPointCount)
    {
        if (Health + HealthPointCount >= MaxHealth) Health = MaxHealth;
        else Health += HealthPointCount;

        Debug.Log("HP: " + Health);
    }
}

using UnityEngine.UI;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponsEnum WeaponType;
    public Text AmmoText;
    public int MaxMagazineBulletCount;
    public int CurrentMagazineBulletCount;
    public int MaxAmmoSupply;
    public int WeaponDamage;
    public float TimeBetweenShots;
    public float TimeForReloading;
    bool CanFire = true;
    bool IsReloading = false;
    public AudioSource ShotSound;
    public AudioSource ReloadSound;
    public LayerMask AttackableLayer;
    private AnimationManager _AnimationManager;

    public void SetAnimationManager(AnimationManager animationManager)
    {
        _AnimationManager = animationManager;
    }

    private IEnumerator LockFire(float Time)
    {
        yield return new WaitForSeconds(Time);
        _AnimationManager.ResetAnimatorSpeed();
        CanFire = true;
    }

    private IEnumerator LockFireForReloading(float Time)
    {
        ReloadSound.Play();

        yield return new WaitForSeconds(Time);
        CurrentMagazineBulletCount = MaxMagazineBulletCount;

        MaxAmmoSupply -= MaxMagazineBulletCount - CurrentMagazineBulletCount;

        CanFire = true;
        IsReloading = false;

        UpdateUI();

        Debug.Log("Reload is complete");
    }

    public bool IsMagazineEmpty()
    {
        if (CurrentMagazineBulletCount == 0) return true;
        else return false;
    }

    public void Reload()
    {
        if (!IsReloading)
        {
            _AnimationManager.SetAnimationReload();
            Debug.Log("Start Reloading");
            IsReloading = true;
            CanFire = false;

            float reloadClipLength = _AnimationManager.GetClipLength("reload");
            float reloadTime = reloadClipLength > 0 ? reloadClipLength : TimeForReloading;
            StartCoroutine(LockFireForReloading(reloadTime));
        }
    }

    public void Fire()
    {
        if (CanFire && !IsMagazineEmpty() && !IsReloading)
        {
            ShotSound.Play();
            _AnimationManager.SetAnimationFire(TimeBetweenShots);

            CurrentMagazineBulletCount--;
            Debug.Log("Bullets remaining: " + CurrentMagazineBulletCount);

            RaycastHit HitInfo = new RaycastHit();

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo))
            {
                if (HitInfo.transform.gameObject.layer == 6)
                {
                    //тоес с того объекта которому мы нанесли урон мы забираем енему
                    //контроллер обращаемя к нему и вызываем метод дамага
                    EnemyController EC = HitInfo.transform.gameObject.GetComponent<EnemyController>();
                    EC.DealDamage(WeaponDamage);

                    Debug.Log("Вы попали по " + HitInfo.transform.name + " и нанесли ему " + WeaponDamage);
                }
            }

            UpdateUI();

            CanFire = false;
            StartCoroutine(LockFire(TimeBetweenShots));
        }
    }

    public void UpdateUI()
    {
        AmmoText.text = CurrentMagazineBulletCount + "/" + MaxMagazineBulletCount + " ";
    }
}

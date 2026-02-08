using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponsEnum WeaponType;
    public int MaxMagazineBulletCount;
    public int CurrentMagazineBulletCount;
    public int MaxAmmoSupply;
    public float TimeBetweenShots;
    public float TimeForReloading;
    bool CanFire = true;
    bool IsReloading = false;
    public AudioSource ShotSound;
    public AudioSource ReloadSound;
    
    private IEnumerator LockFire(float Time)
    {
        yield return new WaitForSeconds(Time);
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

        Debug.Log("Reload is complete");
    }

    public bool IsMagazineEmpty()
    {
        if (CurrentMagazineBulletCount == 0) return true;
        else return false;
    }

    public void Reload(AnimationManager _AnimationManager)
    {
        if (!IsReloading)
        {
            _AnimationManager.SetAnimationReload();
            Debug.Log("Start Reloading");
            IsReloading = true;
            CanFire = false;

            StartCoroutine(LockFireForReloading(TimeForReloading));
        }
    }

    public void Fire(AnimationManager _AnimationManager)
    {
        if (CanFire && !IsMagazineEmpty() && !IsReloading)
        {
            ShotSound.Play();
            _AnimationManager.SetAnimationFire();

            CurrentMagazineBulletCount--;
            Debug.Log("Bullets remaining: " + CurrentMagazineBulletCount);

            RaycastHit HitInfo = new RaycastHit();

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo))
            {
                Debug.Log(HitInfo.transform.name);
            }
            CanFire = false;
            StartCoroutine(LockFire(TimeBetweenShots));
        }
    }
}

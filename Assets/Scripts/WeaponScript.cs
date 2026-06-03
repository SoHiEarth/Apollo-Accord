using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class WeaponScript : MonoBehaviour
{
    public int currentAmmo;
    public int magazineSize;
    public int totalAmmo;
    // milliseconds between shots
    public float fireRate = 0.1f;
    public float reloadTime = 2.5f;
    bool canShoot = true;
    public TextMeshPro ammoText;
    public TextMeshPro totalAmmoText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        ammoText.text = currentAmmo.ToString();
        totalAmmoText.text = totalAmmo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }

    System.Collections.IEnumerator ResetCanShoot()
    {
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    public bool Shoot()
    {        
        if (!canShoot || currentAmmo <= 0)
        {
            return false;
        }
        else if (currentAmmo > 0)
        {
            currentAmmo--;
            ammoText.text = currentAmmo.ToString();
            canShoot = false;
            // start a coroutine to reset canShoot after fireRate seconds
            StartCoroutine(ResetCanShoot());
        }
        return true;
    }

    bool awaitReloading = false;

    System.Collections.IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        awaitReloading = false;
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
        if (!awaitReloading) {
            int ammoNeeded = magazineSize - currentAmmo;
            if (totalAmmo >= ammoNeeded)
            {
                totalAmmo -= ammoNeeded;
                currentAmmo += ammoNeeded;
            }
            else
            {
                currentAmmo += totalAmmo;
                totalAmmo = 0;
            }
            ammoText.text = currentAmmo.ToString();
            totalAmmoText.text = totalAmmo.ToString();
        }
    }
}
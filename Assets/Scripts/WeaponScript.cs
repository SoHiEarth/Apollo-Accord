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
    public GameObject muzzleSmoke;
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

    System.Collections.IEnumerator DisableMuzzleSmoke()
    {
        yield return new WaitForSeconds(3f);
        muzzleSmoke.SetActive(false);
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
            if (muzzleSmoke != null)
            {
                muzzleSmoke.SetActive(true);
                StartCoroutine(DisableMuzzleSmoke());
            }
        }
        return true;
    }

    System.Collections.IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
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

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }
}
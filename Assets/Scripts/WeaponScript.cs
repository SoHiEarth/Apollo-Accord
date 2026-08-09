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
    public float damage = 10f;
    bool canShoot = true;
    public TextMeshPro ammoText;
    public TextMeshPro totalAmmoText;
    public GameObject muzzleSmoke;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 500f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        ammoText.text = currentAmmo.ToString();
        totalAmmoText.text = totalAmmo.ToString();
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
            if (bulletPrefab != null && firePoint != null)
            {
                // Bullet prefab is for visual effect only
                GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = firePoint.forward * bulletSpeed;
                }

                // Also raycast
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, bulletSpeed * fireRate))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
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

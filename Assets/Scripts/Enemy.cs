using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
  [Header("Health Settings")]
  public float health = 100f;
  public float maxHealth = 100f;
  public bool canRegenerateHealth = false;
  public float healthRegenMaxTotalAmount = 40f;
  private float totalHealthRegenerated = 0f;
  public float healthRegenRate = 1f;
  public float healthRegenAmount = 1f;
  public float healthRegenStartDelay = 5f;
  private float healthRegenTimer = 0f;

  [Header("Movement Settings")]
  public float speed = 5f;
  public float sightRange = 10f;

  [Header("Melee Attack Settings")]
  public float meleeDamage = 10f;
  public float meleeRange = 2f;
  public float meleeCooldown = 1f;
  private float meleeCooldownTimer = 0f;

  [Header("Projectile Settings")]
  public bool canEmitProjectiles = true;
  public GameObject projectilePrefab;
  public Transform firePoint;
  public float projectileSpeed = 15f;
  public float projectileRange = 20f;
  public float projectileCooldown = 1f;
  public float projectileDamage = 5f;

  [Header("Drop Settings")]
  public GameObject ammoDropPrefab;
  public int ammoDropAmountRangeMin = 2;
  public int ammoDropAmountRangeMax = 4;
  public GameObject healthDropPrefab;
  public int healthDropAmountRangeMin = 1;
  public int healthDropAmountRangeMax = 3;
  private float projectileCooldownTimer = 0f;
  private NavMeshAgent agent;
  private Transform player;
  private float distanceToPlayer;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player").transform;
    agent = GetComponent<NavMeshAgent>();
  }

  // Update is called once per frame
  void Update()
  {
    distanceToPlayer = Vector3.Distance(transform.position, player.position);
    if (distanceToPlayer <= sightRange)
    {
      MoveTowardsPlayer();
      if (distanceToPlayer <= meleeRange)
      {
        MeleeAttack();
      }
      else if (canEmitProjectiles && distanceToPlayer <= projectileRange)
      {
        ProjectileAttack();
      }
    }
    else
    {
      // Move randomly
      MoveRandomly();
    }
    if (canRegenerateHealth && health < maxHealth && totalHealthRegenerated < healthRegenMaxTotalAmount)
    {
      healthRegenTimer += Time.deltaTime;
      if (healthRegenTimer >= healthRegenStartDelay)
      {
        health += Mathf.RoundToInt(healthRegenAmount);
        totalHealthRegenerated += healthRegenAmount;
        healthRegenTimer = 0f;
      }
    }
  }

  private void MoveTowardsPlayer()
  {
    if (agent != null)
    {
      // slightly offset the destination to avoid overlapping with the player
      agent.SetDestination(player.position + (player.position - transform.position).normalized * 0.5f);
    }
  }

  private void MeleeAttack()
  {
    if (meleeCooldownTimer <= 0f)
    {
      // Perform melee attack
      player.GetComponent<PlayerMovement>().TakeDamage(meleeDamage);
      meleeCooldownTimer = meleeCooldown;
    }
    else
    {
      meleeCooldownTimer -= Time.deltaTime;
    }
  }

  private void ProjectileAttack()
  {
    if (projectileCooldownTimer <= 0f)
    {
      // Perform projectile attack
      GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
      projectile.GetComponent<Rigidbody>().linearVelocity = (player.position - transform.position).normalized * projectileSpeed;
      projectile.GetComponent<Projectile>().damage = projectileDamage;
      projectileCooldownTimer = projectileCooldown;
    }
    else
    {
      projectileCooldownTimer -= Time.deltaTime;
    }
  }

  private void MoveRandomly()
  {
    Vector3 randomDirection = Random.insideUnitSphere * sightRange;
    randomDirection += transform.position;
    NavMeshHit hit;
    NavMesh.SamplePosition(randomDirection, out hit, sightRange, 1);
    Vector3 finalPosition = hit.position;

    if (agent != null)
    {
      agent.SetDestination(finalPosition);
    }
  }

  public void TakeDamage(float damage)
  {
    Debug.Log("Enemy took damage: " + damage);
    health -= damage;
    if (health <= 0)
    {
      Destroy(gameObject);
      // drop some ammo when the enemy is destroyed
      if (ammoDropPrefab != null)
      {
        int ammoAmount = Random.Range(ammoDropAmountRangeMin, ammoDropAmountRangeMax + 1);
        for (int i = 0; i < ammoAmount; i++)
        {
          Instantiate(ammoDropPrefab, transform.position + new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f)), Quaternion.identity);
        }

        int healthAmount = Random.Range(healthDropAmountRangeMin, healthDropAmountRangeMax + 1);
        for (int i = 0; i < healthAmount; i++)
        {
          Instantiate(healthDropPrefab, transform.position + new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f)), Quaternion.identity);
        }
      }
    }
  }
}

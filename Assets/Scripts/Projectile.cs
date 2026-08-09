using UnityEngine;

public class Projectile : MonoBehaviour
{
  public float damage = 10;
  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.CompareTag("Player"))
    {
      PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
      if (playerMovement != null)
      {
        playerMovement.TakeDamage(damage);
      }

      Destroy(gameObject);
    }
  }
}

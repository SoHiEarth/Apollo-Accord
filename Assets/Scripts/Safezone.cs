using UnityEngine;

public class Safezone : MonoBehaviour
{
  private void OnTriggerEnter(Collider other) {
    if (other.gameObject.tag == "Player") {
      PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
      if (playerMovement) {
        playerMovement.HolsterItem();
      }
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.gameObject.tag == "Player") {
      PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
      if (playerMovement) {
        playerMovement.UnholsterItem();
      }
    }
  }
}

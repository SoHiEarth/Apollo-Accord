using UnityEngine;

public class BulletScript : MonoBehaviour
{
  public float aliveTime = 5.0F;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
      Destroy(this.gameObject, aliveTime);
    }
}

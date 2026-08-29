using UnityEngine;

public class HealthCube : MonoBehaviour
{
    public float rotationSpeed = 50f; // Speed of rotation in degrees per second
    public float bobbingAmplitude = 0.5f; // Amplitude of the bobbing motion
    public float bobbingFrequency = 1f; // Frequency of the bobbing motion
    public int healthAmount = 25; // Amount of health to give the player
    void Update()
    {
        // Rotate the cube around its Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bobbing motion
        float newY = Mathf.Abs(Mathf.Sin(Time.time * bobbingFrequency)) * bobbingAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Assuming the player has a PlayerMovement script with an AddHealth method
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.AddHealth(healthAmount);
                Destroy(gameObject); // Destroy the health cube after giving health
            }
        }
    }
}

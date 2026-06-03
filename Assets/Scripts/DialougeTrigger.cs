using UnityEngine;

public class DialougeTrigger : MonoBehaviour
{
    Collider thisCollider;
    public string speaker = "";
    public string dialouge = "";
    public float dialougeDuration = 2f;
    public float triggerResetDuration = 8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisCollider = GetComponent<Collider>();
    }

    System.Collections.IEnumerator ResetTriggerAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        thisCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDialouge playerDialouge = other.GetComponent<PlayerDialouge>();
            if (playerDialouge != null)
            {
                playerDialouge.SetDialouge(speaker, dialouge, dialougeDuration);
                thisCollider.enabled = false;
                StartCoroutine(ResetTriggerAfterTime(triggerResetDuration));
            }
        }
    }
}

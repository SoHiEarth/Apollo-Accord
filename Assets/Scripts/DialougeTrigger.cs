using UnityEngine;

public class DialougeTrigger : MonoBehaviour
{
    Collider thisCollider;
    public string speaker = "";
    public string[] dialouge = new string[0];
    public float dialougeDuration = 2f;
    public float triggerResetDuration = 8f;
    public bool playOnce = false;
    bool hasPlayed = false;
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
        if (playOnce && hasPlayed)
            return;
        if (other.CompareTag("Player"))
        {
            PlayerDialouge playerDialouge = other.GetComponent<PlayerDialouge>();
            if (playerDialouge != null)
            {
                foreach (string line in dialouge)
                {
                    playerDialouge.SetDialouge(speaker, line, dialougeDuration);
                }
                if (playOnce)
                    hasPlayed = true;
                thisCollider.enabled = false;
                StartCoroutine(ResetTriggerAfterTime(triggerResetDuration));
            }
        }
    }
}

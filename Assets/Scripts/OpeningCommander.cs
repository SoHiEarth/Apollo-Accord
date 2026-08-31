using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpeningCommander : MonoBehaviour
{
    Animator animator;

    public string questTitle = "Make Peace";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private IEnumerator ResumeAnimationAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Pause animation for 6 sec
            animator.enabled = false;
            StartCoroutine(ResumeAnimationAfterTime(6f));
            // Add quest "Make Peace"
            PlayerQuest playerQuest = other.GetComponent<PlayerQuest>();
            if (playerQuest != null)
            {
                playerQuest.SetQuest(questTitle);
            }
        }
    }
}

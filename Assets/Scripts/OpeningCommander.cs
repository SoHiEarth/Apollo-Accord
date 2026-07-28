using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpeningCommander : MonoBehaviour
{
    Collider collider;
    Animator animator;

    public string questTitle = "Make Peace";
    public string initialQuestDescription = "Go to the communications module.";

    void Start()
    {
        collider = GetComponent<Collider>();
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
                playerQuest.SetQuest(questTitle, initialQuestDescription);
            }
        }
    }
}

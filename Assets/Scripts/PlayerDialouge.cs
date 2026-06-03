using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerDialouge : MonoBehaviour
{
    public GameObject speakerObject;
    public GameObject dialougeObject;
    TextMeshProUGUI speakerText;
    TextMeshProUGUI dialougeText;
    public GameObject dialougePanel;
    Animator animator;
    readonly Queue<DialougeEntry> dialougeQueue = new Queue<DialougeEntry>();
    bool isPlayingDialouge;

    struct DialougeEntry
    {
        public string speaker;
        public string dialouge;
        public float duration;

        public DialougeEntry(string speaker, string dialouge, float duration)
        {
            this.speaker = speaker;
            this.dialouge = dialouge;
            this.duration = duration;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (speakerObject != null)
            speakerText = speakerObject.GetComponent<TextMeshProUGUI>();
        if (dialougeObject != null)
            dialougeText = dialougeObject.GetComponent<TextMeshProUGUI>();
        if (dialougePanel != null)
            animator = dialougePanel.GetComponent<Animator>();
    }

    public void SetDialouge(string speaker, string dialouge, float duration)
    {
        dialougeQueue.Enqueue(new DialougeEntry(speaker, dialouge, duration));

        if (!isPlayingDialouge)
            StartCoroutine(PlayDialougeQueue());
    }

    private IEnumerator PlayDialougeQueue()
    {
        isPlayingDialouge = true;

        while (dialougeQueue.Count > 0)
        {
            DialougeEntry entry = dialougeQueue.Dequeue();

            if (dialougePanel != null)
                dialougePanel.SetActive(true);

            if (speakerText != null)
                speakerText.text = entry.speaker;

            if (dialougeText != null)
                dialougeText.text = entry.dialouge;

            if (animator != null)
                animator.SetTrigger("ShowDialouge");

            yield return new WaitForSeconds(0.25f);
            yield return new WaitForSeconds(entry.duration);

            if (animator != null)
                animator.SetTrigger("HideDialouge");

            yield return new WaitForSeconds(0.25f);

            if (dialougePanel != null)
                dialougePanel.SetActive(false);
        }

        isPlayingDialouge = false;
    }
}

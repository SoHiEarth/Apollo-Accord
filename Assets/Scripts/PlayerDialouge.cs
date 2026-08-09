using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerDialouge : MonoBehaviour
{
  CapsuleCollider playerCollider;
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
    public BoxCollider boxCollider;

    public DialougeEntry(string speaker, string dialouge, float duration, BoxCollider boxCollider)
    {
      this.speaker = speaker;
      this.dialouge = dialouge;
      this.duration = duration;
      this.boxCollider = boxCollider;
    }
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    playerCollider = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider>();
    if (speakerObject != null)
      speakerText = speakerObject.GetComponent<TextMeshProUGUI>();
    if (dialougeObject != null)
      dialougeText = dialougeObject.GetComponent<TextMeshProUGUI>();
    if (dialougePanel != null)
      animator = dialougePanel.GetComponent<Animator>();
  }

  public void SetDialouge(string speaker, string dialouge, float duration, BoxCollider boxCollider)
  {
    dialougeQueue.Enqueue(new DialougeEntry(speaker, dialouge, duration, boxCollider));
    if (!isPlayingDialouge)
      StartCoroutine(PlayDialougeQueue());
  }

  private IEnumerator PlayDialougeQueue()
  {
    isPlayingDialouge = true;
    while (dialougeQueue.Count > 0)
    {
      // Skip if player exits the box collider area before the dialouge is displayed
      if (dialougeQueue.Peek().boxCollider != null)
      {
        if (!dialougeQueue.Peek().boxCollider.bounds.Intersects(playerCollider.bounds))
        {
          dialougeQueue.Dequeue();
          Debug.Log("Player not inside trigger, skipping dialogue.");
          continue;
        }
      } else
      {
        Debug.LogWarning("BoxCollider is null for the current dialogue entry.");
      }
      DialougeEntry entry = dialougeQueue.Dequeue();
      if (speakerText != null)
        speakerText.text = entry.speaker;
      if (dialougeText != null)
        dialougeText.text = entry.dialouge;
      if (animator != null)
        animator.SetTrigger("ShowDialouge");
      yield return new WaitForSeconds(entry.duration);
      if (animator != null)
        animator.SetTrigger("HideDialouge");
    }
    isPlayingDialouge = false;
  }
}

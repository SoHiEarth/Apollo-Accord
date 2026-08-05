using UnityEngine;

public class FoxtrotGroupBehaviour : MonoBehaviour
{
    QuestInterface quest_interface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quest_interface = GameObject.Find("Player_QuestHUD").GetComponent<QuestInterface>();
    }

    // Update is called once per frame
    void Update()
    {
        if (quest_interface.GetQuestTitle() == "Make Peace")
        {
            this.gameObject.SetActive(false);
        }
    }
}

using UnityEngine;

public class PlayerQuest : MonoBehaviour
{
    public GameObject questPanel;

    public void SetQuest(string title, string description)
    {
        if (questPanel != null)
        {
            QuestInterface questInterface = questPanel.GetComponent<QuestInterface>();
            if (questInterface != null)
            {
                questInterface.SetQuestTitle(title);
                questInterface.SetQuestDescription(description);
                questInterface.ShowQuestInterface();
            }
        }
    }

    public void UpdateQuest(string title, string description)
    {
        if (questPanel != null)
        {
            QuestInterface questInterface = questPanel.GetComponent<QuestInterface>();
            if (questInterface != null)
            {
                if (questInterface.GetQuestTitle() == title)
                {
                    questInterface.SetQuestDescription(description);
                }
            }
        }
    }

    public void ResetQuest(string title)
    {
        if (questPanel != null)
        {
            QuestInterface questInterface = questPanel.GetComponent<QuestInterface>();
            if (questInterface != null)
            {
                if (questInterface.GetQuestTitle() == title)
                    questInterface.ResetQuestInterface();
            }
        }
    }

    public void HideQuest()
    {
        if (questPanel != null)
        {
            QuestInterface questInterface = questPanel.GetComponent<QuestInterface>();
            if (questInterface != null)
            {
                questInterface.ResetQuestInterface();
            }
        }
    }

    void Start() {
        HideQuest();
    }
}

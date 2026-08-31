using UnityEngine;

public class PlayerQuest : MonoBehaviour
{
    public GameObject questPanel;

    public void SetQuest(string title)
    {
        if (questPanel != null)
        {
            QuestInterface questInterface = questPanel.GetComponent<QuestInterface>();
            if (questInterface != null)
            {
                questInterface.SetQuestTitle(title);
                questInterface.ShowQuestInterface();
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

using UnityEngine;
using TMPro;

public class QuestInterface : MonoBehaviour
{
    public TextMeshProUGUI quest_title;
    public TextMeshProUGUI quest_description;

    public void SetQuestTitle(string title)
    {
        if (quest_title != null)
            quest_title.text = title;
    }

    public string GetQuestTitle()
    {
        if (quest_title != null)
            return quest_title.text;
        return "";
    }

    public void SetQuestDescription(string description)
    {
        if (quest_description != null)
            quest_description.text = description;
    }

    public void ShowQuestInterface()
    {
        this.gameObject.SetActive(true);
    }

    public void ResetQuestInterface()
    {
        if (quest_title != null)
            quest_title.text = "";
        if (quest_description != null)
            quest_description.text = "";
        this.gameObject.SetActive(false);
    }
}

using UnityEngine;
using TMPro;

public class QuestInterface : MonoBehaviour
{
    public TextMeshProUGUI QuestTitle;
    public TextMeshProUGUI QuestDescription;

    public void SetQuestTitle(string title)
    {
        if (QuestTitle != null)
            QuestTitle.text = title;
    }

    public string GetQuestTitle()
    {
        if (QuestTitle != null)
            return QuestTitle.text;
        return "";
    }

    public void SetQuestDescription(string description)
    {
        if (QuestDescription != null)
            QuestDescription.text = description;
    }

    public void ShowQuestInterface()
    {
        this.gameObject.SetActive(true);
    }

    public void ResetQuestInterface()
    {
        if (QuestTitle != null)
            QuestTitle.text = "";
        if (QuestDescription != null)
            QuestDescription.text = "";
        this.gameObject.SetActive(false);
    }
}

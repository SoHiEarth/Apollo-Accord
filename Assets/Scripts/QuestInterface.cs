using UnityEngine;
using TMPro;

public class QuestInterface : MonoBehaviour
{
    public TextMeshProUGUI QuestTitle;

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

    public void ShowQuestInterface()
    {
        this.gameObject.SetActive(true);
    }

    public void ResetQuestInterface()
    {
        if (QuestTitle != null)
            QuestTitle.text = "";
        this.gameObject.SetActive(false);
    }
}

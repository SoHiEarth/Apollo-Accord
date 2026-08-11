using UnityEngine;

public class FoxtrotGroupBehaviour : MonoBehaviour
{
    QuestInterface quest_interface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
      GameObject player_quest_hud = GameObject.Find("Player_QuestHUD");
      if (player_quest_hud != null) {
        quest_interface = player_quest_hud.GetComponent<QuestInterface>();  
      }
    }

    // Update is called once per frame
    void Update() {
      if (quest_interface != null) {
        if (quest_interface.GetQuestTitle() == "Make Peace") {
          this.gameObject.SetActive(false);
        }
      } else {
        GameObject player_quest_hud = GameObject.Find("Player_QuestHUD");
        if (player_quest_hud != null) {
          quest_interface = player_quest_hud.GetComponent<QuestInterface>();  
        }
      }
    }
}


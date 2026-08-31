using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using SaveSystem;

public class MissionOneHandler : MonoBehaviour
{
    public Transform EnemySpawnPoint;
    public int EnemyCount = 0; // Will be multiplied by the enemy count multiplier.
    private SaveData saveData;
    private ConfigurationData configData;
    bool isMissionOneCompleted = false;
    bool comsModuleFinished = false;
    bool comsModuleDialoguePlayed = false;
    public float quicktimeDuration = 60f;
    private float quicktimeTimer = 0f;
    private bool isQuicktimeFinished = false;
    PlayerDialouge playerDialouge;
    void Start()
    {
        saveData = SaveSystem.SaveSystem.LoadGame("current");
        configData = SaveSystem.SaveSystem.LoadConfiguration();
        playerDialouge = GameObject.FindWithTag("Player").GetComponent<PlayerDialouge>();
    }

    // Update is called once per frame
    void Update()
    {
        if (comsModuleFinished && !isQuicktimeFinished)
        {
            if (playerDialouge != null && !comsModuleDialoguePlayed)
            {
                playerDialouge.SetDialouge("Group Alpha", "We recieved your signal, but these things keep attacking us! Come to the mess hall, now!", 5f, null);
                comsModuleDialoguePlayed = true;
            }
            GameObject group_one = GameObject.Find("Group 1");
            GameObject group_two = GameObject.Find("Group 2");
            if (group_one != null && group_two != null)
            {
                group_one.SetActive(false);
                group_two.SetActive(false);
            }
        }
    }

    public void ComsModuleInteracted()
    {
        comsModuleFinished = true;
    }
}

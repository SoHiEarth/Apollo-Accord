using UnityEngine;
using UnityEngine.InputSystem;

public class ComsModule : MonoBehaviour
{
    public GameObject defaultCanvas;
    public GameObject interactedCanvas;
    public InputAction interactAction;

    void Start()
    {
        if (interactAction != null)
        {
            interactAction.Enable();
        }

        defaultCanvas.SetActive(true);
        interactedCanvas.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactAction != null && interactAction.IsPressed())
            {
                defaultCanvas.SetActive(false);
                interactedCanvas.SetActive(true);
                GameObject gameHandler = GameObject.Find("GameHandler");
                if (gameHandler != null)
                {
                    MissionOneHandler missionOneHandler = gameHandler.GetComponent<MissionOneHandler>();
                    if (missionOneHandler != null)
                    {
                        missionOneHandler.ComsModuleInteracted();
                    }
                }
            }
        }

        GameObject interactHUD = GameObject.Find("InteractHUD");
        if (interactHUD != null)
        {
            interactHUD.SetActive(other.CompareTag("Player"));
        }
    }
}

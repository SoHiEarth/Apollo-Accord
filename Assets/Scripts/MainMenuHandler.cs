using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject camera;
    public GameObject canvas;
    public GameObject overlay;
    public GameObject startButton;
    public GameObject loadButton;
    public GameObject optionsButton;
    public GameObject quitButton;
    public GameObject LoadSlider;
    public GameObject LoadPercentage;
    private Animator menuAnimator;
    private Animator overlayAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (camera != null)
        {
            menuAnimator = camera.GetComponent<Animator>();
        }

        if (overlay != null)
        {
            overlayAnimator = overlay.GetComponent<Animator>();
        }

        HookButton(startButton, OnStartButtonClicked);
        HookButton(loadButton, OnLoadButtonClicked);
        HookButton(optionsButton, OnOptionsButtonClicked);
        HookButton(quitButton, OnQuitButtonClicked);
        
        if (LoadSlider != null)
        {
            LoadSlider.SetActive(false);
        }

        if (LoadPercentage != null)
        {
            LoadPercentage.SetActive(false);
        }
    }

    private void HookButton(GameObject buttonObject, UnityEngine.Events.UnityAction action)
    {
        if (buttonObject == null)
        {
            return;
        }

        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    private void OnStartButtonClicked()
    {
        StartCoroutine(TransitionToMainScene());
    }

    private void OnLoadButtonClicked()
    {
        Debug.Log("Load button clicked, but load flow is not implemented yet.");
    }

    private void OnOptionsButtonClicked()
    {
        Debug.Log("Options button clicked, but options flow is not implemented yet.");
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    // Play the animation and load the main scene asynchronously (hopefully)
    // If the load is longer than the animation (!?), the loading slider will display
    // how much time is left.
    private System.Collections.IEnumerator TransitionToMainScene()
    {
        canvas.SetActive(false);
        overlayAnimator.Play("MainMenu_OverlayTransition", 0, 0f);
        menuAnimator.Play("MainMenu_Transition", 0, 0f);
        menuAnimator.Update(0f);
        yield return null;
        // wait for the animation to finish
        while (menuAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainMenu_Transition") && menuAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync("NewScene");
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f) * 100f;
            if (LoadSlider != null)
            {
                Slider slider = LoadSlider.GetComponent<Slider>();
                if (slider != null)
                {
                    slider.value = progress / 100f;
                }
            }
            if (LoadPercentage != null)
            {
                Text percentageText = LoadPercentage.GetComponent<Text>();
                if (percentageText != null)
                {
                    percentageText.text = $"{progress:F0}%";
                }
            }
            yield return null;
        }
    }
}

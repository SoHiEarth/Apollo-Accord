using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SaveSystem;
using System.Linq;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject canvas;
    public GameObject overlay;
    public GameObject LoadSlider;
    public GameObject LoadPercentage;
    public TMP_Text popupMessageText;
    [Header("Main Menu Buttons")]
    public GameObject mainMenuPanel;
    public GameObject startButton;
    public GameObject loadButton;
    public GameObject optionsButton;
    public GameObject quitButton;
    [Header("Load Game")]
    public GameObject loadGamePanel;
    public GameObject backButton;
    public GameObject NoSavedGamesText;

    [Header("Options")]
    public GameObject optionsPanel;
    public TMP_Text optionsTitle;
    public GameObject backToMainMenuButton;
    public GameObject gameOptionsButton;
    public GameObject gameOptionsPanel;
    public GameObject videoOptionsButton;
    public GameObject videoOptionsPanel;
    public GameObject audioOptionsButton;
    public GameObject audioOptionsPanel;
    public GameObject inputOptionsButton;
    public GameObject inputOptionsPanel;
    [Header("Game Options")]
    [Header("Planet Selection")]
    public TMP_Dropdown planetDropdown;
    [Header("Enemy Options")]
    public Slider enemyMultiplierSlider;
    public TMP_Text enemyMultiplierValueText;
    [Header("Audio Options")]
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeValueText;
    public TMP_Dropdown audioMixDropdown; // Neutral, Harsh, Midnight.
    [Header("Video Options")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown windowModeDropdown; // Fullscreen, Windowed, Borderless.
    public TMP_Dropdown resolutionDropdown;
    public Slider postProcessingAmountSlider;
    public TMP_Text postProcessingAmountValueText;
    [Header("Input Options")]
    public Slider mouseSensitivitySlider;
    public TMP_Text mouseSensitivityValueText;
    private GameObject EarthPlanet;
    private GameObject MarsPlanet;
    private GameObject MoonPlanet;
    private Animator menuAnimator;
    private Animator overlayAnimator;
    private ConfigurationData configData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        configData = SaveSystem.SaveSystem.LoadConfiguration();

        if (menuCamera != null)
        {
            menuAnimator = menuCamera.GetComponent<Animator>();
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

        EarthPlanet = GameObject.Find("EarthPlanet");
        MarsPlanet = GameObject.Find("MarsPlanet");
        MoonPlanet = GameObject.Find("MoonPlanet");

        // get the dropdown's value and set the planet accordingly
        if (planetDropdown != null) 
        {
            // Set the dropdown value based on the saved configuration
            if (configData != null)
            {
                planetDropdown.value = int.Parse(configData.settings.GetValueOrDefault("SelectedPlanet", "0"));
            }
            planetDropdown.onValueChanged.AddListener(OnPlanetDropdownValueChanged);
            OnPlanetDropdownValueChanged(planetDropdown.value);
        }

        if (enemyMultiplierSlider != null)
        {
            if (configData != null)
            {
                enemyMultiplierSlider.value = float.Parse(configData.settings.GetValueOrDefault("EnemyMultiplier", "1.0"));
            }
            enemyMultiplierSlider.onValueChanged.AddListener((value) => 
            { 
                enemyMultiplierValueText.text = Mathf.RoundToInt(value).ToString() + "x"; 
                configData.settings["EnemyMultiplier"] = value.ToString("F1");
            });
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener((value) => 
            { 
                masterVolumeValueText.text = Mathf.RoundToInt(value * 100).ToString() + "%"; 
                configData.settings["MasterVolume"] = value.ToString("F2");
            });
        }


        if (audioMixDropdown != null)
        {
            if (configData != null)
            {
                audioMixDropdown.value = int.Parse(configData.settings.GetValueOrDefault("AudioMix", "0"));
            }
            audioMixDropdown.onValueChanged.AddListener((value) => 
            { 
                configData.settings["AudioMix"] = value.ToString();
            });
        }

        // query the quality settings and populate the dropdown with the available quality levels
        string[] qualityLevels = QualitySettings.names;

        if (qualityDropdown != null)
        {
            if (configData != null)
            {
                qualityDropdown.value = int.Parse(configData.settings.GetValueOrDefault("QualityLevel", QualitySettings.GetQualityLevel().ToString()));
                QualitySettings.SetQualityLevel(qualityDropdown.value);
            }
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(qualityLevels));
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.onValueChanged.AddListener((value) => 
            { 
                QualitySettings.SetQualityLevel(value); 
                configData.settings["QualityLevel"] = value.ToString();
            });
        }

        if (windowModeDropdown != null)
        {
            if (configData != null)
            {
                windowModeDropdown.value = int.Parse(configData.settings.GetValueOrDefault("WindowMode", ((int)Screen.fullScreenMode).ToString()));
                Screen.fullScreenMode = (FullScreenMode)windowModeDropdown.value;
            }
            windowModeDropdown.onValueChanged.AddListener((value) => 
            { 
                Screen.fullScreenMode = (FullScreenMode)value; 
                configData.settings["WindowMode"] = value.ToString();
            });
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            Resolution[] resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener((value) => 
            { 
                Resolution selectedResolution = resolutions[value];
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
                configData.settings["Resolution"] = value.ToString();
            });
        }

        if (postProcessingAmountSlider != null)
        {
            if (configData != null)
            {
                postProcessingAmountSlider.value = float.Parse(configData.settings.GetValueOrDefault("PostProcessingAmount", "1.0"));
            }
            postProcessingAmountSlider.onValueChanged.AddListener((value) => 
            { 
                postProcessingAmountValueText.text = Mathf.RoundToInt(value * 100).ToString() + "%"; 
                configData.settings["PostProcessingAmount"] = value.ToString("F2");
            });
        }

        if (mouseSensitivitySlider != null)
        {
            if (configData != null)
            {
                mouseSensitivitySlider.value = float.Parse(configData.settings.GetValueOrDefault("MouseSensitivity", "1.0"));
                mouseSensitivityValueText.text = Mathf.RoundToInt(mouseSensitivitySlider.value * 100).ToString() + "%";
            }
            mouseSensitivitySlider.onValueChanged.AddListener((value) => 
            { 
                mouseSensitivityValueText.text = Mathf.RoundToInt(value * 100).ToString() + "%"; 
                configData.settings["MouseSensitivity"] = value.ToString("F2");
            });
        }

        if (SaveSystem.SaveSystem.QuerySavedGames().Count == 0)
        {
            if (NoSavedGamesText != null)
            {
                NoSavedGamesText.SetActive(true);
            }
        }
        else
        {
            if (NoSavedGamesText != null)
            {
                NoSavedGamesText.SetActive(false);
            }
        }

        HookButton(gameOptionsButton, OnGameOptionsButtonClicked);
        HookButton(audioOptionsButton, OnAudioOptionsButtonClicked);
        HookButton(videoOptionsButton, OnVideoOptionsButtonClicked);
        HookButton(inputOptionsButton, OnInputOptionsButtonClicked);
        HookButton(backToMainMenuButton, OnBackToMainMenuButtonClicked);
        HookButton(backButton, OnBackToMainMenuButton_LoadMenuClicked);

        mainMenuPanel.SetActive(true);
        loadGamePanel.SetActive(false);
        optionsPanel.SetActive(false);
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
        SaveSystem.SaveSystem.SaveGame(new SaveData(), "current", ShowSaveSuccessMessage);
        StartCoroutine(TransitionToMainScene());
    }

    private void OnLoadButtonClicked()
    {
        if (loadGamePanel != null)
        {
            mainMenuPanel.SetActive(false);
            loadGamePanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    private void OnOptionsButtonClicked()
    {
        if (optionsPanel != null)
        {
            mainMenuPanel.SetActive(false);
            loadGamePanel.SetActive(false);
            optionsPanel.SetActive(true);
        }
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnBackToMainMenuButtonClicked()
    {
        if (optionsPanel != null)
        {
            mainMenuPanel.SetActive(true);
            loadGamePanel.SetActive(false);
            optionsPanel.SetActive(false);
        }
        SaveSystem.SaveSystem.SaveConfiguration(configData, ShowSaveSuccessMessage);
    }

    private void OnBackToMainMenuButton_LoadMenuClicked()
    {
        if (loadGamePanel != null)
        {
            mainMenuPanel.SetActive(true);
            loadGamePanel.SetActive(false);
            optionsPanel.SetActive(false);
        }
    }

    private void ShowSaveSuccessMessage()
    {
        StartCoroutine(ShowTemporaryMessage("Settings saved successfully!", 2f));
    }

    private System.Collections.IEnumerator ShowTemporaryMessage(string message, float duration)
    {
        if (popupMessageText != null)
        {
            popupMessageText.gameObject.SetActive(true);
            popupMessageText.text = message;
            yield return new WaitForSeconds(duration);
            popupMessageText.text = "";
            popupMessageText.gameObject.SetActive(false);
        }
    }

    void OnPlanetDropdownValueChanged(int value)
    {
        if (EarthPlanet != null) EarthPlanet.SetActive(value == 0);
        if (MarsPlanet != null) MarsPlanet.SetActive(value == 1);
        if (MoonPlanet != null) MoonPlanet.SetActive(value == 2);
        configData.settings["SelectedPlanet"] = value.ToString();
    }

    void OnGameOptionsButtonClicked()
    {
        if (gameOptionsPanel != null)
        {
            gameOptionsPanel.SetActive(true);
            audioOptionsPanel.SetActive(false);
            videoOptionsPanel.SetActive(false);
            inputOptionsPanel.SetActive(false);
            optionsTitle.text = "GAME";
        }
    }

    void OnAudioOptionsButtonClicked()
    {
        if (audioOptionsPanel != null)
        {
            gameOptionsPanel.SetActive(false);
            audioOptionsPanel.SetActive(true);
            videoOptionsPanel.SetActive(false);
            inputOptionsPanel.SetActive(false);
            optionsTitle.text = "AUDIO";
        }
    }

    void OnVideoOptionsButtonClicked()
    {
        if (videoOptionsPanel != null)
        {
            gameOptionsPanel.SetActive(false);
            audioOptionsPanel.SetActive(false);
            videoOptionsPanel.SetActive(true);
            inputOptionsPanel.SetActive(false);
            optionsTitle.text = "VIDEO";
        }
    }

    void OnInputOptionsButtonClicked()
    {
        if (inputOptionsPanel != null)
        {
            gameOptionsPanel.SetActive(false);
            audioOptionsPanel.SetActive(false);
            videoOptionsPanel.SetActive(false);
            inputOptionsPanel.SetActive(true);
            optionsTitle.text = "INPUT";
        }
    }

    // Play the animation and load the main scene asynchronously (hopefully)
    // If the load is longer than the animation (!?), the loading slider will display
    // how much time is left.
    private System.Collections.IEnumerator TransitionToMainScene()
    {
        overlay.SetActive(true);
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

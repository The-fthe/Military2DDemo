using InGameAsset.Scripts;
using InGameAsset.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public Button RestartButton;
    public Button MainMenuButton;
    public Button SettingButton;
    public Button AudioButton;
    public Slider audioSlider;
    public CanvasGroup PauseMenuCG;
    public CanvasGroup AudioMenuCG;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] SoundManager _soundManager;
    public bool IsAudioPanelOpened { get; private set; }
    void Start()
    {
        SettingButton.onClick.AddListener(HandleResumeClicked);
        ResumeButton.onClick.AddListener(HandleResumeClicked);
        RestartButton.onClick.AddListener(HandleRestartClicked);
        MainMenuButton.onClick.AddListener(HandleBackToMainMenu);
        AudioButton.onClick.AddListener(ActivateAudioPanel);
        audioSlider.onValueChanged.AddListener(HandleSoundClicked);
        _levelManager = FindObjectOfType<LevelManager>();
        _soundManager = FindObjectOfType<SoundManager>();

    }

    public void CloseAudioMenu()
    {
        IsAudioPanelOpened = false;
        AudioMenuCG.alpha = 0;
        AudioMenuCG.interactable = false;
        AudioMenuCG.blocksRaycasts = false;
        ResumeButton.Select();        
    }
    void ActivateAudioPanel()
    {
        IsAudioPanelOpened = true;
        AudioMenuCG.alpha = 1;
        AudioMenuCG.interactable = true;
        AudioMenuCG.blocksRaycasts = true;
        audioSlider.Select();
    }
    void HandleResumeClicked()
    {
        _levelManager.TogglePause();
    }

    void HandleRestartClicked()
    {
        _levelManager.RestatGame();
    }

    void HandleSoundClicked(float level)
    {
        _soundManager.AdjustSoundVolume(level);
    }
    // void HandleBackMenu()
    // {
    //     _levelManager.LoadLevelAsyncAdditive("MainMenu");
    // }
    void HandleBackToMainMenu()
    {
        _levelManager.BackToMainMenu();
    }
}
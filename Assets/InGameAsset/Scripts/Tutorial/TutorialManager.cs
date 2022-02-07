using InGameAsset.Scripts;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialManager : ManagerManagement
{
    [SerializeField]VideoClip[] _videoClips;
    [SerializeField] TextMeshProUGUI _clipTitle;
    [SerializeField] string[] _textToDisplays;
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] Button nextButton;
    [SerializeField] Button _backToMainMenuButton;
    [SerializeField] int currentIndex=0;
    [SerializeField] BoolGameData hasTutorialShow;
    [SerializeField] CanvasGroup tutorialCanvasGroup;
    public bool HasTutorialShow => hasTutorialShow.RunTimeValue;
    bool _isOpen;

    void Start()
    {
        LevelManager.OnGameStateChanged.AddListener(HandleGameStateChanged);
        nextButton.onClick.AddListener(SkipToNextClip);
        _backToMainMenuButton.onClick.AddListener(()=>
        {
            UIManager.GetTutorialButton.Select();
            CloseTutorialMenu();
        });    

        CloseTutorialMenu();
        _clipTitle.SetText(_textToDisplays[currentIndex]);
        _videoPlayer.clip = _videoClips[currentIndex];
    }

    void HandleGameStateChanged(LevelManager.GameState currentState, LevelManager.GameState previousState)
    {
        if (currentState == LevelManager.GameState.Running && previousState == LevelManager.GameState.PreGame)
        {
            if (!hasTutorialShow.RunTimeValue)
            {
                UIManager.OnCountDownFinish(ShowTutorialMenu);
            }
        }
    }

    void ShowTutorialMenu()
    {
        tutorialCanvasGroup.alpha = 1;
        tutorialCanvasGroup.interactable = true;
        tutorialCanvasGroup.blocksRaycasts = true;
        hasTutorialShow.RunTimeValue = true;
        PlayerManager.DisablePlayerControl = true;
        nextButton.Select();
    }

    public void ToggleTutorialMenu()
     {
         if (_isOpen)
         {
             CloseTutorialMenu();
             _isOpen = false;
         }
         else
         {
             ShowTutorialMenu();
             _isOpen = true;
         }
     }
     void SkipToNextClip()
    {
        currentIndex++;
        if (currentIndex % _videoClips.Length ==0 )
            currentIndex = 0;
        _clipTitle.SetText(_textToDisplays[currentIndex]);
        _videoPlayer.clip = _videoClips[currentIndex];
    }
     public void CloseTutorialMenu()
     {
         tutorialCanvasGroup.alpha = 0;
         tutorialCanvasGroup.interactable = false;
         tutorialCanvasGroup.blocksRaycasts = false;
         UIManager.OnCountDownFinish(ShowTutorialMenu,true);
         PlayerManager.DisablePlayerControl = false;
     }
}

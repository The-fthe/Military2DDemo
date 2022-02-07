using System.Linq;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : ManagerManagement
{
    [SerializeField] HealthStatusDisplay _healthStatusDisplay;
    [SerializeField] BulletStatusDisplay _bulletStatusDisplay;
    [SerializeField] DisplayBulletLoading _displayBulletLoading;
    [SerializeField] BossHpDisplayPanel _bossHpDisplayPanel;
    [SerializeField] CountDown _countDown;
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] PauseMenu _pauseMenu;
    [SerializeField] GameOverMenu _gameOverMenu;
    [SerializeField] ScoreCounter _scoreCounter;
    [SerializeField] Button _ManualAutoTrigger;
    [SerializeField] Button[] _FireButtons;
    [SerializeField] Button _tutorialButton;
    [SerializeField] Button _aboutButton;
    [SerializeField] Button[] ClosePanel;
    [SerializeField] Image _triggerBtnImg;
    [SerializeField] Sprite[] _triggerSprites;
     
    public ScoreCounter GetScoreBar => _scoreCounter;
    public UnityEvent<bool> OnMainMenuFadeComplete;
    public Button GetTutorialButton => _tutorialButton;
    public BossHpDisplayPanel GetBossHpDisplayPanel => _bossHpDisplayPanel;
    public void ActivateMainMenuCamera(bool cameraActivate) => _mainMenu.SetDummyCameraActivate(cameraActivate);
    int _previousScore;
    void Start()
    {
        if (_mainMenu != null)
        {
            _mainMenu.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
            LevelManager.OnGameStateChanged.AddListener(HandleGameStateChanged);
            LevelManager.OnBackMenu.AddListener(UIManager.GetScoreBar.ResetScore);
            LevelManager.OnRestart.AddListener(ResetToPreviousScore);
            foreach (var button in ClosePanel)
            {
                button.onClick.AddListener(TutorialManager.ToggleTutorialMenu);
            }
            _tutorialButton.onClick.AddListener(TutorialManager.ToggleTutorialMenu);
        }
    }

    public void InitializeMainLevel()
    {
        _FireButtons =_FireButtons.Length <= 0?
            GameObject.FindGameObjectsWithTag("FireBtn").Where(i=>i.GetComponent<Button>())
                .Select(i=>i.GetComponent<Button>()).ToArray()
            : _FireButtons;
        _gameOverMenu ??= GetComponentInChildren<GameOverMenu>();
        _healthStatusDisplay ??= GetComponent<HealthStatusDisplay>();
        _bulletStatusDisplay ??= GetComponent<BulletStatusDisplay>();
        _displayBulletLoading ??= GetComponent<DisplayBulletLoading>();
        _bossHpDisplayPanel ??= GetComponentInChildren<BossHpDisplayPanel>();
        _scoreCounter ??= GetComponentInChildren<ScoreCounter>();
        _countDown ??= GetComponentInChildren<CountDown>();
        _countDown.Initialized();
        if (PlayerManager != null)
        {
            PlayerManager.OnPlayerActivated.RemoveListener(_healthStatusDisplay.Initialize);
            PlayerManager.OnPlayerActivated.RemoveListener(_bulletStatusDisplay.Initialize);
            PlayerManager.OnPlayerActivated.RemoveListener(_displayBulletLoading.Initialize);
            PlayerManager.OnPlayerActivated.RemoveListener(ActivatedScoreDisplay);
            
            PlayerManager.OnPlayerActivated.AddListener(_healthStatusDisplay.Initialize);
            PlayerManager.OnPlayerActivated.AddListener(_bulletStatusDisplay.Initialize);
            PlayerManager.OnPlayerActivated.AddListener(_displayBulletLoading.Initialize);
            PlayerManager.OnPlayerActivated.AddListener(ActivatedScoreDisplay);

            _scoreCounter.gameObject.SetActive(false);
        }
        else
        {
            Debugger.LogError($"UI manager cant find playerSpawner");
        }
    }

    void ActivatedScoreDisplay()
    {
        _scoreCounter.gameObject.SetActive(true);
    }

    public void InitializedBossLevel()
    {
        LevelManager.CurrentStageLevel.OnEnemiesHit(_bossHpDisplayPanel.Bind);
        LevelManager.CurrentStageLevel.OnEnemiesDead((() => { _bossHpDisplayPanel.Bind(); }));
       // _bossHpDisplayPanel.gameObject.SetActive(true);
    }

    public void OnCountDownFinish(UnityAction action,bool removeAction=false)
    {
        //_countDown.OnCountDownFinish.RemoveAllListeners();
        if(!removeAction)
            _countDown.OnCountDownFinish.AddListener(action);
        else
            _countDown.OnCountDownFinish.RemoveListener(action);
    }
    
    public void ResetToPreviousScore()
    {
        GetScoreBar.ScoreNum = _previousScore;
        GetScoreBar.ShowScoreText();
    }
    void HandleGameStateChanged(LevelManager.GameState currentState, LevelManager.GameState previousState)
    {
        if (currentState == LevelManager.GameState.Running && previousState == LevelManager.GameState.Pause)
        {
            _pauseMenu.gameObject.SetActive(false);
        }

        if (currentState == LevelManager.GameState.Running && previousState == LevelManager.GameState.PreGame)
        {
            InitializeMainLevel();
            _previousScore = GetScoreBar.ScoreNum;
            _pauseMenu.gameObject.SetActive(false);
            _gameOverMenu.gameObject.SetActive(false);
            _scoreCounter.gameObject.SetActive(true);
            _bossHpDisplayPanel.gameObject.SetActive(false);
            _ManualAutoTrigger.onClick.AddListener(() =>
            {
                PlayerManager.TriggerAutoManualButton();
                _triggerBtnImg.sprite = _triggerBtnImg.sprite == _triggerSprites[0]
                    ? _triggerSprites[1]
                    : _triggerSprites[0];
            });
        }

        if (currentState == LevelManager.GameState.Pause && previousState == LevelManager.GameState.Running)
        {
            if (LevelManager.IsWin || LevelManager.IsLost)
            {
                _gameOverMenu.gameObject.SetActive(true);
                _gameOverMenu.RestartButton.Select();
            }
            else if (!LevelManager.IsWin || !LevelManager.IsLost)
            {
                _pauseMenu.gameObject.SetActive(true);
                _pauseMenu.ResumeButton.Select();
            }
        }

        if (currentState == LevelManager.GameState.PreGame && previousState == LevelManager.GameState.Pause)
        {
        }
    }

    public void ActivatedBossPanel(IntGameData bossHpData)
    {
        _bossHpDisplayPanel.gameObject.SetActive(true);
        _bossHpDisplayPanel.SetHpData(bossHpData);
        _bossHpDisplayPanel.Bind();
    }

    public void AddClickEventToFireButton(UnityAction action)
    {
        foreach (var fireButton in _FireButtons)
        {
            fireButton.onClick.AddListener(action);
        }
    }

    public void CloseSecondLayerMenu()
    {
        if (_pauseMenu.IsAudioPanelOpened)
        {
            _pauseMenu.CloseAudioMenu();
        }
        else if (TutorialManager.HasTutorialShow)
        {
            TutorialManager.CloseTutorialMenu();
        }
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        OnMainMenuFadeComplete?.Invoke(fadeOut);
    }

    void OnDestroy()
    {
        Debug.Log("UI manager is destroyed");
    }
}
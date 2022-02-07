using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts.army;
using InGameAsset.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace InGameAsset.Scripts.Managers
{
    public class LevelManager : ManagerManagement
    {
        [SerializeField] GameStage _gameStageToLoad = GameStage.Level1;
        [SerializeField] GameState _currentGameState=GameState.PreGame;
        public StageLevel CurrentStageLevel;
        public UnityEvent<GameState, GameState> OnGameStateChanged;
        public UnityEvent OnBackMenu;
        public UnityEvent OnRestart;
        public UnityEvent OnStageLevelStart;
        public bool IsWin;
        public bool IsLost;
        public BoolGameData _canEnemyStartShoot;
        List<AsyncOperation> _loadOperations;
        string _currentLevelName = string.Empty;
        CancellationTokenSource _cts;
        bool isGameStart;
        void Start()
        {
            _cts = new CancellationTokenSource();
            _loadOperations = new List<AsyncOperation>();
            UIManager.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
        }
        public void StartGame()
        {
            if (isGameStart) return;
            if(_currentGameState != GameState.PreGame) return;
            LoadLevelAsyncAdditive(_gameStageToLoad.ToString());
            SoundManger.PlayStageEnterMusic();
            isGameStart = true;
        }

        public void TransferToAnotherStage(string stageToSwitch,string stageToUnload)
        {
           // if(_currentGameState != GameState.PreGame) return;
            LevelManager.UpdateState(GameState.PreGame);
            UnloadLevelAsync(stageToUnload);
            LoadLevelAsyncAdditive(stageToSwitch);
            SoundManger.PlayStageEnterMusic();
        }
        
        void HandleMainMenuFadeComplete(bool fadeOut)
        {
            if(!fadeOut)
                UnloadLevelAsync(_currentLevelName);
        }

        public void LoadLevelAsyncAdditive(string levelName)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            if (ao == null)
            {
                Debug.LogError($"Unable to load level {levelName}");
                return;
            }
            ao.completed += OnLoadOperationComplete;
            _loadOperations.Add(ao);
            _currentLevelName = levelName;
        }
        void OnLoadOperationComplete(AsyncOperation ao)
        {
            if (_loadOperations.Contains(ao))
            {
                _loadOperations.Remove(ao);
         
                if (_loadOperations.Count == 0)
                {
                    UpdateState(GameState.Running);
                }
            }
        }
        public void UnloadLevelAsync(string levelName)
        {
            AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
            if (ao == null)
            {
                Debug.LogError($"Unable to Unload level {levelName}");
                return;
            }
            ao.completed += UnLoadOperationComplete;
        }
        void UpdateState(GameState gamestate)
        {
            GameState previousGameState = _currentGameState;
            _currentGameState = gamestate;
            switch (_currentGameState)
            {
                case GameState.PreGame:
                    Time.timeScale = 1f;
                    break;
                case  GameState.Running:
                    Time.timeScale = 1f;
                    break;
                case GameState.Pause:
                    Time.timeScale = 0f;
                    break;
            }
            OnGameStateChanged.Invoke(_currentGameState,previousGameState); //dispatch message
        }

        public void SetStageLevel(StageLevel stageLevel)
        {
            if (CurrentStageLevel != null)
            {
                CurrentStageLevel.OnGameStart.RemoveAllListeners();
                CurrentStageLevel.OnBossDefeated.RemoveAllListeners();
            }
            CurrentStageLevel = stageLevel;
            if (CurrentStageLevel != null && !CurrentStageLevel.IsGameStateToWin)
            {
                CurrentStageLevel.OnGameStart.AddListener(SoundManger.HandleStageLevelBGMusic);
                CurrentStageLevel.OnBossDefeated.AddListener(SoundManger.PlayWinMusic);
            }
            else if (CurrentStageLevel != null && CurrentStageLevel.IsGameStateToWin)
            {
                CurrentStageLevel.OnGameStart.AddListener(SoundManger.HandleStageLevelBGMusic);
                CurrentStageLevel.OnBossDefeated.AddListener(SoundManger.PlayWinMusic);
            }
            else
            {
                Debug.LogWarning("CantFind CurrentStageLevel");
            }
        }

        void UnLoadOperationComplete(AsyncOperation obj)
        {
            Debug.Log("Unload Complete");
        }
        
        public void TogglePause()
        {
            if (TutorialManager.HasTutorialShow)
            {
                TutorialManager.CloseTutorialMenu();
            }
            UpdateState(_currentGameState == GameState.Running ? GameState.Pause : GameState.Running);
        }

        public void GameOverLostTrigger()
        {
            IsLost = true;
            HandleGameOverTrigger(_cts.Token).Forget();
            SoundManger.PlayLostMusic();
            UIManager.GetScoreBar.ShowScoreText();
        }
        public void GameOverWinTrigger()
        {
            IsWin = true;
            HandleGameOverTrigger(_cts.Token).Forget();
            UIManager.GetScoreBar.ShowScoreText();
        }

        async UniTask HandleGameOverTrigger(CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: ct); 
            UpdateState(_currentGameState == GameState.Running ? GameState.Pause : GameState.Running);
        }
        
        public void RestatGame()
        {
            if (!isGameStart) return;
            IsWin = false;
            IsLost = false;
            OnRestart?.Invoke();
            SoundManger.PlayStageEnterMusic();
            UpdateState(GameState.PreGame);
            UnloadLevelAsync(_currentLevelName);
            LoadLevelAsyncAdditive(_currentLevelName);
            isGameStart = true;
        }
        public void BackToMainMenu()
        {    
            IsWin = false;
            IsLost = false;
            isGameStart = false; 
            // UnloadLevelAsync(_currentLevelName);
            // PlayerManager.DestroyPlayer();
            // SoundManger.PlayMainMenuMusic();
            // UIManager.ActivateMainMenuCamera(true);
            // UpdateState(GameState.PreGame);
            // OnBackMenu?.Invoke();
            SceneManager.LoadScene("MainMenu");
        }

        public void HandleReturnButtonTrigger()
        {
            Debug.Log("Handle Exit button is trigged");
            UIManager.CloseSecondLayerMenu();
        }
        void OnDestroy()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts.Dispose();
        }

        [Serializable]
        public enum GameState
        {
            PreGame,Running,Pause
        }
        [Serializable]
        public enum GameStage
        {
            MainMenu,Level1,Level2,Level3,Level4
        }
    }
}





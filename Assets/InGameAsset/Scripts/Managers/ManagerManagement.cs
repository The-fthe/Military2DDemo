using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InGameAsset.Scripts
{
    public class ManagerManagement : MonoBehaviour
    {
        protected CachedComponent<GameManager> _gameManager = new CachedComponent<GameManager>();

        public GameManager GameManager
        {
            get => _gameManager.instance(this);
        }
        protected CachedComponent<LevelManager> _levelManager = new CachedComponent<LevelManager>();

        public LevelManager LevelManager
        {
            get => _levelManager.instance(this);
        }
        public CachedComponent<UIManager> _UIManager = new CachedComponent<UIManager>();
        public UIManager UIManager
        {
            get => _UIManager.instance(this);
        }
        
        public CachedComponent<GamePersistance> _GamePersistance = new CachedComponent<GamePersistance>();
        public GamePersistance GamePersistance
        {
            get => _GamePersistance.instance(this);
        }
        public CachedComponent<PlayerManager> _PlayerManager = new CachedComponent<PlayerManager>();
        public PlayerManager PlayerManager
        {
            get => _PlayerManager.instance(this);
        }
        public CachedComponent<CameraManager> _cameraManager = new CachedComponent<CameraManager>();
        public CameraManager CameraManager
        {
            get => _cameraManager.instance(this);
        }

        public CachedComponent<SoundManager> _soundManager = new CachedComponent<SoundManager>();
        public SoundManager SoundManger
        {
            get => _soundManager.instance(this);
        }
        
        public CachedComponent<TutorialManager> _tutorialManager = new CachedComponent<TutorialManager>();
        public TutorialManager TutorialManager
        {
            get => _tutorialManager.instance(this);
        }

        // void OnEnable()
        // {
        //     SceneManager.sceneLoaded += OnLevelFinishedLoading;
        // }
        //
        // void OnDisable()
        // {
        //     SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        // }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            // _gameManager.clear();
            // _UIManager.clear();
            // _levelManager.clear();
            // _GamePersistance.clear();
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts;
using InGameAsset.Scripts.army;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class StageLevel : MonoBehaviour
{
    public LevelManager.GameStage _nextStageTransfer;
    public LevelManager.GameStage _currentGameStage;
    [SerializeField] NextStageTrigger.ArrowFaceDir _arrowFace;
    [SerializeField] Camera _mainCamera;
    [SerializeField] CinemachineVirtualCamera _virtualCamera;
    [SerializeField] ScoreCounter _scoreCounter;
    [Space(10)] [SerializeField] RoadBlocker _roadBlocker;
    [SerializeField] Transform _nextStageSpawnPoint;
    [SerializeField] PlayerSpawnPoint[] _spawnPoint;
    [SerializeField] GameObject[] _RewardGOs;
    [SerializeField] BossUnit[] _bossUnits;
    [Space(10)] [SerializeField] LevelManager _levelManager;
    [SerializeField] UIManager _uiManager;
    [SerializeField] PlayerManager _playerManager;
    [SerializeField] GameObject _nextStageTriggerGO;
    [SerializeField] Damageable[] _damageableEnemies;
    [SerializeField] GameEvent OnGameOverEvent;
    [SerializeField] GameEvent OnNextstageEvent;
    [SerializeField] GameEvent ChangeBackCamEvent;
    [SerializeField] GameEvent OnGameStartEvent;
    [SerializeField] PlayableDirector _playableDirector;
    [SerializeField] PlayableAsset[] _playableAssets;
    [SerializeField] bool IsGameStartTrigger = false;
    [SerializeField] bool IsNextStageTrigger;
    public AttachmentBuff[] PlayerAttachmentBuff;
    public bool IsGameStateToWin;
    public UnityEvent OnGameStart;
    public UnityEvent OnBossDefeated;
    CancellationTokenSource _cts;

    void Start()
    {
        if (_RewardGOs.Length > 0 && _RewardGOs != null)
        {
            foreach (var rewardGO in _RewardGOs)
                rewardGO.SetActive(false);
        }

        _nextStageTriggerGO = Resources.Load(Const.PATH_NEXT_STAGE_TRANSFER_GATE) as GameObject;
        _nextStageTriggerGO.GetComponent<NextStageTrigger>().SetArrowFaceDir = _arrowFace;
        InitializeStageLevel();
        _roadBlocker ??= FindObjectOfType<RoadBlocker>();
        _cts = new CancellationTokenSource();
        _roadBlocker.OnRoadBlockerCalled.AddListener(OnGameStartEvent.Invoke);
    }

    #region PublicFunc
    public bool IsEnemiesHpLessThan0()
    {
        return _bossUnits[0].GetBossHp().RunTimeValue <= 0;
    }

    public void OnEnemiesDead(UnityAction listener)
    {
        foreach (var damageableEnemy in _damageableEnemies)
        {
            if (!damageableEnemy.IsPlayer)
                damageableEnemy.OnDead.AddListener(listener);
        }
    }
    public void OnEnemiesHit(UnityAction action)
    {
        foreach (var damageableEnemy in _damageableEnemies)
        {
            if (!damageableEnemy.IsPlayer)
                damageableEnemy.OnHit.AddListener(action);
        }
    }

    #endregion

    #region BossInitializeFunc
    public  void LevelIntroduction()
    {
        if (IsGameStartTrigger) return;
        Debug.Log("level introduction is trigger");
        StartPlayableClip(0);
    }

    public void StartEnemiesAction()
    {
        _levelManager._canEnemyStartShoot.RunTimeValue = true;
        OnGameStart?.Invoke();
        UpdateBossHp();
    }
    public void ChangeCameraSize(int size)
    {
        _virtualCamera.m_Lens.OrthographicSize = size;
    }

    public void ChangePlayerSpawnPoint()
    {
        _playerManager._spawnPointPos = _spawnPoint[1].Pos;
    }

    #endregion

    void InitializeStageLevel()
    {
        var gm = FindObjectOfType<GameManager>();
        _levelManager = gm.InstancedSystemPrefabs
            .Select(i => i.GetComponent<LevelManager>()).FirstOrDefault(i => i != null);
        _levelManager.SetStageLevel(this);

        _playerManager = gm.InstancedSystemPrefabs
            .Select(i => i.GetComponent<PlayerManager>()).FirstOrDefault(i => i != null);
        _uiManager = gm.InstancedSystemPrefabs.Where(i => i.GetComponent<UIManager>())
            .Select(i => i.GetComponent<UIManager>()).FirstOrDefault(i => i != null);
        _bossUnits = FindObjectsOfType<BossUnit>();
        _damageableEnemies = FindObjectsOfType<Damageable>().Where(i => !i.IsPlayer).ToArray();
        SetCameraActivate(true);
        _scoreCounter ??= FindObjectOfType<ScoreCounter>();
        _playerManager._spawnPointPos = _spawnPoint[0].Pos;
        OnEnemiesHit(UpdateScore);
        OnEnemiesDead(UpdateScore);
        OnEnemiesDead(HandleEndGame);
        _uiManager.InitializedBossLevel();
    }
    void HandleEndGame()
    {
        if (IsEnemiesHpLessThan0() && !IsGameStateToWin)
        {
            Debug.Log("Boss defeat next stage transfer is trigger");
            OnBossDefeated?.Invoke();
            ActivateNextStageTransferPoint();
        }
        else if (IsEnemiesHpLessThan0() && IsGameStateToWin)
        {
            OnGameOverEvent.Invoke();
            OnBossDefeated?.Invoke();
            _WaitToChangeBackCam(_cts.Token).Forget();
            if (IsGameStateToWin)
            {
                Debug.Log("Damagable gameover handler is trigger");
                _levelManager.GameOverWinTrigger();
            }
        }
    }

    void ActivateNextStageTransferPoint()
    {
        if (IsNextStageTrigger) return;
        OnBossDefeated?.Invoke();
        OnNextstageEvent.Invoke();
        StartPlayableClip(1);
        Debug.Log("NextStage Is instantiated");
        var checkOtherNextStrgierAvailable = FindObjectOfType<NextStageTrigger>();
        if (checkOtherNextStrgierAvailable == null)
        {
            var spawnPoint = Instantiate(_nextStageTriggerGO, _nextStageSpawnPoint.position,
                _nextStageSpawnPoint.rotation);
            spawnPoint.transform.parent = _nextStageSpawnPoint.transform;
            var nextStageTrigger = spawnPoint.GetComponent<NextStageTrigger>();
            nextStageTrigger.GetLevelManager = _levelManager;
        }
        IsNextStageTrigger = true;
    }

    void StartPlayableClip(int clipToPlay)
    {
        Debug.Log("star playable clip is trigger");
        _playableDirector.Stop();
        _playableDirector.playableAsset = _playableAssets[clipToPlay];
        _playableDirector.Play();
        _WaitToChangeBackCam(_cts.Token).Forget();
    }

    async UniTask _WaitToChangeBackCam(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_playableDirector.duration), false, cancellationToken: ct);
        ChangeBackCamEvent.Invoke();
        Debug.Log("wait to change back cam is trigger");
        IsGameStartTrigger = true;
    }

    void UpdateBossHp()
    {
        _uiManager.ActivatedBossPanel(_bossUnits.FirstOrDefault(i => i != null).GetBossHp());
        _uiManager.GetBossHpDisplayPanel.Bind(_currentGameStage);
    }

    void UpdateScore() => _scoreCounter.EveryScoreIncrease();

    void SetCameraActivate(bool active)
    {
        _mainCamera.gameObject.SetActive(active);
    }

    void OnDestroy()
    {
        _playerManager.OnPlayerActivated.RemoveListener(UpdateBossHp);
        _playerManager._spawnPointPos = _spawnPoint[0].Pos;
        if (_cts == null) return;
        if (!_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts;
using InGameAsset.Scripts.army;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerManager : ManagerManagement
{
    public bool IsPlayerDeath { get; set; }
    public GameObject _playerPrefab;
    public GameObject GetPlayerGO => _player;
    public HostAttachment PlayerHostAttachment;
    public Vector3 _spawnPointPos;
    [SerializeField] List<BulletData> _bulletDatas;
    [SerializeField] TankSpawner _tankSpawner;
    [SerializeField] GameObject _player;
    [SerializeField] IntGameData _playerLife;
    [SerializeField] IntGameData _playerHealth;
    [SerializeField] BoolGameData _canPlayerSpawn;
    [SerializeField] BoolGameData _disablePlayerControl;
    [SerializeField] BoolGameData _canPlayerShoot;
    [SerializeField] BoolGameData _isButtonBeingDrag;
    [SerializeField] bool _isIntialized;
    [SerializeField] bool isGameOverTrigger;
    public ITurret Playerturret { get; private set; }
    [SerializeField] AutoTargetAimerTurret _AutoManualTrigger;

    CancellationTokenSource _cts;
    public UnityEvent OnPlayerActivated;

    #region pubilc method
    
    public bool DisablePlayerControl
    {
        get => _disablePlayerControl.RunTimeValue;
        set => _disablePlayerControl.RunTimeValue = value;
    }

    public void TriggerAutoManualButton()
    {
        _AutoManualTrigger.TriggerAutoManualTrigger();
    }

    public void ResetSpawnPosition(Vector3 spawnPos)
    {
        _player.transform.position = spawnPos;
        _player.GetComponentInChildren<Rigidbody2D>().transform.position = spawnPos;
    }

    public void SetPlayerToDeath()
    {
        IsPlayerDeath = true;
    }

    public void DestroyPlayer()
    {
        Helpers.DeleteChildren(_player.transform);
        Destroy(_player);
    }

    #endregion

    void Start()
    {
        Input.backButtonLeavesApp = true;
        _cts = new CancellationTokenSource();
        _tankSpawner ??= GetComponent<TankSpawner>();
        LevelManager.OnGameStateChanged.AddListener(HandleGameStateChange);
    }

    void HandleGameStateChange(LevelManager.GameState currentState, LevelManager.GameState previousState)
    {
        if (previousState == LevelManager.GameState.PreGame && currentState == LevelManager.GameState.Running)
        {
            Debug.Log("player initialize is trigger");
            _isIntialized = false;
            Initialized();
            ResetBulletNum();
            ResetPlayerLifeAndHP();
            OnPlayerActivated.AddListener(InitializeStageLevelPlayerBuff);
        }
    }

    void InitializeStageLevelPlayerBuff()
    {
        foreach (var buff in LevelManager.CurrentStageLevel.PlayerAttachmentBuff)
        {
            PlayerHostAttachment.SetAllAttachmentStateToActive(buff);
        }
    }

    void ActivatePlayerAttachment(AttachmentBuff attachment)
    {
        _player.GetComponentInChildren<HostAttachment>().SetAllAttachmentStateToActive(attachment);
    }

    void ResetPlayerLifeAndHP()
    {
        _playerHealth.RunTimeValue = _playerHealth.Value;
        _playerLife.RunTimeValue = _playerLife.Value;
    }

    void ResetBulletNum()
    {
        foreach (var bulletData in _bulletDatas)
        {
            bulletData.BulletNum.RunTimeValue = bulletData.BulletNum.Value;
        }
    }

    void Update()
    {
        if (!_isIntialized)
            return;
        if (_playerLife == null) return;
        if (isPlayerAlive(_playerLife.RunTimeValue) && IsPlayerDeath)
        {
            _tankSpawner.SpawnUnitAtSpawnPoint(_player, _spawnPointPos);
            IsPlayerDeath = false;
        }
        else if (IsPlayerDeath && !isPlayerAlive(_playerLife.RunTimeValue) && !isGameOverTrigger)
        {
            Debug.Log("GameOver event is invoke");
            isGameOverTrigger = true;
            LevelManager.GameOverLostTrigger();
        }
    }

    void Initialized()
    {
        _isIntialized = false;
        isGameOverTrigger = false;
        IsPlayerDeath = false;
        if (_player == null)
            CreatePlayer();
        ListenToPlayerDeath();
        GetCurrentPlayerHealthData();
        _player.SetActive(false);

        UIManager.OnCountDownFinish(HandlePlayerActivation);
        Playerturret = _player.GetComponentInChildren<TurretShootFromBarrelWithOwnUpdate>();
        _AutoManualTrigger = _player.GetComponentInChildren<AutoTargetAimerTurret>();
        PlayerHostAttachment = _player.GetComponentInChildren<HostAttachment>();
        UIManager.AddClickEventToFireButton(() =>
        {
            if (_isButtonBeingDrag.RunTimeValue)
                _isButtonBeingDrag.RunTimeValue = false;
            else
                Playerturret.Shoot();
        });
        var playerBullets = _player.GetComponentInChildren<Turret>().GetBulletData;
        _bulletDatas.Add(playerBullets);
        _isIntialized = true;
    }

    void HandlePlayerActivation()
    {
        _canPlayerSpawn.RunTimeValue = true;
        _canPlayerShoot.RunTimeValue = true;
        if (!_player.gameObject.activeSelf)
            _player.gameObject.SetActive(true);
        ResetSpawnPosition(_spawnPointPos);
        OnPlayerActivated?.Invoke();
    }

    bool isPlayerAlive(int playerLife)
    {
        if (playerLife > 1)
        {
            return true;
        }

        return false;
    }

    void CreatePlayer()
    {
        _player = Instantiate(_playerPrefab);
    }

    void GetCurrentPlayerHealthData()
    {
        _playerHealth = _player.GetComponentInChildren<UnitStatus>()._currentHealthData;
    }

    void ListenToPlayerDeath()
    {
        _playerLife = _player.GetComponentInChildren<UnitStatus>().Life;
        _player.GetComponentInChildren<Damageable>().OnDead.AddListener(SetPlayerToDeath);
    }

    void OnDestroy()
    {
        if (_cts == null) return;
        if (!_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
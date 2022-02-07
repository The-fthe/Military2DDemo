using System;
using Cinemachine;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : ManagerManagement
{
    [SerializeField] CinemachineVirtualCamera[] _cameras;
    [SerializeField] CinemachineVirtualCamera _currentCamera;
    void Start()
    {
        LevelManager.OnGameStateChanged.AddListener(HandleOnStageChange);
    }

    void HandleOnStageChange(LevelManager.GameState currentStage, LevelManager.GameState previousStage)
    {
        if (previousStage == LevelManager.GameState.PreGame && currentStage == LevelManager.GameState.Running)
        {
            _cameras = null;
            PlayerManager.OnPlayerActivated.AddListener(Initialize);
        }
    }

    void Initialize()
    {
        _cameras = null;
        _cameras = FindObjectsOfType<CinemachineVirtualCamera>();
        _currentCamera = _cameras[0];
        for (int i = 0; i < _cameras.Length; i++)
        {
            if(_cameras[i].Priority >_currentCamera.Priority )
                _currentCamera = _cameras[i];
        }
        var playerTrans =FindObjectOfType<PlayerInputEvent>().gameObject
            .GetComponentInChildren<Damageable>().transform;
        _currentCamera.LookAt = playerTrans;
        _currentCamera.Follow = playerTrans;
    }

    public void CheckAsignCameraPriority()
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            if(_cameras[i].Priority >_currentCamera.Priority )
                _currentCamera = _cameras[i];
        }
        var playerTrans =FindObjectOfType<PlayerInputEvent>().gameObject
            .GetComponentInChildren<Damageable>().transform;
        _currentCamera.LookAt = playerTrans;
        _currentCamera.Follow = playerTrans;
        Debug.Log("Check assign is being called");
    }
}

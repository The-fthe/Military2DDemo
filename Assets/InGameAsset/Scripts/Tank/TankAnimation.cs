using System;
using System.Linq;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankAnimation : MonoBehaviour
{
    public bool ContinueCurrentAnim = false;
    [SerializeField] Animator[] _animators;
    string[] _currentStates;
    PlayerInputEvent _playerInputEvent;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (_animators.Length <= 0)
        {
            _animators = GetComponentsInChildren<Animator>().Where(i => i.gameObject.CompareTag("Player")).ToArray();
            _currentStates = new string[_animators.Length];
        }
        _currentStates = new string[_animators.Length];

        _playerInputEvent ??= GetComponent<PlayerInputEvent>();
        _playerInputEvent.OnMoveBody.AddListener(MovementAnimation);
    }

    public void ChangeAnimState(string newState)
    {
        if (ContinueCurrentAnim) return;
        for (int i = 0; i < _animators.Length; i++)
        {
            if (_currentStates[i] == newState) continue;
            _animators[i].Play(newState);
            _currentStates[i] = newState;
        }
    }


    public float GetCurrentAnimDelayTime()
    {
        return _animators[0].GetCurrentAnimatorStateInfo(0).length;
    }

    public float GetCurrentAnimDelayTime(int index)
    {
        return _animators[index].GetCurrentAnimatorStateInfo(0).length;
    }

    public void MovementAnimation(Vector2 dirVec)
    {
        if (dirVec == Vector2.zero)
        {
            ChangeAnimState(Const.PLAYER_IDLE);
        }
        else
            ChangeAnimState(Const.PLAYER_MOVE);
    }
    
    void OnDestroy()
    {
        if (_playerInputEvent != null)
            _playerInputEvent.OnMoveBody.RemoveListener(MovementAnimation);
    }
}
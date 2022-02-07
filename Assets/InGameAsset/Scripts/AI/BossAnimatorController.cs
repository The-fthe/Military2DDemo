using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimatorController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    string _currentState;
    void Start()
    {
        _animator ??= GetComponent<Animator>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    public void ChangeAnimationState(string newState, float animSpeed= 1)
    {
        if (_animator == null) return;
        if (_currentState == newState) return;
        _animator.speed =animSpeed;
        _animator.Play(newState);
        _currentState = newState;
    }

    public float GetCurrentAnimDelayTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

}

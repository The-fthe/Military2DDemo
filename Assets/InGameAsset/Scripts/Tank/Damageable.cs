using System;
using System.Collections;
using System.Collections.Generic;
using InGameAsset.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace InGameAsset.Scripts
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] UnitStatus _unitStatus;
        [SerializeField] HandlingHit _hitHandling;
        public bool IsPlayer => gameObject.layer == LayerMask.NameToLayer("Player");
        public UnityEvent OnHit, OnDead;
        public UnityEvent <IntGameData,IntGameData> OnHealthAndLifeChange;

        public void SetHitHandling(HandlingHit hitHandler)
        {
            _hitHandling = hitHandler;
        }

        void Start()
        {
            if (_unitStatus is null) _unitStatus = GetComponentInParent<UnitStatus>();
            _unitStatus.WasInvinsibleFinish = true;
            OnHealthAndLifeChange?.Invoke(_unitStatus._currentHealthData,_unitStatus.Life);
        }

        public void Hit(int damage)
        {
            if (_unitStatus.WasInvinsibleFinish)
            {
                var d = damage;
                if (_hitHandling != null && _hitHandling.GetType() != typeof(ShieldHandleHit))
                    d = _hitHandling.HandleHit(damage);
                _unitStatus.Health -= d;
                OnHitUpdate();
                OnHit?.Invoke();
            }
            if (_unitStatus.Health <= 0)
                OnDead?.Invoke();
            _unitStatus.Health = Mathf.Clamp(_unitStatus.Health, 0, _unitStatus.MaxHealth.Value);
        }

        // public void Hit(int damage)
        // {
        //     //Debug.Log($"unit status is {_unitStatus.WasInvinsibleFinish}");
        //     if (!_unitStatus.WasInvinsibleFinish) return;
        //     if (_unitStatus.WasInvinsibleFinish)
        //     {
        //         _unitStatus.Health -= damage;
        //         OnHitUpdate();
        //         if (_unitStatus.Health <= 0)
        //         {
        //             OnDead?.Invoke();
        //         }
        //         else
        //         {
        //             OnHit?.Invoke();
        //         }
        //     }
        //     _unitStatus.Health = Mathf.Clamp(_unitStatus.Health, 0, _unitStatus.MaxHealth.Value);
        // }
        void OnHitUpdate()
        {
            OnHitFinish();
        }

        void OnHitFinish()
        {
            if (IsPlayer)
            {
                OnHealthAndLifeChange?.Invoke(_unitStatus._currentHealthData, _unitStatus.Life);
                if (_hitHandling != null && _hitHandling.GetType() == typeof(ShieldHandleHit))
                    StartCoroutine(InvisibleCountDown(_unitStatus.TimeToInvisible));
            }
        }

        public void OnDisable()
        {
            if (!Application.isPlaying)
            {
                OnDead?.Invoke();
                StopAllCoroutines();                
            }
        }

        IEnumerator InvisibleCountDown(float timeToInvisible)
        {
            _unitStatus.WasInvinsibleFinish = false;
            yield return new WaitForSeconds(timeToInvisible);
            _unitStatus.WasInvinsibleFinish = true;
        }
    }
}
using System;
using System.Collections;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;

namespace InGameAsset.Scripts
{
    public class ShowInvisibleEffect : MonoBehaviour
    {
        [SerializeField] UnitStatus _unitStatus;
        [SerializeField] TankAnimation _tankAnimation;
        float _invisibleTime;
        
        public void Initialize(UnitStatus unitStatus, TankAnimation animation,Damageable damageable)
        {
            _unitStatus = unitStatus;
            _invisibleTime = _unitStatus.TimeToInvisible;
            _tankAnimation = animation;
            _tankAnimation.ContinueCurrentAnim = false;
            damageable.OnHit
                .AddListener(UpdateInvisibleAnimBool);
        }

        public void UpdateInvisibleAnimBool()
        {
            if (_unitStatus.WasInvinsibleFinish) return;
            _tankAnimation.ChangeAnimState(Const.PLAYER_INVISIBLE);
            _tankAnimation.ContinueCurrentAnim = true;
            StartCoroutine(CountDownInvisibleTime());
        }

        IEnumerator CountDownInvisibleTime()
        {
            yield return new WaitForSeconds(_invisibleTime);
            _tankAnimation.ContinueCurrentAnim = false;
            _tankAnimation.ChangeAnimState(Const.PLAYER_MOVE);
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
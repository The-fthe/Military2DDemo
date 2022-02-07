using System.Collections;
using System.Collections.Generic;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Tank.Turrets;
using InGameAsset.Scripts.Util;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] SimpleTurretSingleBarrel Turret;
    public Transform BarrelTrans => Turret.TurretBarrelTran.transform;
    string _currentState;
    bool _isFinsihPlaying = true;

    public void SetTurretBulletData(BulletData bulletData,ObjectPool bulletPool) => Turret.Initialized(bulletData, bulletPool);
    void Awake()
    {
        _animator = GetComponent<Animator>();
        Turret ??= GetComponent<SimpleTurretSingleBarrel>();
    }

    public void HandleIdle()
    {
        _animator.StopPlayback();
        _animator.Play(Const.BOSS_IDLE);
    }

    public void HandleShoot(int bulletPerSecond)
    {
        if (_isFinsihPlaying)
        {
            _animator.enabled = true;
            _animator.speed = bulletPerSecond;
            _animator.Play(Const.TURRET_SHORT_FIRE);
        }
    }
    public void HandleHideMode(int speed=1)
    {
        _animator.enabled = true;
        _animator.speed =speed;
        _animator.Play(Const.BOSS_HIDE_MODE);
    }
    public void HandleShowMode(int speed= 1)
    {
        _animator.enabled = true;
        _animator.speed =speed;
        _animator.Play(Const.BOSS_SHOW_MODE);
    }

    public void HandleDieMode(int speed = 1)
    {
        _animator.enabled = true;
        _animator.speed = speed;
        _animator.Play(Const.BOSS_DIE_MODE);
    }
    
    public void StartShooting(int isFinish)
    {
        if(isFinish <=0)
            Turret.Shoot();
        _isFinsihPlaying = isFinish >= 1;
    }
    public void StartShowMode(int isFinish)
    {
        _isFinsihPlaying = isFinish >= 1;
    }
    public void StartHideMode(int isFinish)
    {
        _isFinsihPlaying= isFinish >= 1;
    }

    public void CheckIsAnimationFinish(int isFinish)
    {
        _isFinsihPlaying = isFinish >= 1;
    }
}

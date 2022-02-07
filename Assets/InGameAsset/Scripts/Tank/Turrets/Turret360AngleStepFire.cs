using System;
using System.Linq;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

public class Turret360AngleStepFire : Turret,ITurret
{
    [SerializeField] BoolGameData EnableShooting;
    [SerializeField] float _angleStep = 15f;
    bool _recoilCompleted;
    float _currentRotateAngle;
    float _currentDelay;
    [SerializeField] UnityEvent OnShoot;

    public  void Shoot()
    {
        ShootAtEveryAngleRoundStep();
    }

    void ShootAtEveryAngleRoundStepNoReload()
    {
        for (_currentRotateAngle = 0; _currentRotateAngle < 360; _currentRotateAngle += _angleStep)
        {
            FireOnAngle(_currentRotateAngle);
        }
        _currentRotateAngle = _currentRotateAngle >= 360 ? 0 : _currentRotateAngle;
        OnShoot?.Invoke();
    }

    void ShootAtEveryAngleRoundStep()
    {
        if (_recoilCompleted && EnableShooting.RunTimeValue)
        {
            _recoilCompleted = false;
            _currentRotateAngle += _angleStep;
            _currentDelay = _bulletData.Delay;

            for (_currentRotateAngle = 0; _currentRotateAngle < 360; _currentRotateAngle += _angleStep)
            {
                FireOnAngle(_currentRotateAngle);
            }

            _currentRotateAngle = _currentRotateAngle >= 360 ? 0 : _currentRotateAngle;
            OnShoot?.Invoke();
        }
    }

    void FireOnAngle(float currentAngle)
    {
        var bullet =
            _bulletPool.CreateObjectWithNewParent(_barrelTranTrans.position, MathfPlus.AngToRotationZAxis(currentAngle));
        bullet.transform.position = _barrelTranTrans.position;
        bullet.transform.localRotation = MathfPlus.AngToRotationZAxis(currentAngle);
        bullet.GetComponent<Bullet>().Initialize(_bulletData);
    }

    void Update()
    {
        if (_recoilCompleted == false)
        {
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0f) _recoilCompleted = true;
        }
    }
    void OnDisable()
    {
        if (_bulletPool == null) return; 
        foreach (var bullet in _bulletPool._objectPoolGOs.ToArray())
        {
            if(bullet.activeSelf)
                _bulletPool.ReturnToPool(bullet);
        }    
    }
}
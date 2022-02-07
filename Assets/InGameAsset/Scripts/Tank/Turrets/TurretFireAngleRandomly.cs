using InGameAsset.Scripts;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

public class TurretFireAngleRandomly : Turret, ITurret
{
    [SerializeField] BoolGameData EnableShooting;
    [SerializeField] float _angleStep;
    [SerializeField] int _minRandomRange = 10;
    [SerializeField] int _maxRandomRange = 30;
    bool _recoilCompleted;
    float _currentRotateAngle;
    float _currentDelay;
    int _numberOfBullets;
    [SerializeField] UnityEvent OnShoot;

    void ShootAtAngleDetermineByNumBullet()
    {
        _angleStep = 360f / _numberOfBullets;
        float angle = 0f;
        for (int i = 0; i <= _numberOfBullets - 1; i++)
        {
            float bulletDirXPos =
                _barrelTranTrans.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * _bulletData.MaxDistance;
            float bulletDirYPos =
                _barrelTranTrans.position.x + Mathf.Cos((angle * Mathf.PI) / 180) * _bulletData.MaxDistance;
            Vector2 bulletPos = new Vector2(bulletDirXPos, bulletDirYPos);
            Vector2 bulletDir = (bulletPos - (Vector2) _barrelTranTrans.position).normalized;
            var bullet =
                _bulletPool.CreateObjectWithNewParent(_barrelTranTrans.position, MathfPlus.DirToRotationZAxis(bulletDir));
            bullet.GetComponent<Bullet>().Initialize(_bulletData);
            angle += _angleStep;
        }
    }
    void Update()
    {
        if (_recoilCompleted) return;
        _currentDelay -= Time.deltaTime;
        if (_currentDelay <= 0f) _recoilCompleted = true;
    }

    public void Shoot()
    {
        if (_recoilCompleted && EnableShooting.RunTimeValue)
        {
            _numberOfBullets = Random.Range(_minRandomRange, _maxRandomRange);
            _recoilCompleted = false;
            _currentDelay = _bulletData.Delay;
            ShootAtAngleDetermineByNumBullet();
            // OnShoot?.Invoke();
        }
        // ShootAtEveryAngleWithDelayPerCycle();
    }
    void OnDisable()
    {
        foreach (var bullet in _bulletPool._objectPoolGOs)
        {
            if(bullet.activeSelf)
                _bulletPool.ReturnToPool(bullet);
        }    
    }
}
using System.Collections;
using System.Collections.Generic;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

public class TurretShootFromBarrelWithOwnUpdate : Turret, ITurret
{
    [SerializeField] BoolGameData _enableShooting;
    //public BulletData GetBulletData => _bulletData;
    public List<Transform> _turretBarrels;
    public UnityEvent<BulletData> OnShoot;
    public UnityEvent<BulletData> OnReloadingFinish;
    public UnityEvent<float> OnReloading;
    

    float _currentDelay;
    bool _recoilCompleted = true;
    float _percentageReload = 1;
    IEnumerator _reloadBulletIEnumerator;
    
    void Awake()
    {
        var turretBarrelObject = gameObject.GetComponentsInChildren<Barrel>();
        for (int i = 0; i < turretBarrelObject.Length; i++)
        {
            _turretBarrels.Add(turretBarrelObject[i].transform);
        }
        //bulletPool = GetComponent<ObjectPool>();
    }

    protected override void  Start()
    {
        base.Start();
        OnReloading?.Invoke(1);
        OnReloadingFinish?.Invoke(_bulletData);
    }

    void OnEnable()
    {
        // OnShoot?.Invoke(BulletData);
        _enableShooting.RunTimeValue = true;
        OnReloading?.Invoke(_percentageReload);
        OnReloadingFinish?.Invoke(_bulletData);
    }

    void Update()
    {
        if (_recoilCompleted == false)
        {
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0f)
            {
                _recoilCompleted = true;
            }
        }
    }
    public void ShootWithNoReload()
    {
        foreach (var barrel in _turretBarrels)
        {
            var bullet = _bulletPool.CreateObjectWithNewParent(barrel.position, barrel.rotation);
            bullet.GetComponent<Bullet>().Initialize(_bulletData);
        }
        OnShoot?.Invoke(_bulletData);
    }

    public void Shoot()
    {
        if (_recoilCompleted && _enableShooting.RunTimeValue)
        {
            _recoilCompleted = false;
            _currentDelay = _bulletData.Delay;
            foreach (var barrel in _turretBarrels)
            {
                //var hit = Physics2D.Raycast(barrel.position, barrel.up);
                var bullet = _bulletPool.CreateObjectWithNewParent(barrel.position, barrel.rotation);
                bullet.GetComponent<Bullet>().Initialize(_bulletData);
            }

            if (gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _bulletData.BulletNum.RunTimeValue--;
            }

            OnShoot?.Invoke(_bulletData); 
        }

        if (_bulletData.BulletNum.RunTimeValue <= 0 && _enableShooting.RunTimeValue &&
            gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_NAME))
        {
            _reloadBulletIEnumerator = ReloadingBullet();
            StartCoroutine(_reloadBulletIEnumerator);
        }
    }

    IEnumerator ReloadingBullet()
    {
        _enableShooting.RunTimeValue = false;
        var currentTime = Time.time;
        var reloadRequireTime = currentTime + _bulletData.ReloadTime;
        _percentageReload = 0;
        while (reloadRequireTime > Time.time)
        {
            float timeSinceStarted = Time.time - currentTime;
            _percentageReload = timeSinceStarted / _bulletData.ReloadTime;
            _bulletData.BulletNum.RunTimeValue =
                Mathf.FloorToInt(Mathf.Lerp(0, _bulletData.Capacity, _percentageReload));
            if (_percentageReload > 0.9f)
                _bulletData.BulletNum.RunTimeValue = _bulletData.Capacity;
            OnReloading?.Invoke(_percentageReload);
            yield return null;
        }
        _enableShooting.RunTimeValue = true;
        OnReloading?.Invoke(_percentageReload);
        OnReloadingFinish?.Invoke(_bulletData);
    }
}
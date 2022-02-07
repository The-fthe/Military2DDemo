using System;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Util;
using UnityEngine;

public abstract class Turret:MonoBehaviour
{
    [SerializeField] protected Transform _barrelTranTrans;
    [SerializeField] protected  ObjectPool _bulletPool;
    [SerializeField] protected  BulletData _bulletData;
    public BulletData GetBulletData => _bulletData;

    protected virtual void Start()
    {
        _barrelTranTrans ??= GetComponentInChildren<Barrel>().transform;
        _bulletPool = _bulletData.ObjectPool;
        var bulletPool = Instantiate(_bulletPool, gameObject.transform, true);
        _bulletPool = bulletPool;
        _bulletPool.SetObjectPoolAndPoolSize(null, _bulletData.Capacity * 2);
    }
    public virtual void Initialized(BulletData bulletData, ObjectPool bulletPool)
    {
        _bulletPool = bulletPool;
        _bulletData = bulletData;
    }

}
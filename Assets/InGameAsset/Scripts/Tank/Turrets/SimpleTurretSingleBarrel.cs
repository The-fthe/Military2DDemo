using System;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

namespace InGameAsset.Scripts.Tank.Turrets
{
    public class SimpleTurretSingleBarrel : Turret, ITurret
    {
        public UnityEvent<BulletData> OnShoot;
        public Transform TurretBarrelTran => _barrelTranTrans;
        

        public void Shoot()
        {
            var bullet = _bulletPool.CreateObjectWithNewParent(_barrelTranTrans.position, _barrelTranTrans.rotation);
            bullet.GetComponent<Bullet>().Initialize(_bulletData);
            if (gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_NAME))
            {
                _bulletData.BulletNum.RunTimeValue--;
            }

            OnShoot?.Invoke(_bulletData);
        }
    }
}
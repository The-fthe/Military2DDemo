using System.Collections.Generic;
using System.Linq;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

namespace InGameAsset.Scripts.army
{
    public class Gun : Turret,ITurret
    {
        [SerializeField] List<Transform> _turretBarrels;

        //public GunData gundata;
       // public List<Transform> gunPointers;

        bool canShoot = true;
        [SerializeField]Collider2D[] _tankColliders;
        float currentDelay;

        public UnityEvent OnShoot, OnCantShoot;
        public UnityEvent<float> OnReloading;
        protected override void Start()
        {
            base.Start();
            Initialize();
        }

        public void Initialize()
        {
            _tankColliders ??= GameObject.Find(Const.PLAYER_MOVE_UNIT_NAME).GetComponents<Collider2D>();
             _turretBarrels = GetComponentsInChildren<Barrel>().Select((i)=>i.transform).ToList();
            OnReloading?.Invoke(currentDelay); //TODO: maybe a reload animation
        }

        void Update()
        {
            if (canShoot == false)
            {
                currentDelay -= Time.deltaTime;
                OnReloading?.Invoke(currentDelay / _bulletData.Delay);
                if (currentDelay <= 0f) canShoot = true;
            }
        }
        public void Shoot()
        {
            if (canShoot)
            {
                canShoot = false;
                currentDelay = _bulletData.Delay;
                foreach (var gunPointer in _turretBarrels)
                {
                    var hit = Physics2D.Raycast(gunPointer.position, gunPointer.up);
                    var bullet = _bulletPool.CreateObjectWithNewParent(gunPointer.position, gunPointer.rotation);
                    bullet.transform.position = gunPointer.position;
                    bullet.transform.localRotation = gunPointer.rotation;
                    bullet.GetComponent<Bullet>().Initialize(_bulletData);
                    foreach (var collider in _tankColliders)
                    {
                        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), collider);
                    }
                }

                OnShoot?.Invoke();
                OnReloading?.Invoke(currentDelay);
            }
            else
            {
                OnCantShoot?.Invoke();
            }
        }
    }
}
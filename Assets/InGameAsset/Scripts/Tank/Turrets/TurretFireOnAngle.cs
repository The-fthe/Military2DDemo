using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

namespace InGameAsset.Scripts.Tank.Turrets
{
    public class TurretFireOnAngle : Turret, ITurret
    {
        [SerializeField] BoolGameData EnableShooting;
        [SerializeField] int _angleStep;
        [SerializeField,Range(-360,0)] int _minAngle = 5;
        [SerializeField,Range(0,360)] int _maxAngle=20;
        bool _recoilCompleted;
        float _currentRotateAngle;
        float _currentDelay;
        [SerializeField] UnityEvent OnShoot;
        //public override string Name = "FireOnAngle";

        void ShootAtAngWithMaxAngle()
        {
            if (_recoilCompleted && EnableShooting.RunTimeValue)
            {
                _recoilCompleted = false;
                _currentDelay = _bulletData.Delay;
                for (int i = _minAngle; i < _maxAngle; i += _angleStep)
                {
                    var shotPos= MathfPlus.AngToDir(transform.rotation.eulerAngles.z + i);
                    FireOnDirection(shotPos,_barrelTranTrans.position);
                }
                _currentRotateAngle = _currentRotateAngle >= 360 ? 0 : _currentRotateAngle;
                OnShoot?.Invoke();
            }
        }
        void Update()
        {
            if (_recoilCompleted == false)
            {
                _currentDelay -= Time.deltaTime;
                if (_currentDelay <= 0f) _recoilCompleted = true;
            }
        }

        void FireOnAngle(float currentAngle)
        {
            var bullet = _bulletPool.CreateObjectWithNewParent(_barrelTranTrans.position, MathfPlus.AngToRotationZAxis(currentAngle));
            bullet.transform.position = _barrelTranTrans.position;
            bullet.transform.localRotation = MathfPlus.AngToRotationZAxis(currentAngle);
            bullet.GetComponent<Bullet>().Initialize(_bulletData);
        }

        public void FireOnDirection(Vector2 dir, Vector3 pos)
        {
            var bullet = _bulletPool.CreateObjectWithNewParent(pos, MathfPlus.DirToRotationZAxis(dir));
            bullet.GetComponent<Bullet>().Initialize(_bulletData);
        }

        public void Shoot()
        {
            ShootAtAngWithMaxAngle();
        }
        void OnDisable()
        {
            foreach (var bullet in _bulletPool._objectPoolGOs.ToArray())
            {
                if(bullet.activeSelf)
                    _bulletPool.ReturnToPool(bullet);
            }    
        }

    }
}
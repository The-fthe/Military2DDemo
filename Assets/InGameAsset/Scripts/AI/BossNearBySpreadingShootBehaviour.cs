using System;
using System.Linq;
using InGameAsset.Scripts.Tank.Turrets;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameAsset.Scripts.AI
{
    public class BossNearBySpreadingShootBehaviour : SerializedMonoBehaviour, IState
    {
        TankController _bossTank;
        float _currentScanAroundDelay;
        [SerializeField] float _scanningTime=3f;
        [SerializeField]Transform _target;
        void Start()
        {
            var parent = transform.parent.parent;
            _bossTank =parent.GetComponentInChildren<TankController>();
        }

        public  void Tick()
        {
            _bossTank.HandleMoveBody(Vector2.zero);
            var AimTarget = (_target.position - _bossTank._turretAimer.transform.position).normalized;
            float angle = Vector2.Angle(_bossTank._turretAimer.transform.right, AimTarget);
            if (_currentScanAroundDelay <= 0 || angle < 4)
            {
                //_bossTank.HandleShoot();
                _bossTank._turretAimer._turrets.FirstOrDefault(i => i.GetType().Equals(typeof(TurretFireOnAngle)))?.Shoot();
                _currentScanAroundDelay = _scanningTime;
            }
            else
            {
                if (_currentScanAroundDelay > 0) 
                    _currentScanAroundDelay -= Time.deltaTime;
                _bossTank.HandleTurretMovement(AimTarget);
            }
        }

        public  void Enter()
        {
            _target = FindObjectOfType<PlayerManager>().GetPlayerGO.GetComponentInChildren<Rigidbody2D>().transform;
        }

        public  void Exit()
        {
            Debug.Log($"Exited static shoot {gameObject.name }");
        }
    }
}
using System;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class AIShootBehaviour : MonoBehaviour, IState
    {
        TankController _tank;
        AIDetector _aiDetector;
        public float fieldOfVisionForShooting = 60;
        [SerializeField]Transform _target;
        Vector2 _targetDirection;
        void SetTarget() => _target = _aiDetector.Target;

        public void Awake()
        {
            _tank = GetComponentInChildren<TankController>();
            _aiDetector = GetComponentInChildren<AIDetector>();
            _aiDetector.OnTargetFind += SetTarget;
        }
        
        public  void Tick()
        {
            if (TargetInFOV(_tank, _target))
            {
                _tank.HandleMoveBody(Vector2.zero);
               _tank.HandleTurretMovement(_targetDirection);
                _tank.HandleShoot();
            }
        }

        bool TargetInFOV(TankController tank, Transform target)
        {
            if (target != null)
            {
                _targetDirection = (target.position - tank._turretAimer.transform.position).normalized;
                if (Vector2.Angle(tank._turretAimer.transform.right, _targetDirection) < fieldOfVisionForShooting / 2)
                    return true;
            }
            return false;
        }

        public  void Enter()
        {
        }

        public  void Exit()
        {
        }
    }
}
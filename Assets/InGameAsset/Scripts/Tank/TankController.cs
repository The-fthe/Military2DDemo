using Sirenix.OdinInspector;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Animations;

namespace InGameAsset.Scripts
{
    public  class TankController : SerializedMonoBehaviour
    {
        public TankMover _tankMover;
        public TurretAimer _turretAimer;
        public ITurret[] _turrets;

        public  void Awake()
        {
            _tankMover ??= GetComponentInChildren<TankMover>();
            var parent = gameObject.transform.parent;
            _turretAimer ??= parent.GetComponentInChildren<TurretAimer>();

            if (_turrets is null || _turrets.Length == 0)
            {
                _turrets = _turretAimer.gameObject.GetComponentsInChildren<ITurret>();
            }
        }

        public void HandleShoot()
        {
            foreach (var turret in _turrets)
            {
                turret.Shoot();
            }
        }

        public  void HandleMoveBody(Vector2 movementVector)
        {
            _tankMover.Move(movementVector);

        }

        public  void HandleTurretMovement(Vector2 pointerPosition)
        {
            _turretAimer.AimToDir(pointerPosition);

        }

        public void HandleTurretMovement(float angle)
        {
            _turretAimer.RotateTurretToAng(angle);

        }
    }
}
using System.Collections.Generic;
using System.Linq;
using InGameAsset.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InGameAsset.Scripts
{
    public class TurretAimer : SerializedMonoBehaviour
    {
        [SerializeField] TankMovementData _tankData;
        float _desiredAngle;
        Vector2 _aimVec2;
        [SerializeField] Camera _mainCamera;
        public List<ITurret> _turrets;
        public UnityEvent<Vector2> OnMoveTurret = new UnityEvent<Vector2>();

        void Awake()
        {
            _desiredAngle = gameObject.transform.rotation.eulerAngles.z;
            _mainCamera = Camera.main;
            if (_turrets is null || _turrets.Count == 0)
            {
                _turrets =gameObject.GetComponentsInChildren<ITurret>().ToList();
            }
        }
        
        public void OnFire(InputValue inputValue)
        {
        //     if (inputValue.isPressed)
        //     {
        //         foreach (var turret in _turrets)
        //         {
        //             turret.Shoot();
        //         }
        //     }
        }

        public void AimToDir(Vector2 inputPointerPosition)
        {
            _aimVec2 = inputPointerPosition;
            _desiredAngle = MathfPlus.DirToAng(inputPointerPosition);
        }

        public void RotateTurretToAng(float desiredAngle)
        {
            _desiredAngle = desiredAngle;
        }

        public void GetAimDirAndAngle(Vector2 dir, float angle)
        {
            _aimVec2 = dir;
            _desiredAngle = angle;
        }

        void FixedUpdate()
        {
            if (_aimVec2 != Vector2.zero || _desiredAngle != 0)
            {
                var rotationStep = _tankData.turretRotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(0, 0, _desiredAngle), rotationStep);
            }
        }
    }
}
using UnityEngine;

namespace InGameAsset.Scripts
{
    public class PlayerTankController
    {

        // public override void Awake()
        // {
        // }

        // public override void HandleBarrelShoot()
        // {
        //     foreach (var turret in _turrets)
        //     {
        //         turret.ShootFromBarrel();
        //     }
        // }

        // public override void HandleMoveBody(Vector2 movementVector)
        // {
        //     _tankMover.Move(movementVector);
        // }

        // public override void HandleTurretMovement(Vector2 pointerPosition)
        // {
        //     // Debug.Log("tank controller "+ pointerPosition);
        //     if (pointerPosition.x < -1 && pointerPosition.y < -1 || pointerPosition.x > 1 && pointerPosition.y > 1)
        //     {
        //         Vector3 mousePosition = pointerPosition;
        //         mousePosition.z = mainCamera.nearClipPlane;
        //         Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        //         var turretDirection = (mouseWorldPosition - (Vector2) _turrets[0].gameObject.transform.position)
        //             .normalized;
        //         _aimTurret.AimToDir(turretDirection);
        //     }
        //     else
        //         _aimTurret.AimToDir(pointerPosition);
        // }
        //
        // public override void HandleTurretMovement(float angle)
        // {
        //     _aimTurret.RotateTurretToAng(angle);
        // }
    }
}
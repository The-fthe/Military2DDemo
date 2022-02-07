using InGameAsset.Scripts.Data;
using UnityEngine;
using UnityEngine.Events;

namespace InGameAsset.Scripts
{
    class EnemyTankMover : TankMover
    {
        public Rigidbody2D rb2d;
        public TankMovementData MovementData;

        [SerializeField] float currentSpeed = 0;
        [SerializeField] float currentForwardDirection = 1;
        Vector2 movementVector;

        public Vector3 Pos => transform.position;

        void Awake()
        {
            rb2d = GetComponentInParent<Rigidbody2D>();
            if (rb2d) return;
            rb2d = GetComponent<Rigidbody2D>();
        }

        public override void Move(Vector2 movementVector2)
        {
            this.movementVector = movementVector2;
            CalculataSpeed(movementVector2);
            if (movementVector2.y > 0) currentForwardDirection = 1;
            else if (movementVector2.y < 0) currentForwardDirection = -1;
        }

        void CalculataSpeed(Vector2 movementVector)
        {
            if (Mathf.Abs(movementVector.y) > 0)
                currentSpeed += MovementData.acceleration * Time.deltaTime;
            else
                currentSpeed -= MovementData.deacceleration * Time.deltaTime;
            if (movementVector.x != 0)
                currentSpeed -= MovementData.deacceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, MovementData.maxSpeed);
        }

        void FixedUpdate()
        {
            rb2d.velocity = (Vector2) transform.up * currentSpeed * currentForwardDirection * Time.fixedDeltaTime;
            rb2d.MoveRotation(transform.rotation *
                              Quaternion.Euler(0, 0, -movementVector.x *
                                                     MovementData.rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
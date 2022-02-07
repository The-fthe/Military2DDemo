using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Data
{
    [CreateAssetMenu(fileName = "tankData_new", menuName = "Tank/tankData", order = 0)]
    [InlineEditor]
    public class TankMovementData : SerializedScriptableObject
    {
        public float acceleration = 0;
        public float deacceleration;
        public float maxSpeed;
        public float rotationSpeed;

        public float turretRotationSpeed = 150;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace InGameAsset.Scripts
{
    public abstract class TankMover:MonoBehaviour
    {
        public virtual void Move(Vector2 movementVector2){}
    }
}
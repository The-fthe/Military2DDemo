using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class TargetDetector:MonoBehaviour
    {
        Transform origin;
        float _nearbyDistance;
        public Transform Target { get; private set; }
        bool _targetIsDetory;

        public void Initialized(Transform targetToDetect)
        {
            origin = transform;
            Target = targetToDetect;
            _targetIsDetory = false;
        }

        public void Destruction()
        {
            _targetIsDetory = true;
        }
        
        public bool isTargetNearBy(float nearbyDistance)
        {
            if (_targetIsDetory) return false;
            _nearbyDistance = nearbyDistance;
            return  Vector2.Distance(origin.position, Target.position) <= _nearbyDistance;
        }

        void OnDrawGizmos()
        {
            if (Target != null)
            {
               // Debug.Log($" distance is {Vector2.Distance(origin.position, Target.position)}");
                Gizmos.DrawLine(origin.position,Target.position);
            }
        }
    }
}
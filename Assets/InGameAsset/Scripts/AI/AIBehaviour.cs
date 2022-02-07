using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public abstract class AIBehaviour: MonoBehaviour
    {
        public abstract void PerformAction(TankController tank, AIDetector detector);
    }
}
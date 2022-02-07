using UnityEngine;

namespace InGameAsset.Scripts.Util
{
    public class ReturnToPoolIfDisabled : MonoBehaviour
    {
        [SerializeField]ObjectPool objectPool;
        public bool SelfDestructionEnable { get; set; } = false; 
        void OnDisable()
        {
            if(SelfDestructionEnable) objectPool.ReturnToPool(gameObject);
        }
    }
}
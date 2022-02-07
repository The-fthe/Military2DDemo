using JetBrains.Annotations;
using UnityEngine;

namespace InGameAsset.Scripts.Util
{
    public class DestroyIfDisabled : MonoBehaviour
    {
        ObjectPool _ObjectPool;
        public ObjectPool ObjectPool { get=>  _ObjectPool; set =>  _ObjectPool= value;}
        public bool SelfDestructionEnable { get; set; } = false; 
        
        void OnDisable()
        {
            //if(SelfDestructionEnable) Destroy(gameObject);
            if(SelfDestructionEnable) _ObjectPool.ReturnToPool(gameObject);
        }
    }
}
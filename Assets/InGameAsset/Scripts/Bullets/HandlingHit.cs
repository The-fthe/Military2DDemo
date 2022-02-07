using System;
using UnityEngine;

namespace InGameAsset.Scripts
{
    [Serializable]
    public abstract class HandlingHit:IHandleHit
    {
        public abstract int OnHandleHit(int hit);
        public int HandleHit(int hit)
        {
            return OnHandleHit(hit);
        }
    }
}
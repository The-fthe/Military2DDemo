using System;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Data
{
    [CreateAssetMenu(fileName = "BulletData_new", menuName = "Bullet/BulletData", order = 0)]
    [InlineEditor()]
    public class BulletData : ScriptableObject
    {
        public float InitialSpeed= 5;
        public float MaxSpeed = 10;
        public float MaxDistance = 5;
        public int Damage= 5;
        public int Bounce = 0;
        public LayerMask BounceLayer ;
        public  TagLayer TagOwner;
        public TagLayer TargetUnit;
        public int Capacity;
        public float Delay = 1;
        public int ReloadTime = 5;
        public float AcceleartionPerSec;
        public IntGameData BulletNum;
        public bool CanReactToBullet = true;
        public bool UseAcceleration = false;
        public bool LimitMaxSpeed= false;
        public float UpdateFrequency = 0.1f;
        public ObjectPool ObjectPool;
       // public bool isMagazineLoading = false;        
        
        void OnEnable()
        {
            BounceLayer = LayerMask.GetMask("Hitable");
            Capacity = BulletNum.Value;
        }
    }

    public enum TagLayer
    {
        Player,Enemy,None
    }
}
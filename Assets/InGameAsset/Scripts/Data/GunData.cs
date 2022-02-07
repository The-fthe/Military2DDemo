using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Data
{
    [InlineEditor()]
    [CreateAssetMenu(fileName = "Data_Gun_new", menuName = "Gun/gun_new", order = 0)]
    public class GunData : ScriptableObject
    {
        public GameObject bulletPrefab;
        public float reloadDelay = 1;
        public float FullyReloadDelay = 2;
        public BulletData bulletData;
        //public bool canStartShooting;

        // public void enableShooting()
        // {
        //         canStartShooting= true;
        // }
        // void OnEnable()
        // {
        //     canStartShooting = false;
        // }
        //
        // void OnDisable()
        // {
        //     canStartShooting = false;
        // }
    }
}
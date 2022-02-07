using System.Collections;
using System.Linq;
using InGameAsset.Scripts.Player;
using UnityEngine;

namespace InGameAsset.Scripts.army
{
    public class ArmyController : MonoBehaviour
    {
        [SerializeField] GunAimer[] gunAimer;

        [SerializeField] Gun[] gun;

        // [SerializeField] GunData gunData;
        [SerializeField] BoolGameData canStartShoot;
        [SerializeField] HostAttachment _HostAttachment;

        bool isInitilize = false;

        void Start()
        {
        }
        
        void Update()
        {
            if (canStartShoot.RunTimeValue && isInitilize)
            {
                // HandleRotation(Input.mousePosition);
                // HandleShooting();
            }
        }

        // public void HandleShooting()
        // {
        //     for (int i = 0; i < gun.Length; i++)
        //     {
        //         gun[i].Shoot();
        //     }
        // }
        //
        public void HandleRotation(Vector2 target)
        {
            // for (int i = 0; i < gunAimer.Length; i++)
            //   //  gunAimer[i].Aim(target);
        }
    }
}
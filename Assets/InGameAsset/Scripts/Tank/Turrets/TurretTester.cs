using UnityEngine;

namespace InGameAsset.Scripts.Turrets
{
    public class TurretTester : ITurret
    {
        public void Shoot()
        {
            Debugger.Log("shoot is trigger");
        }
    }
}
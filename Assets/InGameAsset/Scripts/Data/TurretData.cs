using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Data
{
    [InlineEditor()]
    [CreateAssetMenu(fileName = "Data_Turret_new", menuName = "Turret/TurretData", order = 0)]
    public class TurretData : ScriptableObject
    {
        public float reloadDelay= 1;
    }
}
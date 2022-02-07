using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Data
{
    [CreateAssetMenu(fileName = "DataAmy_new", menuName = "Soldiers/DataAmy_new", order = 0)]
    [InlineEditor()]
    public class ArmyData : ScriptableObject
    {
        public float rotationSpeed = 150;
    }
}
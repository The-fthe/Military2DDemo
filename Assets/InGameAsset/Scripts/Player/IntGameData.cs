using System;
using InGameAsset.Scripts.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Player
{
    [InlineEditor()]
    [CreateAssetMenu(fileName = "Int_new", menuName = "GameData/Int", order = 0)]
    public class IntGameData : ScriptableObject
    {
        public int Value= 3;
        public int RunTimeValue ;
        void OnEnable()
        {
            RunTimeValue = Value;
        }
    }
}
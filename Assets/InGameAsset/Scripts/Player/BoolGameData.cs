using System;
using System.Net.Http.Headers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Player
{
    [InlineEditor()]
    [CreateAssetMenu(fileName = "Bool_new", menuName = "GameData/Bool", order = 1)]
    public class BoolGameData : ScriptableObject
    {
         public bool Value = false;
        public bool RunTimeValue ;
        void OnEnable() => RunTimeValue = Value;
        void OnDisable()=> RunTimeValue = Value;
        public void Set( bool value)
        {
            RunTimeValue = value;
        }
    }
}
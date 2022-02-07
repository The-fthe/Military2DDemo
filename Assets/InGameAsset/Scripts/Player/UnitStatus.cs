using System;
using InGameAsset.Scripts.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace InGameAsset.Scripts.Player
{
    public class UnitStatus : MonoBehaviour
    {
        public IntGameData MaxHealth; 
        public IntGameData Life;

        public  UnityEvent<IntGameData,IntGameData> OnUnitCreated;
        public int Health
        {
            get
            {
                if (_currentHealthData == null)
                {
                    _currentHealthData = ScriptableObject.CreateInstance<IntGameData>();
                    _currentHealthData.Value = MaxHealth.Value;
                    _currentHealthData.RunTimeValue = MaxHealth.Value;
                    return _currentHealthData.RunTimeValue;
                }
                return _currentHealthData.RunTimeValue;
            }
            set =>_currentHealthData.RunTimeValue = value;
        }

        public bool WasInvinsibleFinish;
        public float TimeToInvisible = 0f;
        public int SpawnTime = 2;
        public float UnitScale = 1;
        public IntGameData _currentHealthData;

        void Awake()
        {
            _currentHealthData ??= ScriptableObject.CreateInstance<IntGameData>();
            _currentHealthData.Value = MaxHealth.Value;
            _currentHealthData.RunTimeValue = MaxHealth.Value;
        }
        
        void OnEnable()
        {
            WasInvinsibleFinish = true;
            UnitScale =gameObject.transform.localScale.z;
            OnUnitCreated?.Invoke(_currentHealthData,Life);
        }
        
    }
}
using System;
using InGameAsset.Scripts.AI;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public abstract class AIState : MonoBehaviour,IState
    {
        public abstract void OnTick();
        public abstract void OnEnter();
        public abstract  void OnExit();
        public void Tick()
        {
             OnTick();
        }

        public void Enter()
        {
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
        }
    }
}

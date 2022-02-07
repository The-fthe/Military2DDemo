using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public interface IState
    {
        public void Tick();
        public void Enter();
        public void Exit();
    }
}
using System;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class PatrollingEnemyAI : MonoBehaviour
    {
        [SerializeField] TankController _tank;
        [SerializeField] AIDetector _detector;
        StateMachine _stateMachine;

        void Awake()
        {
            _detector = GetComponentInChildren<AIDetector>();
            _tank = GetComponentInChildren<TankController>();

            _stateMachine = new StateMachine();
            var shootAtTarget = GetComponent<AIShootBehaviour>();
            var searchTarget = GetComponent<AIPatrolStaticBehaviour>();

            At(searchTarget,shootAtTarget,FindTarget());
            At(shootAtTarget,searchTarget,TargetIsOutOfRange());

            //_stateMachine.AddAnyTransition(shootAtTarget,FindTarget());
            _stateMachine.SetState(searchTarget);            
            
            void At(IState from, IState to, Func<bool> condition) 
                => _stateMachine.AddTransition(from, to, condition);
            
            Func<bool> FindTarget() => () =>_detector.TargetVisible;
            Func<bool> TargetIsOutOfRange() =>() =>  _detector.Target != null &&!_detector.TargetVisible;
        }

        void Update()
        {
            _stateMachine.Tick();
            Debug.Log("find target" +   _detector.TargetVisible);
            //Debug.Log("detertor target is" + _detector.Target.name);
        }
    }
}
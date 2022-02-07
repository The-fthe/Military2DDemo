using System;
using System.Collections;
using InGameAsset.Scripts.Player;
using UnityEditor;
using UnityEngine;
using SerializedMonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;

namespace InGameAsset.Scripts.AI
{
    public class EnemyAI_ChaserTank : SerializedMonoBehaviour
    {
        [SerializeField] TankController _tank;
        [SerializeField] AIDetector _detector;
        [SerializeField] float _distanceToGiveUp = 25f;
        public bool IsLastFight;
        public Transform FocusTarget;
        
        AIShootBehaviour shootAtTarget;
        AIPatrolStaticBehaviour searchTarget;
        AIChaseBehavior moveToTarget;
        ChaserAIPatrolPathBehaviour goPatroling;
        

        StateMachine _stateMachine;
        DisplayLog _displayLog;

        void Start()
        {
            var playerSpawner = FindObjectOfType<StageLevel>();
            if(playerSpawner != null) 
                playerSpawner.OnGameStart.AddListener(Initialize);
            else
            {
                Debugger.LogError("StageLevel Cant be found");
            }
        }
        void Initialize()
        {
            _displayLog ??= GetComponentInChildren<DisplayLog>();
            _detector = GetComponentInChildren<AIDetector>();
            _tank = GetComponentInChildren<TankController>();

            _stateMachine = new StateMachine();
            shootAtTarget = GetComponent<AIShootBehaviour>();
            searchTarget = GetComponent<AIPatrolStaticBehaviour>();
            moveToTarget = GetComponent<AIChaseBehavior>();
            goPatroling = GetComponentInChildren<ChaserAIPatrolPathBehaviour>();

            At(shootAtTarget, moveToTarget, TargetInChaseRange());
            At(moveToTarget, goPatroling, TargetOutOfChaseRange());
            At(goPatroling,moveToTarget,LastFight());
           // At(shootAtTarget,moveToTarget,LastFight());
            At(shootAtTarget,moveToTarget,TargetOutOfShootRange());
            At(moveToTarget, goPatroling, () => moveToTarget._isTimeToGiveUp && !IsLastFight);

            _stateMachine.AddAnyTransition(shootAtTarget, FindTarget());
            _stateMachine.SetState(goPatroling);

            void At(IState from, IState to, Func<bool> condition)
                => _stateMachine.AddTransition(@from, to, condition);

            Func<bool> FindTarget() => () => _detector.TargetVisible;

            Func<bool> TargetInChaseRange() => ()
                => _detector.Target != null && !_detector.TargetVisible &&
                   moveToTarget.TotalDistance <= _distanceToGiveUp && !IsLastFight;

            Func<bool> TargetOutOfChaseRange()
                => () => _detector.Target != null && !_detector.TargetVisible &&
                         moveToTarget.TotalDistance > _distanceToGiveUp && !IsLastFight;

            Func<bool> TargetOutOfShootRange() => () => !_detector.TargetVisible;

            Func<bool> LastFight() => () =>
            {
                if(IsLastFight)
                    moveToTarget.Target = FocusTarget;
                return IsLastFight;
            };
            _displayLog.Log += () => _stateMachine.CurrentState.GetType().Name;
        }

        void Update()
        {
            if(_stateMachine != null)
                _stateMachine.Tick();
        }
    }
}
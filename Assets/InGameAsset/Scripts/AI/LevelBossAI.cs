using System;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class LevelBossAI : BossUnit
    {
        [SerializeField] StageLevel _stageLevel;
        [SerializeField] Transform _targetToKill;
        [SerializeField] Level1BossStateController[] _states;
        [SerializeField,Range(0,1.0f)] float[] _changeStatePercentages= {0.9f,0.75f,0.5f,0.25f};
         float BossHpPercentage => _bossHp == null?-1: _bossHp.RunTimeValue/(float)_bossHp.Value;
        StateMachine _stateMachine;
        DisplayLog _displayLog;
        bool _isInitialize;

        void Start()
        {
            _stageLevel.OnGameStart.AddListener(Initialize);
        }

        public void Initialize()
        {
            Debug.Log("boss initialize is trigger");
            _bossHp.RunTimeValue = _bossHp.Value;
            _targetToKill = GameObject.Find(Const.PLAYER_MOVE_UNIT_NAME).transform;
            foreach (var state in _states)
            {
                state.SetTargetTrans = _targetToKill;
            }
            _displayLog ??= GetComponentInChildren<DisplayLog>();
            _stateMachine = new StateMachine();
            
            At(_states[0], _states[1], isStage2Ready());
            At(_states[1], _states[2], isStage3Ready());
            At(_states[2], _states[3], isStage4Ready());

            _states ??= GetComponentsInChildren<Level1BossStateController>();
            _stateMachine.SetState(_states[0]);

            void At(IState from, IState to, Func<bool> condition)
                => _stateMachine.AddTransition(from, to, condition);

            Func<bool> isStage2Ready() => () => BossHpPercentage >= _changeStatePercentages[1] && BossHpPercentage <=_changeStatePercentages[0];
            Func<bool> isStage3Ready() => () => BossHpPercentage >= _changeStatePercentages[2] && BossHpPercentage <_changeStatePercentages[1];
            Func<bool> isStage4Ready() => () => BossHpPercentage <=_changeStatePercentages[3]&& BossHpPercentage <_changeStatePercentages[2];

            _isInitialize = true;
            _displayLog.Log += () => _stateMachine.CurrentState.GetType().Name;
        }

        void Update()
        {
            if (_isInitialize)
            {
                _stateMachine.Tick();
            }
        }

        public override IntGameData GetBossHp()
        {
            return _bossHp;
        }
    }
}
using System;
using InGameAsset.Scripts.AI;
using InGameAsset.Scripts.Player;
using UnityEngine;
using Utility;

public class TowerBossAI : BossUnit
{
    [SerializeField] StageLevel _stageLevel;
    [SerializeField]TowerBossFireMode[] _states;
    [SerializeField] DeadMode _deadMode;
    [SerializeField,Range(0,1.0f)] float[] _changeStatePercentages= {0.9f,0.75f,0.5f,0.25f,0f};
    float BossHpPercentage => _bossHp == null?-1: _bossHp.RunTimeValue/(float)_bossHp.Value;
    StateMachine _stateMachine;
    bool _isInitialize;
    void Start() => _stageLevel.OnGameStart.AddListener(Initialize);

    public void Initialize()
    {
        _stateMachine = new StateMachine();
        _bossHp.RunTimeValue = _bossHp.Value;
         At(_states[0], _states[1], isStage2Ready());
         At(_states[1], _states[2], isStage3Ready());
         At(_states[2], _states[3], isStage4Ready());
        
        //_states ??= GetComponentsInChildren<Level1BossStateController>();
        _stateMachine.SetState(_states[0]);
        _stateMachine.AddAnyTransition(_deadMode,isBossDead());
        _states =_states.Length <=0? GetComponentsInChildren<TowerBossFireMode>(): _states;
        
        void At(IState from, IState to, Func<bool> condition)
            => _stateMachine.AddTransition(from, to, condition);

        Func<bool> isStage2Ready() => () => BossHpPercentage >= _changeStatePercentages[1] && BossHpPercentage <=_changeStatePercentages[0];
        Func<bool> isStage3Ready() => () => BossHpPercentage >= _changeStatePercentages[2] && BossHpPercentage <_changeStatePercentages[1];
        Func<bool> isStage4Ready() => () => BossHpPercentage <=_changeStatePercentages[3]&& BossHpPercentage <_changeStatePercentages[2];
        Func<bool> isBossDead() => () => BossHpPercentage <= _changeStatePercentages[4];

        _isInitialize = true;
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
        if (_bossHp == null)
        {
            Debug.LogWarning($"{gameObject.name} boss hp is null");
            return null;
        }
        return _bossHp;
    }
}
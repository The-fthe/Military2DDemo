using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using Utility;

public class DeadMode : AIState
{
    [SerializeField]BossAnimatorController _animator;
    [SerializeField] GameObject _bossGO;
    [SerializeField] TurretController[] _turretControllers;
    CancellationTokenSource _cts;

    public override void OnTick()
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Dead mode is trigger");
        _animator ??= FindObjectOfType<BossAnimatorController>();
        _cts = new CancellationTokenSource();
        deathAnimationTrigger(_cts.Token).Forget();
    }

    public override void OnExit()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    async UniTask deathAnimationTrigger(CancellationToken ct)
    {
        foreach (var turretController in _turretControllers)
        {
            turretController.HandleDieMode();
        }
        _animator.ChangeAnimationState(Const.BOSS_DIE_MODE);
        await UniTask.Delay(TimeSpan.FromSeconds(4), cancellationToken: ct);
        _bossGO.SetActive(false);
    }
}

using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts;
using InGameAsset.Scripts.AI;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Util;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossAIPatrolPathBehaviour : AIPatrolPathBehaviour, IState
{
    [SerializeField] PatrolPath bossPatrolPath;

    [SerializeField] BossAnimatorController _bossAnimCtrl;
    TankController _bossTank;
    TurretAimer _turretAimer;
    TargetDetector _targetDetector;
    CancellationTokenSource _cts;
    [SerializeField]bool _startPatrolFire = false;
    [SerializeField]bool _startMoveAndShoot = false;
    
    public void Tick()
    {
        if (_path == null || _path.Length < 2 || _isWaitingToCurrentPathPoint) return;
        if (!_isInitialised)
        {
            _currentPathPoint = _path.GetFirstPathPoint();
            _isInitialised = true;
        }

        if (Vector2.Distance(_tank.transform.position, _currentPathPoint.Position) < PATH_POINT_MOVE_THRESHOLD)
        {
            var nextPathPoint = _path.GetNextPathPoint(_currentPathPoint.Index);
            if (!_isWaitingToCurrentPathPoint)
                _currentPathPoint = nextPathPoint;
            return;
        }
        if (!_isStoppingToScanAround)
        {
            FollowingPath();
            if (!_startPatrolFire)
            {
                OnPatrolFire(_cts.Token).Forget();
            }
        }
        else
            MoveToTargetAndShoot();
    }

    public void Enter()
    {
        patrolPath = bossPatrolPath;
        _targetDetector ??= _parent.GetComponentInChildren<TargetDetector>();
        _bossTank ??= _tank.GetComponent<TankController>();
        _turretAimer ??= _bossTank._turretAimer;
        _bossAnimCtrl ??= _parent.GetComponentInChildren<BossAnimatorController>();
        _currentPatrolPointTarget = patrolPath.GetNextPathPoint(-1);
        _isPatrolling = true;
        _isStoppingToScanAround = false;
        _randomDirection = Random.insideUnitCircle;
        _startPatrolFire = false;
        _startMoveAndShoot = false;
        _cts = new CancellationTokenSource();
       UpdatePathAsync(_cts.Token).Forget();
    }

    public void Exit()
    {
        _isPatrolling = false;
        _tank.HandleMoveBody(Vector2.zero);
        if (!_cts.Token.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
    protected override void FollowingPath()
    {
        Vector2 directionToGo = _currentPathPoint.Position - (Vector2) _tank._tankMover.transform.position;
        var dotProduct = Vector2.Dot(_tank._tankMover.transform.up, directionToGo.normalized);
        if (dotProduct < 0.98)
        {
            var crossProduct = Vector3.Cross(_tank._tankMover.transform.up, directionToGo.normalized);
            int rotationResult = crossProduct.z >= 0 ? -1 : 1;
            _tank.HandleMoveBody(new Vector2(rotationResult, 1));
        }
        else
            _tank.HandleMoveBody(Vector2.up);
    }

    async UniTask  UpdatePathAsync(CancellationToken ct)
     { 
         if (Time.timeSinceLevelLoad < .3f) await UniTask.Delay(TimeSpan.FromSeconds(MIN_PATH_UPDATE_TIME),false,cancellationToken: ct);
        PathRequestManager.RequestPath(new PathRequest(_tank.transform.position, _currentPatrolPointTarget.Position,
            OnPathFound));
        float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
        while (_isPatrolling || !ct.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(PATH_POINT_MOVE_THRESHOLD),false, cancellationToken: ct);
            // print (((Target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
            if ((_currentPatrolPointTarget.Position - (Vector2) _tank.transform.position).sqrMagnitude >
                sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(_tank.transform.position,
                    _currentPatrolPointTarget.Position, OnPathFound));
                if ((_currentPatrolPointTarget.Position - (Vector2) _tank.transform.position).sqrMagnitude <
                    _distanceToStop && !_isStoppingToScanAround)
                {
                    StopAndScanAroundEnumeratorAsync(ct).Forget();
                    Debug.Log("change to next patrol point");
                    _currentPatrolPointTarget = patrolPath.GetNextPathPoint(_currentPatrolPointTarget.Index);
                }
            }
        }
    }

    void  TurretPatrolShoot()
    {
        _turretAimer._turrets.FirstOrDefault(i => i.GetType().Equals(typeof(Turret360AngleStepFire))).Shoot();
    }

    async UniTask OnPatrolFire(CancellationToken ct)
    {
        _startPatrolFire = true;
        _bossAnimCtrl.ChangeAnimationState(Const.BOSS_360_ATTACK);
//        await UniTask.Delay(TimeSpan.FromSeconds(_bossAnimCtrl.GetCurrentAnimDelayTime()),false, cancellationToken: ct);
        await UniTask.Delay(TimeSpan.FromSeconds(2),false, cancellationToken: ct);
        _bossAnimCtrl.ChangeAnimationState(Const.BOSS_IDLE);
        TurretPatrolShoot();
        _startPatrolFire = false;
    }

    void MoveToTargetAndShoot()
    {
        _tank.HandleMoveBody(Vector2.zero);
        Vector2 dirToTarget = (_targetDetector.Target.position - _bossTank.transform.position).normalized;
        float angle = Vector2.Angle(_bossTank._turretAimer.transform.right, dirToTarget);
        if (_currentScanAroundDelay <= 0 || angle < 2)
        {
            //_bossTank.HandleShoot();
            if(!_startMoveAndShoot)
                BarrelFireMode(_cts.Token).Forget();
            //Invoke(nameof(ChangeBackToIdle),_bossAnimCtrl.GetCurrentAnimDelayTime());
            _currentScanAroundDelay = _scanningTime;
        }
        else
        {
            if (_currentScanAroundDelay > 0) _currentScanAroundDelay -= Time.deltaTime;
            _bossTank.HandleTurretMovement(dirToTarget);
        }
    }

    async UniTask BarrelFireMode(CancellationToken ct)
    {
        _startMoveAndShoot = true;
        _bossAnimCtrl.ChangeAnimationState(Const.BOSS_MOVE_AND_SHOOT);
        await UniTask.Delay(TimeSpan.FromSeconds(0.6f), false, cancellationToken: ct);
        _turretAimer._turrets.FirstOrDefault(i => i.GetType().Equals(typeof(TurretShootFromBarrelWithOwnUpdate)))?.Shoot();
        _bossAnimCtrl.ChangeAnimationState(Const.BOSS_IDLE);
        _startMoveAndShoot = false;
    }

    void OnDestroy()
    {
        if (_cts == null) return;
        if (!_cts.IsCancellationRequested) 
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
    void ChangeBackToIdle() => _bossAnimCtrl.ChangeAnimationState(Const.BOSS_IDLE);
}
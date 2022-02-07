using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameAsset.Scripts.AI
{
    public class ChaserAIPatrolPathBehaviour : AIPatrolPathBehaviour,IState
    {
        CancellationTokenSource _cts;

        [SerializeField] PatrolPath chaserPatrolPoint;
        public   void Tick()
        {
            if (_path == null || _path.Length < 2 || _isWaitingToCurrentPathPoint) return;
            if (!_isInitialised)
            {
                _currentPathPoint = _path.GetFirstPathPoint();
                _isInitialised = true;
            }

            if (Vector2.Distance(_tank.transform.position, _currentPathPoint.Position) < PATH_POINT_MOVE_THRESHOLD)
            {
                //_isWaitingToCurrentPathPoint = true;
                var nextPathPoint = _path.GetNextPathPoint(_currentPathPoint.Index);
                //StartCoroutine(WaitCoroutine(nextPathPoint));
                WaitCoroutineAsync(_cts.Token).Forget();
                if (!_isWaitingToCurrentPathPoint)
                    _currentPathPoint = nextPathPoint;
                return;
            }

            if (!_isStoppingToScanAround)
            {
                FollowingPath();
                TurretPatrolShoot(_currentPathPoint.Position);
            }
            else
                ScanAround();
        }

        public  void Enter()
        {
            patrolPath ??= transform.parent.parent.GetComponent<PatrolPath>();
            patrolPath = chaserPatrolPoint;
            _currentPatrolPointTarget = patrolPath.GetClosePathPoint(_tank.transform.position);
            _isPatrolling = true;
            _isStoppingToScanAround = false;
            _randomDirection = Random.insideUnitCircle;
            _cts = new CancellationTokenSource();
             UpdatePathAsync(this.GetCancellationTokenOnDestroy()).Forget();

        }

        public   void Exit()
        {
            _isPatrolling = false;
            _cts.Cancel();
            _cts.Dispose();
            _tank.HandleMoveBody(Vector2.zero);
        }

        async UniTask  UpdatePathAsync(CancellationToken ct)
        {
            if (Time.timeSinceLevelLoad < .3f) await UniTask.Delay(TimeSpan.FromSeconds(MIN_PATH_UPDATE_TIME),false,cancellationToken: ct);
            PathRequestManager.RequestPath(new PathRequest(_tank.transform.position, _currentPatrolPointTarget.Position,
                OnPathFound));
            float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
            while (_isPatrolling)
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
                        //UniTask.Create(()=>StopAndScanAroundEnumeratorAsync(_cts.Token)).Forget();
                        StopAndScanAroundEnumeratorAsync(ct).Forget();
                        _currentPatrolPointTarget = patrolPath.GetNextPathPoint(_currentPatrolPointTarget.Index);
                    }
                }
            }
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
    }
}
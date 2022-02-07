using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameAsset.Scripts.AI
{
    public abstract class AIPatrolPathBehaviour :MonoBehaviour
    {
        protected PatrolPath patrolPath;

        [SerializeField]protected  int _turretOffsetAngle;
        [SerializeField]protected  float _scanningTime = 2f;
        [SerializeField]protected  float _distanceToStop = 3f;
        [SerializeField]protected  TankController _tank;

        protected  Transform _parent;
        protected   PathPoint _currentPathPoint;
        protected   Path _path;
        protected    PathPoint _currentPatrolPointTarget;

       protected bool _isInitialised;
       protected  bool _isWaitingToCurrentPathPoint;
       protected  bool _isPatrolling;
       protected  bool _isStoppingToScanAround;

       [SerializeField]protected  float _turnDst;
         float _curretTurnDelayTime;
       protected  float _currentScanAroundDelay;
         float _turretAngTarget;
         int _turretLeftOrRight = 1;
       protected  Vector2 _randomDirection;

        void Awake()
        {
            _parent = gameObject.transform.parent.parent;
            //patrolPath ??= _parent.GetComponentInChildren<PatrolPath>();
            _tank ??= _parent.GetComponentInChildren<TankController>();
        }

        protected async UniTask  WaitCoroutineAsync(CancellationToken ct)
        {
            _isWaitingToCurrentPathPoint = false;
            await UniTask.Delay(TimeSpan.FromSeconds(MIN_PATH_UPDATE_TIME),false,cancellationToken: ct);
            _isWaitingToCurrentPathPoint = false;
            
           // _currentPathPoint = nextPathPoint;
        }

        protected virtual void  FollowingPath()
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

        protected virtual  void TurretPatrolShoot(Vector2 target)
        {
            if (_curretTurnDelayTime <= 0)
            {
                _turretLeftOrRight = _turretLeftOrRight >= 0 ? -1 : 1;
                var turretDirToTarget = (target - (Vector2) _tank._turretAimer.transform.position).normalized;
                _turretAngTarget = DirToAng(turretDirToTarget);
                _turretAngTarget += _turretOffsetAngle * _turretLeftOrRight;
                _curretTurnDelayTime = 1.5f;
            }
            else if (_curretTurnDelayTime > 0)
                _curretTurnDelayTime -= Time.deltaTime;

            _tank.HandleTurretMovement(_turretAngTarget);
        }

        protected virtual  void  ScanAround()
        {
            _tank.HandleMoveBody(Vector2.zero);
            float angle = Vector2.Angle(_tank._turretAimer.transform.right, _randomDirection);
            if (_currentScanAroundDelay <= 0 || angle < 2)
            {
                _randomDirection = Random.insideUnitCircle;
                _currentScanAroundDelay = _scanningTime * 0.5f;
            }
            else
            {
                if (_currentScanAroundDelay > 0) _currentScanAroundDelay -= Time.deltaTime;
                _tank.HandleTurretMovement(_randomDirection);
            }
        }

        protected async  UniTask StopAndScanAroundEnumeratorAsync(CancellationToken ct)
        {
            _isStoppingToScanAround = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_scanningTime),false, cancellationToken: ct);
            _isStoppingToScanAround = false;
        }

        protected void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                if (_path is null)
                    _path = _parent.GetComponent<Path>();
                _path.Initilization(waypoints, _tank.transform.position, _turnDst, _tank.transform);
                _isWaitingToCurrentPathPoint = false;
                _isInitialised = false;
            }
        }

        float DirToAng(Vector2 dir)
        {
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        public void OnDrawGizmos()
        {
            if (_path != null && _isPatrolling && patrolPath != null)
            {
                _path.DrawWithGizmos();
            }
        }

      //   protected IEnumerator WaitCoroutine(PathPoint nextPathPoint)
      //   {
      //       yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
      //       _isWaitingToCurrentPathPoint = false;
      //       _currentPathPoint = nextPathPoint;
      //   }
      //
      //   protected IEnumerator  StopAndScanAroundEnumerator()
      // {
      //     _isStoppingToScanAround = true;
      //     yield return new WaitForSeconds(_scanningTime);
      //     _isStoppingToScanAround = false;
      // }

      //   protected virtual IEnumerator UpdatePath()
      // {
      //     yield return null;
          // if (Time.timeSinceLevelLoad < .3f) yield return new WaitForSeconds(.3f);
          // PathRequestManager.RequestPath(new PathRequest(_tank.transform.position, _currentPatrolPointTarget.Position,
          //     OnPathFound));
          // float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
          // while (_isPatrolling)
          // {
          //     yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
          //     // print (((Target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
          //     if ((_currentPatrolPointTarget.Position - (Vector2) _tank.transform.position).sqrMagnitude >
          //         sqrMoveThreshold)
          //     {
          //         PathRequestManager.RequestPath(new PathRequest(_tank.transform.position,
          //             _currentPatrolPointTarget.Position, OnPathFound));
          //         if ((_currentPatrolPointTarget.Position - (Vector2) _tank.transform.position).sqrMagnitude <
          //             _distanceToStop && !_isStoppingToScanAround)
          //         {
          //             StartCoroutine(nameof(StopAndScanAroundEnumerator));
          //             Debug.Log("change to next patrol point");
          //             _currentPatrolPointTarget = patrolPath.GetNextPathPoint(_currentPatrolPointTarget.Index);
          //         }
          //     }
           // }
      // }

      protected const float PATH_POINT_MOVE_THRESHOLD = .3f;

      protected const float PATH_UPDATE_MOVE_THRESHOLD = .3f;
        protected  const float MIN_PATH_UPDATE_TIME = .2f;
    }
}
using System.Collections;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class AIChaseBehavior : MonoBehaviour, IState
    {
        public float TotalDistance;
        public bool IsEndOfPath;
        
        public bool _isTimeToGiveUp { get; private set; }
        public Transform Target;
        //[SerializeField] Transform Chaser;
        [SerializeField] float _turnDst = 3;
        [SerializeField] float _turretOffsetAngle = 15f;
        [SerializeField] float _timeToGiveUp=10f;
        
        const float MIN_PATH_UPDATE_TIME = .2f;
        const float PATH_POINT_MOVE_THRESHOLD = 1f;
        const float PATH_UPDATE_MOVE_THRESHOLD = .25f;
        
        Path _path;
       AIDetector _detector;
       TankController _tank;
        bool _isInitialised;
        bool _isWaitingToCurretPathPoint = true;
        bool _isFindingTarget;
        PathPoint _currentPathPoint;

        float curretTurnDelayTime;
        float _TurretAngTarget;
        float _turretLeftOrRight = 1;
        
        public void Awake()
        {
            _detector ??= GetComponentInChildren<AIDetector>();
            _tank ??= GetComponentInChildren<TankController>();
            _detector.OnTargetFind += Set;
        }

       public void Set() => Target = _detector.Target;

       public  void Tick()
        {
            if (_path == null || _path.Length < 2 || _isWaitingToCurretPathPoint) return;
            if (!_isInitialised)
            {
                _currentPathPoint = _path.GetFirstPathPoint();
                _isInitialised = true;
            }

            if (Vector2.Distance(_tank.transform.position, _currentPathPoint.Position) < PATH_POINT_MOVE_THRESHOLD)
            {
                _isWaitingToCurretPathPoint = true;
                var nextPathPoint = _path.GetNextPathPoint(_currentPathPoint.Index);
                StartCoroutine("WaitCoroutine", nextPathPoint);
                return;
            }
            TurentPatrol(_currentPathPoint.Position);
            FollowingPath();
        }

        public  void Enter()
        {
            _turretLeftOrRight = 1;
            IsEndOfPath = false;
            _isTimeToGiveUp = false;
            _isFindingTarget = true;
            StartCoroutine("UpdatePath");
            StartCoroutine(nameof(CountDownIEnumerator));
        }

        public  void Exit()
        {
            IsEndOfPath = false;
            _isFindingTarget = false;
            StopCoroutine(nameof(CountDownIEnumerator));
            _tank.HandleMoveBody(Vector2.zero);
        }

        #region MoveToPath

        void FollowingPath()
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

        void TurentPatrol(Vector2 target)
        {
            if (curretTurnDelayTime <= 0)
            {
                _turretLeftOrRight = _turretLeftOrRight >= 0 ? -1 : 1;
              var  turretDirToTarget = (target - (Vector2) _tank._turretAimer.transform.position).normalized;
                _TurretAngTarget = DirToAng(turretDirToTarget);
                _TurretAngTarget += _turretOffsetAngle * _turretLeftOrRight;
                curretTurnDelayTime = 0.2f;
            }
            else
                if (curretTurnDelayTime > 0) curretTurnDelayTime -= Time.deltaTime;
            _tank.HandleTurretMovement(_TurretAngTarget);
        }

        float DirToAng(Vector2 dir)
        {
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        IEnumerator WaitCoroutine(PathPoint nextPathPoint)
        {
            yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
            _isWaitingToCurretPathPoint = false;
            _currentPathPoint = nextPathPoint;
        }

        #endregion

        #region Update Path

        IEnumerator UpdatePath()
        {
             if (Time.timeSinceLevelLoad < .3f) yield return new WaitForSeconds(.3f);// important to not get null error
             PathRequestManager.RequestPath(new PathRequest(_tank.transform.position, Target.position, OnPathFound));
            float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
            Vector3 targetPosOld = Target.position;
            while (_isFindingTarget)
            {
                yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
                // print (((Target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
                if ((Target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(_tank.transform.position, Target.position, OnPathFound));
                    targetPosOld = Target.position;
                    TotalDistance = _path.TotalDistance;
                    if (_path.isPathEnd(_currentPathPoint.Index)) IsEndOfPath = true;
                }
            }
        }

        void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                if (_path is null)
                    _path = GetComponent<Path>();
                _path.Initilization(waypoints, _tank.transform.position, _turnDst, _tank.transform);
                _isWaitingToCurretPathPoint = false;
                _isInitialised = false;
            }
        }

        #endregion

        IEnumerator CountDownIEnumerator()
        {
            yield return new WaitForSeconds(_timeToGiveUp);
            _isTimeToGiveUp = true;
        }
        void OnDisable()
        {
            _detector.OnTargetFind -= Set;
        }
        public void OnDrawGizmos()
        {
            if (_path != null && _isFindingTarget && Target != null)
            {
                _path.DrawWithGizmos();
            }
        }
    }
}
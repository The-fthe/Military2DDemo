using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Random = UnityEngine.Random;

[Serializable]
struct RotationData
{
    [Range(0, 360)] public float _minRotatePerSecond;
    [Range(0, 360)] public float _maxRotatePerSecond;
    [Range(0, 360)] public float _rotatePerSecond ;
    public bool _clockWiseRotation ;
}

public class TowerBossFireMode : AIState
{
    [SerializeField] Transform _bossTurretParent;
    [SerializeField] Transform _target;
    [SerializeField] UnitStatus _bossUnit;
    [SerializeField] RotationData _rotationData;
    [SerializeField] int _bulletFirePerSecond = 1;
    [SerializeField, Range(0, 180)] int _aimViewAngle = 30;
    [SerializeField] float _attackModeChangeTime = 5f;
    [SerializeField] BossMode _currentBossMode = BossMode.CircleFire;
    [SerializeField] float _spawnTimeDelay=2f;
    [SerializeField] BoolGameData _couldEnemyStart;
    [SerializeField] bool _isActivated;
    [SerializeField] bool _isChangingPos;
    [SerializeField] TurretController[] _turrets;
    [SerializeField] EnemySpawnPoint[] _movePoints;
    [SerializeField] BulletData _bulletData;

    ObjectPool _bulletPool;
    CancellationTokenSource _cts;
    public UnityEvent OnShoot;

    void Start()
    {
        _cts = new CancellationTokenSource();
        _bulletPool = Instantiate(_bulletData.ObjectPool, gameObject.transform, true);
        _bulletPool.SetObjectPoolAndPoolSize(null, _bulletData.Capacity * 2);
    }
    public override void OnTick()
    {
        if (_couldEnemyStart.RunTimeValue && !_isChangingPos)
        {
            RotateAndFireModeCountDown();
        }
        FireMode();
    }

    public override void OnEnter()
    {
        _isActivated = true;
       // _turrets =_turrets.Length<=0? _bossParent.GetComponentsInChildren<TurretController>() : _turrets;
        //_movePoints = _movePoints.Length <= 0 ? FindObjectsOfType<EnemySpawnPoint>() : _movePoints;
        RandomSwitchToSpawnPoint(_cts.Token).Forget();
       //ChangePositionTimer(_cts.Token).Forget();
        FindTarget();
        foreach (var turret in _turrets)
            turret.SetTurretBulletData(_bulletData,_bulletPool);
    }

    void FindTarget()
    {
        _target = GameObject.Find(Const.PLAYER_MOVE_UNIT_NAME).transform;
    }

    public override void OnExit()
    {
        _couldEnemyStart.RunTimeValue = false;
        _isActivated = false;
        
        foreach (var turret in _turrets)
        {
            turret.HandleHideMode();
        }

        if (_bossUnit._currentHealthData.RunTimeValue <= 0)
        {
            if (_bossUnit._currentHealthData.RunTimeValue <= 0)
            {
                foreach (var objectToReturn in _bulletPool._objectPoolGOs.ToArray())
                {
                    if(objectToReturn.activeSelf)
                        _bulletPool.ReturnToPool(objectToReturn);    
                }
            }
        }
        if (_cts == null) return;
        _cts.Cancel();
        _cts.Dispose();
    }

    void FireMode()
    {
        if (_isChangingPos) return;
        switch (_currentBossMode)
        {
            case BossMode.CircleFire:
                HandleCircleFire();
                HandleShootSound();
                break;
            case BossMode.AimFire:
                HandleAimFire();
                HandleShootSound();
                break;
            case BossMode.TranferMode:
                RandomSwitchToSpawnPoint(_cts.Token).Forget();
                break;
        }
    }
    float shootTimeSound= 0;
    void HandleShootSound()
    {
        if (shootTimeSound <= Time.time )
        {
            shootTimeSound = Time.time + 1;
            OnShoot?.Invoke();
        }
    }

    void RotateAndFireModeCountDown()
    {
        //if (_couldEnemyStart.RunTimeValue && _currentBossMode != BossMode.TranferMode)
            RotateBoss(_rotationData._rotatePerSecond, _rotationData._clockWiseRotation);
    }

    void HandleRotateAndFireModeSwitch()
    {
        _rotationData._rotatePerSecond = RandomiseRotation();
        _rotationData._clockWiseRotation = RandomiseClockwise();
        _currentBossMode = RandomiseShootMode();
    }

    void RotateBoss(float rotateRatePerSecond, bool clockwiseMode = true)
    {
        int clockwork = clockwiseMode ? -1 : 1;
        _bossTurretParent.transform.Rotate(Vector3.forward * clockwork, rotateRatePerSecond * Time.deltaTime);
    }


    async UniTask RandomSwitchToSpawnPoint(CancellationToken ct)
    {
        _couldEnemyStart.RunTimeValue = false;
        _isChangingPos = true;
        _bossUnit.tag = "IgnoreEnemy";
        foreach (var turret in _turrets)
        {
            turret.HandleHideMode();
        }
        _bossUnit.WasInvinsibleFinish = false;
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
        _bossTurretParent.transform.position = _movePoints[RandomSelectSpawnPoint()].Pos;
        _bossTurretParent.transform.SetPositionAndRotation(_movePoints[RandomSelectSpawnPoint()].Pos,Quaternion.Euler(0,0,0));
        await UniTask.Delay(TimeSpan.FromSeconds(RandomiseSpawnTime()), cancellationToken: ct);
        foreach (var turret in _turrets)
        {
            turret.HandleShowMode();
        }
        _bossUnit.WasInvinsibleFinish = true;
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
        HandleRotateAndFireModeSwitch();
        _bossUnit.tag = "Boss";
        _couldEnemyStart.RunTimeValue = true;
        _isChangingPos = false;
        ChangePositionTimer(ct).Forget();
    }

    int RandomSelectSpawnPoint()
    {
        var index = Random.Range(0, _movePoints.Length - 1);
        if (_movePoints[index] != null)
        {
            while (_movePoints[index].isPlayerOnTop() && _isActivated)
            {
                index = Random.Range(0, _movePoints.Length - 1);
                Debug.Log($"move point index {index} is found");
            }
        }

        return index;
    }

    async UniTask ChangePositionTimer(CancellationToken ct)
    {
            await UniTask.Delay(TimeSpan.FromSeconds(_attackModeChangeTime), cancellationToken: ct);
            _currentBossMode = BossMode.TranferMode;
    }

    void HandleCircleFire()
    {
        foreach (var turret in _turrets)
        {
            turret.HandleShoot(_bulletFirePerSecond);
        }
    }

    void HandleAimFire()
    {
        foreach (var turret in _turrets)
        {
            var turretFaceDirection = (_target.position - turret.BarrelTrans.position).normalized;
            var dot = Vector2.Dot(turret.BarrelTrans.up, turretFaceDirection);
            var angleToDotPercenatage = _aimViewAngle / 180;
            if (dot >= angleToDotPercenatage)
            {
                turret.HandleShoot(_bulletFirePerSecond);
            }
        }
    }

    float RandomiseRotation() => Random.Range(_rotationData._minRotatePerSecond, _rotationData._maxRotatePerSecond);
    bool RandomiseClockwise() => Random.Range(-1, 1) >= 0;
    float RandomiseSpawnTime() => Random.Range(_spawnTimeDelay - 1f, _spawnTimeDelay + 1f);

    BossMode RandomiseShootMode()
    {
        int randomRange = 0;
        randomRange = Random.Range(1, 4);
        return randomRange <= 2 ? BossMode.CircleFire : BossMode.AimFire;
    }

    void OnDisable()
    {
        _isActivated = false;
   
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

    enum BossMode
    {
        CircleFire,
        AimFire,
        TranferMode
    }
}
using System;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Tank.Turrets;
using InGameAsset.Scripts.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class Level1BossStateController : SerializedMonoBehaviour,IState
    {
        [SerializeField] TargetDetector _detector;
        [SerializeField] float _distanceAlertFromOrigin = 10f;
        [SerializeField] Transform _target;
        [SerializeField] string StateName;
        [SerializeField] BulletData _turretBulletData;
        [SerializeField] BulletData _turret360BulletData;
        [SerializeField] BulletData _turretAngleBulletData;
        [SerializeField] ObjectPool _turretBulletPool;
        [SerializeField] ObjectPool _turret360BulletPool;
        [SerializeField] ObjectPool _turretAngleBulletPool;
        [SerializeField] TurretShootFromBarrelWithOwnUpdate _turret;
        [SerializeField] Turret360AngleStepFire _turret360;
        [SerializeField] TurretFireOnAngle _turretFireOnAngle;
        StateMachine _stateMachine;
        BossAIPatrolPathBehaviour _bossAIPatrolPathBehaviour;
        
        bool isInitialize;

        void Start()
        {
            _turretBulletPool = Instantiate(_turretBulletData.ObjectPool, gameObject.transform, true);
            _turretBulletPool.SetObjectPoolAndPoolSize(null, _turretBulletData.Capacity * 2);
            _turret360BulletPool = Instantiate(_turret360BulletData.ObjectPool, gameObject.transform, true);
            _turret360BulletPool.SetObjectPoolAndPoolSize(null, _turretBulletData.Capacity * 2);
            _turretAngleBulletPool = Instantiate(_turretAngleBulletData.ObjectPool, gameObject.transform, true);
            _turretAngleBulletPool.SetObjectPoolAndPoolSize(null, _turretBulletData.Capacity * 2);
        }

        public Transform SetTargetTrans
        {
            get { return _target;}
            set => _target = value;
        }
        public void Tick()
        {
            if(isInitialize)
                _stateMachine.Tick();
        }

        public void Enter()
        {
           Debug.Log($"Enter {StateName} ");
           StateName = gameObject.name;
           var parent = gameObject.transform.parent.transform.parent;
          // _target =GameObject.Find(Const.PLAYER_MOVE_UNIT_NAME).transform;
          _turret360.Initialized(_turret360BulletData,_turret360BulletPool);
          _turretFireOnAngle.Initialized(_turretAngleBulletData,_turretAngleBulletPool);
          _turret.Initialized(_turretBulletData,_turretBulletPool);
          
           _detector = parent.GetComponentInChildren<TargetDetector>();
           _detector.Initialized(_target);
           
           _bossAIPatrolPathBehaviour = GetComponent<BossAIPatrolPathBehaviour>();
           var standAndShoot = GetComponent<BossNearBySpreadingShootBehaviour>();
           _stateMachine = new StateMachine();
            
           At(_bossAIPatrolPathBehaviour,standAndShoot,()=>_detector.isTargetNearBy(_distanceAlertFromOrigin));
           At(standAndShoot,_bossAIPatrolPathBehaviour,()=>!_detector.isTargetNearBy(_distanceAlertFromOrigin));
           _stateMachine.SetState(_bossAIPatrolPathBehaviour);

           void At(IState from, IState to, Func<bool> condition)
               => _stateMachine.AddTransition(from, to, condition);
           isInitialize = true;
        }

        public void Exit()
        {
            _detector.Destruction();
            _stateMachine.ExitCurrentState();
            Debug.Log($"Exited {StateName}");
        }
    }
}
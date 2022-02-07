using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameAsset.Scripts.AI
{
    public class AIPatrolStaticBehaviour : MonoBehaviour, IState
    {
        public float patrolDelay = 4;
        [SerializeField] Vector2 randomDirection = Vector2.zero;
        [SerializeField] float currentPatrolDelay;

        TankController _tank;
        AIDetector _aiDetector;
        public void Awake()
        {
            _tank = GetComponentInChildren<TankController>();
            _aiDetector = GetComponentInChildren<AIDetector>();
        }
        
        public  void Tick()
        {
            _tank.HandleMoveBody(Vector2.zero);
            float angle = Vector2.Angle(_tank._turretAimer.transform.right, randomDirection);
            if (currentPatrolDelay <= 0 && angle < 2)
            {
                randomDirection = Random.insideUnitCircle;
                currentPatrolDelay = patrolDelay;
            }
            else
            {
                if (currentPatrolDelay > 0) currentPatrolDelay -= Time.deltaTime;
                else
                {
                    _tank.HandleTurretMovement(randomDirection);
                    //detector.Aim(aimPosition);
                }
            }
        }

        public  void Enter()
        {
            randomDirection = Random.insideUnitCircle;
        }

        public  void Exit()
        {
        }
    }
}
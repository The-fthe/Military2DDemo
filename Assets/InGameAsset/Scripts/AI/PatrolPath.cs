using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameAsset.Scripts.AI
{
    public class PatrolPath : MonoBehaviour
    {
        public List<Transform> patrolPoints = new List<Transform>();
        public int Length => patrolPoints.Count;

        [Header("Gizmoz parameters")] 
        Color[] pointsColors= new Color[14];
        public float pointSize = 0.3f;
        public Color lineColor = Color.yellow;
        
            
        public PathPoint GetClosePathPoint(Vector2 tankPosition)
        {
            var minDistance = float.MaxValue;
            var index = -1;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                var tempDistance = Vector2.Distance(tankPosition, patrolPoints[i].position);
                if (tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    index = i;
                }
            }

            return new PathPoint (index,  patrolPoints[index].position);
        }

        public PathPoint GetNextPathPoint(int index)
        {
            var newIndex = index + 1 >= patrolPoints.Count ? 0 : index + 1;
            return new PathPoint (newIndex, patrolPoints[newIndex].position);
        }

        void OnDrawGizmos()
        {
             if (patrolPoints.Count == 0) return;
             for (int i  = patrolPoints.Count - 1; i >= 0 ; i--)
             {
                 if (i == -1 || patrolPoints[i] is null) return;
                 pointsColors[i] = new Color(Random.Range(0,256), Random.Range(0,256), Random.Range(0,256), 1);
                 Gizmos.color = pointsColors[i];
                 Gizmos.DrawSphere(patrolPoints[i].position, pointSize);
                 if (patrolPoints.Count == 1 || i == 0) return;
                     Gizmos.color = lineColor;
                 Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i-1].position);
                 if(patrolPoints.Count > 2 && i== patrolPoints.Count-1)
                     Gizmos.DrawLine(patrolPoints[i].position,patrolPoints[0].position);
            }
        }
    }
}
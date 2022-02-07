using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class Path : MonoBehaviour
{
    public int Length => patrolPoints.Length;
    public float pointSize = 0.3f;
    public float TotalDistance;
    [SerializeField] Color lineColor = Color.yellow;
    [SerializeField] PathCreator _pathCreator;
    Transform _chaser;
    Vector3[] patrolPoints;
    Vector3[] LookPoints;
    Vector3[] BezierPath;
    Line[] TurnBoundaries;
    int FinishLineIndex;
    float PathPointMoveThreshold = 1f;
    float _stoppingDst;

    public bool isPathEnd(int pathIndex) => pathIndex == patrolPoints.Length - 1;

    public void Initilization(Vector3[] waypoint, Vector3 startPos, float turnDst, Transform chaser)
    {
        LookPoints = waypoint;
        TurnBoundaries = new Line[LookPoints.Length];
        FinishLineIndex = TurnBoundaries.Length - 1;
        _chaser = chaser;

        Vector2 previousPoint = V3ToV2(startPos);
        for (int i = 0; i < LookPoints.Length; i++)
        {
            Vector2 currentPoint = V3ToV2(LookPoints[i]);
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint =
                (i == FinishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;
            TurnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
            previousPoint = turnBoundaryPoint;
        }

        GeneratedBezierPathPoint();
        GetTotalDistance();
    }

    public void GetTotalDistance()
    {
        {
            var dstFromEndPoint = 0f;
            // dstFromEndPoint = PathPointMoveThreshold* patrolPoints.Length;
            for (int i = patrolPoints.Length - 1; i > 0; i--)
            {
                dstFromEndPoint += Vector2.Distance(patrolPoints[i], patrolPoints[i - 1]);
            }
            TotalDistance = dstFromEndPoint;
        }
    }

    public PathPoint GetFirstPathPoint() => new PathPoint {Index = 0, Position = patrolPoints[0]};

    public PathPoint GetNextPathPoint(int index)
    {
        var newIndex = index + 1 >= patrolPoints.Length ? patrolPoints.Length - 1 : index + 1;
        return new PathPoint {Index = newIndex, Position = patrolPoints[newIndex]};
    }

    void GeneratedBezierPathPoint()
    {
        var bezierPath = GetUnitBezierPath(LookPoints);
        _pathCreator.bezierPath = bezierPath;
        VertexPath vertexPath = new VertexPath(bezierPath, _chaser, PathPointMoveThreshold);
        patrolPoints = vertexPath.localPoints;
    }

    BezierPath GetUnitBezierPath(Vector3[] paths)
    {
        if (paths.Length < 3)
        {
            BezierPath = new Vector3[paths.Length + 2];
            BezierPath[paths.Length + 1] = paths[paths.Length - 1];
        }
        else
        {
            BezierPath = new Vector3[paths.Length + 1];
        }
        for (int i = 0; i < paths.Length; i++)
        {
            BezierPath[0] = _chaser.position;
            BezierPath[i + 1] = paths[i];
        }
        BezierPath bezierPath = new BezierPath(BezierPath);
        return bezierPath;
    }


    Vector2 V3ToV2(Vector3 v3) => new Vector2(v3.x, v3.y);

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (var point in LookPoints)
        {
            Gizmos.DrawCube(point, Vector3.one * 0.25f);
        }

        Gizmos.color = Color.white;
        foreach (var line in TurnBoundaries)
        {
            line.DrawWithGizmos(10);
        }

        if (patrolPoints.Length == 0) return;
        for (int i = patrolPoints.Length - 1; i >= 0; i--)
        {
            if (i == patrolPoints.Length - 1)
                Gizmos.color = Color.blue;
            Gizmos.DrawSphere(patrolPoints[i], pointSize);
            if (patrolPoints.Length == 1 || i == 0) return;
            Gizmos.color = lineColor;
            Gizmos.DrawLine(patrolPoints[i], patrolPoints[i - 1]);
        }
    }
}
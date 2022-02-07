using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    const float verticalLineGradient = 1e5f;
    float _gradient;
    float _y_intercept;
    float _gradientPerpendicular;

    Vector2 pointOnline_1;
    Vector2 pointOnline_2;

    bool approachSide;
     public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
     {
         float dx = pointOnLine.x - pointPerpendicularToLine.x;
         float dy = pointOnLine.y - pointPerpendicularToLine.y;
         if (dx == 0)
         {
             _gradientPerpendicular = verticalLineGradient;
         }
         else
         {
             _gradientPerpendicular = dy / dx;
         }
        if(_gradientPerpendicular == 0)
        {
            _gradient = verticalLineGradient;
        }else
         _gradient = -1 / _gradientPerpendicular;

        _y_intercept = pointOnLine.y - _gradient * pointOnLine.x;
        pointOnline_1 = pointOnLine;
        pointOnline_2 = pointOnLine + new Vector2(1, _gradient);
        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
     }

     bool GetSide(Vector2 p)
     {
         return (p.x - pointOnline_1.x) * (pointOnline_2.y - pointOnline_1.y) >
                (p.y - pointOnline_1.y) * (pointOnline_2.x - pointOnline_1.x);
     }

     public bool HasCrossedLine(Vector2 p)
     {
         return GetSide(p) != approachSide;
     }

     public float DistanceFromPoint(Vector2 point)
     {
         float yInterceptPerpendicular = point.y - _gradientPerpendicular * point.x;
         float intersectX = (yInterceptPerpendicular - _y_intercept) / (_gradient - _gradientPerpendicular);
         float intersectY = _gradient * intersectX + _y_intercept;
         return Vector2.Distance(point, new Vector2(intersectX, intersectY));
     }
     public void DrawWithGizmos(int length)
     {
         Vector3 lineDir = new Vector3 (1, _gradient,0).normalized;
         Vector3 lineCentre = new Vector3 (pointOnline_1.x, pointOnline_1.y,0 );
         Gizmos.DrawLine (lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
     }
}

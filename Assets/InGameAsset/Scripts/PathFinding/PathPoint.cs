using UnityEngine;

public struct PathPoint
{
    public PathPoint(int index, Vector2 position)
    {
        Index = index;
        Position = position;
    }
    public int Index;
    public Vector2 Position;
}
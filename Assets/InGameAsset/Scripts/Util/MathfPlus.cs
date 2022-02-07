using UnityEngine;

public  struct MathfPlus
{
    public static float DirToAng(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public static Vector2 AngToDir(float angleDeg)
    {
        return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
    }
    public static Quaternion DirToRotationZAxis(Vector2 dir)
    {
        return Quaternion.Euler(0,0,DirToAng(dir)); 
    }
    public static Quaternion AngToRotationZAxis(float angleDeg)
    {
        return Quaternion.Euler(0,0,angleDeg); 
    }
}
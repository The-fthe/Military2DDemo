using UnityEngine;

public abstract class SpawnPoint:MonoBehaviour
{
    public Vector3 Pos => transform.position;
}
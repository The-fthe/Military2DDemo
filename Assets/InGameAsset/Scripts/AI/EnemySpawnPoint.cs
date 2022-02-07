using System;
using UnityEngine;

public class EnemySpawnPoint : SpawnPoint
{
    [SerializeField] bool _isOnTop;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            _isOnTop = true;
        else
        {
            _isOnTop= false;
            Debug.LogWarning("player is not onTop");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
       if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            _isOnTop = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            _isOnTop = true;
        else
        {
            _isOnTop= false;
            Debug.LogWarning("player is not onTop");
        }
    }

    public bool isPlayerOnTop()
    {
        return _isOnTop;
    }
}
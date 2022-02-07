using UnityEngine;

public class PlayerOverlapDetector : MonoBehaviour
{
    CircleCollider2D _circleCollider2D;
    public bool IsPlayerOnTop { get; private set; }
    void Start()
    {
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        DetectPlayerOverStack(other);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DetectPlayerOverStack(other);
    }

    void DetectPlayerOverStack(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IsPlayerOnTop = true;
        }
        else
        {
            IsPlayerOnTop = false;
        }
    }
}

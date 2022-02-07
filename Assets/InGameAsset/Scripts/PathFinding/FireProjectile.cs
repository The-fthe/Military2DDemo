using System;
using Unity.Mathematics;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] GameObject _bullet;
    [SerializeField] Transform _FirePoint;
    [SerializeField] GameObject _hitPoint;

    public Vector2 Direction { get; private set; }
    public int _moveSpeed = 10;

    Vector3 _MousePos;
    Camera cam;
    Vector2 _reflectedDirection;
    Vector2 _reflectedPos;
    RaycastHit2D hit;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var bullet = Instantiate(_bullet, _FirePoint.transform.position, _FirePoint.transform.rotation);
        }
    }
    void FixedUpdate()
    {
  
        hit = Physics2D.Raycast(_FirePoint.transform.position, Direction);
        _hitPoint.transform.position = hit.point;
        if (hit.collider != null)
        {
            _reflectedPos = hit.point;
            _reflectedDirection = Vector2.Reflect(Direction, hit.normal);
        }

        Debug.DrawRay(_FirePoint.position, Direction.normalized * hit.distance, Color.red);
        Debug.DrawRay(_reflectedPos, _reflectedDirection, Color.blue);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_reflectedPos, 0.2f);
    }
}
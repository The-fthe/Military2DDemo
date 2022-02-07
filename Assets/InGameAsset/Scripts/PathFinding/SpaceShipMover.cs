using System;
using UnityEngine;

public class SpaceShipMover:MonoBehaviour
{
    Vector3 _MousePos;
    Camera cam;
    public float speed = .2f;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] float _moveSpeed = 100f;
    public Vector2 Direction { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(Time.fixedDeltaTime * _moveSpeed * Input.GetAxis("Horizontal"),
            _moveSpeed * Time.fixedDeltaTime * Input.GetAxis("Vertical"));
        Movement();

    }

    void Movement()
    {
        _MousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Direction = _MousePos - transform.position;
        var angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}
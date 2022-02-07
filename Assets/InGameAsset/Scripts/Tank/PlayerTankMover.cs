using System;
using System.Collections;
using System.Collections.Generic;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Data;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerTankMover : TankMover
{
    public Rigidbody2D rb2d;
    public TankMovementData MovementData;
    [SerializeField] float currentSpeed = 5;
    Vector2 movementVector;
    
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // public void OnMove(InputValue inputValue)
    // {
    //     movementVector = inputValue.Get<Vector2>(); 
    //     Move(movementVector);
    // }
    
    public override void Move(Vector2 movementVector2)
    {
        this.movementVector = movementVector2;
        if (movementVector2 == Vector2.zero) currentSpeed = 0;
        currentSpeed = MovementData.maxSpeed;
    }

    void OnDisable()
    {
        movementVector =Vector2.zero;
        rb2d.velocity = Vector2.zero;
    }

    void OnEnable()
    {
        movementVector =Vector2.zero;
        rb2d.velocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        //rb2d.MovePosition(rb2d.position + movementVector*5* Time.fixedDeltaTime);
        rb2d.velocity =  movementVector * currentSpeed * Time.deltaTime;
        if (movementVector != Vector2.zero)
        {
            float zAngle = Mathf.Atan2(movementVector.x, movementVector.y) * Mathf.Rad2Deg * -1;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, zAngle)
                , MovementData.rotationSpeed*Time.fixedDeltaTime);
        }
    }
}
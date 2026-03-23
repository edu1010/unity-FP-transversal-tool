using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    public float Speed = 0.1f;
    float _currentSpeed;
    public float MaxSpeed = 20f;
    private float _horizonantal;
    WallJump wallJump;
    Vector2 dir;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        wallJump = GetComponent<WallJump>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!wallJump.wallJumping)
        {
            Vector2 vel = _rigidbody.linearVelocity;
            vel.x = _horizonantal * Speed;
            _rigidbody.linearVelocity = vel;
        }
        dir = _rigidbody.linearVelocity.normalized;
        _currentSpeed = _rigidbody.linearVelocity.magnitude;
        _rigidbody.linearVelocity = dir * Mathf.Min(_currentSpeed, MaxSpeed);
    

    }

    void OnMove(InputValue input)
    {
        
        var inVal = input.Get<Vector2>();
        _horizonantal = inVal.x;
        
    }


    }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    [SerializeField]
    public float MaxSpeed = 12f;
    [SerializeField]
    public float Acceleration = 80f;
    [SerializeField]
    public float Deceleration = 90f;
    [SerializeField]
    [Range(0f, 1f)]
    public float AirControl = 0.7f;
    private float _horizonantal;
    WallJump wallJump;
    CollisionDetection _detector;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        wallJump = GetComponent<WallJump>();
        _detector = GetComponent<CollisionDetection>();

        if (_rigidbody == null || wallJump == null || _detector == null)
        {
            Debug.LogError($"[PlayerMovement] '{name}' requires Rigidbody2D, WallJump and CollisionDetection on the same object. Disabling.", this);
            enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!wallJump.wallJumping)
        {
            Vector2 vel = _rigidbody.linearVelocity;
            float targetSpeed = _horizonantal * MaxSpeed;
            float speedDiff = targetSpeed - vel.x;
            float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? Acceleration : Deceleration;

            if (!_detector.IsGrounded)
            {
                accelRate *= AirControl;
            }

            vel.x += speedDiff * accelRate * Time.fixedDeltaTime;
            vel.x = Mathf.Clamp(vel.x, -MaxSpeed, MaxSpeed);
            _rigidbody.linearVelocity = vel;
        }
    }

    void OnMove(InputValue input)
    {
        
        var inVal = input.Get<Vector2>();
        _horizonantal = inVal.x;
        
    }


    }

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jumper : MonoBehaviour
{
    Rigidbody2D rigidbody;
    CollisionDetection _detector;
    [SerializeField]
    float jumpSpeed = 5;
    //Y
    public float JumpHeight = 3;
    public float TimeToPeak = 0.5f;
    //X
    public float PeakDistance = 2;
    public float MaxSpeedHorizontal = 5;

    public bool X = true;//Si es true las funciones se hacen en funcion de la horizontal sino de la vertical
    public float PressTimeForMaxJump = 0.15f;
    public int _currentJumps;
    public int NumberOfJumps = 1;
    private float _jumpStartTime;
    WallJump wallJump;
    [SerializeField]
    private float CoyoteTime = 0.1f;
    [SerializeField]
    private float JumpBufferTime = 0.1f;
    [SerializeField]
    private float FallGravityMultiplier = 2.2f;
    [SerializeField]
    private float LowJumpGravityMultiplier = 1.8f;
    [SerializeField]
    private float MaxFallSpeed = 25f;

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _jumpHeld;
    private float _baseGravityScale = 1f;



    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _detector = GetComponent<CollisionDetection>();
        wallJump = GetComponent<WallJump>();
        SetGravity();
    }

    private void OnEnable()
    {
        _detector.OnLanding += OnLanded;
    }
    private void OnDisable()
    {
        _detector.OnLanding -= OnLanded;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateTimers();

        if (_jumpBufferTimer > 0f)
        {
            TryJump();
        }

        ApplyBetterJumpGravity();
    }

   

    void OnJumpStarted()
    {
        _jumpBufferTimer = JumpBufferTime;
        _jumpHeld = true;
    }
    void OnJumpFinished()
    {
        _jumpHeld = false;
    }

    private void TryJump()
    {
        if (wallJump.IsWallSliding)
        {
            wallJump.BeginWallJump();
            WallJump();
        }
        else
        {
            if (_detector.IsGrounded || _coyoteTimer > 0f || _currentJumps < NumberOfJumps)
            {

                Jump();
            }
        }
        _jumpBufferTimer = 0f;
    }
    void Jump()
    {
        _currentJumps++;
        _jumpStartTime = Time.time;
        SetGravity();
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, getJumpSpeed());
    }
    void WallJump()
    {
        SetGravity();
        _currentJumps = 1;
        _jumpStartTime = Time.time;
        //rigidbody.AddForce(new Vector2(wallJump.NextWall * wallJump.WallJumpSpeed, 2), ForceMode2D.Impulse);
        //rigidbody.velocity = new Vector2(rigidbody.velocity.x, getJumpSpeed());
        float jumpDirection = Mathf.Sign(wallJump.NextWall);
        transform.localScale = new Vector3(jumpDirection, transform.localScale.y, transform.localScale.z);
        rigidbody.linearVelocity = new Vector2(jumpDirection * wallJump.WallJumpSpeed, getJumpSpeed());
        
        
    }
    private void SetGravity()
    {
        //La gravedad es -2 veces la altura dividido entre el tiempo
        //GravityScale multipla por la gravedad del juego en este caso -9,8 
        //por lo tanto tenenemos que dividir el resultado entre la gravedad total
        float gravity = -2f * JumpHeight / (TimeToPeak * TimeToPeak);
        _baseGravityScale = gravity / Physics2D.gravity.y;
        rigidbody.gravityScale = _baseGravityScale;
        
    }

    float getJumpSpeed()
    {
        //Velocidad = 2 veces la altura maxima divididio entre el tiempo
        return 2 * JumpHeight / TimeToPeak;

    }


    void OnLanded()
    {
        _currentJumps = 0;
        rigidbody.gravityScale = _baseGravityScale;
    }

    private void UpdateTimers()
    {
        if (_detector.IsGrounded)
        {
            _coyoteTimer = CoyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.fixedDeltaTime;
        }

        if (_jumpBufferTimer > 0f)
        {
            _jumpBufferTimer -= Time.fixedDeltaTime;
        }
    }

    private void ApplyBetterJumpGravity()
    {
        if (wallJump.wallJumping)
        {
            return;
        }

        Vector2 vel = rigidbody.linearVelocity;
        if (vel.y < 0)
        {
            rigidbody.gravityScale = _baseGravityScale * FallGravityMultiplier;
        }
        else if (vel.y > 0 && ShouldCutJump())
        {
            rigidbody.gravityScale = _baseGravityScale * LowJumpGravityMultiplier;
        }
        else
        {
            rigidbody.gravityScale = _baseGravityScale;
        }

        if (vel.y < -MaxFallSpeed)
        {
            vel.y = -MaxFallSpeed;
            rigidbody.linearVelocity = vel;
        }
    }

    private bool ShouldCutJump()
    {
        return !_jumpHeld && (Time.time - _jumpStartTime) < PressTimeForMaxJump;
    }
}

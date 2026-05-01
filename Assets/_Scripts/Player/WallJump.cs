using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class WallJump : MonoBehaviour, IWallJump
{
    Rigidbody2D rigidbody;
    CollisionDetection _detector;

    public bool Wallsliding => _detector.IsTouchingRightWall;
    public bool WallslidingInvers => _detector.IsTouchingLeftWall;//Por si se gira el jugador
    public bool IsWallSliding => !_detector.IsGrounded && (Wallsliding || WallslidingInvers) && rigidbody.linearVelocity.y <= 0f;

    public float WallSlideSpeed = 2;
    public bool WallsGrounded => _detector.IsGrounded;
    public bool wallJumping = false;
    public bool wasWallJumping = false;
    public int NextWall = -1; //-1 derecha 1 izquierda
    public float WallJumpSpeed = 10;
    public float contador = 0.02f;
    private float _contador = 0.02f;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _detector = GetComponent<CollisionDetection>();
        _contador = contador;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateWallJumpTimer();

        if (IsWallSliding)
        {
            // If touching right wall, jump must push to the left and vice versa.
            NextWall = Wallsliding ? -1 : 1;
            SetWallSlide();
        }
    }

    private void SetWallSlide()
    {
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x,
            Mathf.Clamp(rigidbody.linearVelocity.y, -WallSlideSpeed,float.MaxValue));
    }

    public void OnMove(InputValue value)
    {
        _ = value;
        // Input is intentionally not needed for wall slide, kept for interface compatibility.
    }

    public void BeginWallJump()
    {
        wallJumping = true;
        contador = _contador;
    }

    private void UpdateWallJumpTimer()
    {
        if (!wallJumping)
        {
            return;
        }

        if (WallsGrounded)
        {
            wallJumping = false;
            contador = _contador;
            return;
        }

        contador -= Time.fixedDeltaTime;
        if (contador <= 0f)
        {
            wallJumping = false;
            contador = _contador;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class WallJump : MonoBehaviour, IWallJump
{
    Rigidbody2D rigidbody;
    CollisionDetection _detector;

    bool PlayerMoveToTheWall = false;

    public bool Wallsliding => _detector.IsTouchingRightWall;
    public bool WallslidingInvers => _detector.IsTouchingLeftWall;//Por si se gira el jugador

    public float WallSlideSpeed = 2;
    public bool WallsGrounded => _detector.IsGrounded;
    public bool wallJumping = false;
    public bool wasWallJumping = false;
    public int NextWall = -1; //-1 derecha 1 izquierda
    public float WallJumpSpeed = 15;
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
        if (Wallsliding || WallslidingInvers)
        {
            if (Wallsliding)
            {
                if (transform.localScale.x == -1)
                {
                    NextWall = 1;
                }
                else
                {
                    NextWall = -1;
                }

                if (PlayerMoveToTheWall)
                {
                    SetWallSlide();
                }
                wallJumping = true;

            } else
            {
                if (transform.localScale.x == -1)
                {
                    NextWall = -1;
                }
                else
                {
                    NextWall = 1;
                }
                wallJumping = true;
            }
            
        }
       
        else
        {
            if(wallJumping == true)
            {
                contador -= Time.deltaTime;
                if ((!WallsGrounded) && contador <= 0)
                {
                    wallJumping = false;
                    contador = _contador;
                }
            }
            
               
        }

    }

    private void SetWallSlide()
    {
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x,
            Mathf.Clamp(rigidbody.linearVelocity.y, -WallSlideSpeed,float.MaxValue));
    }

    public void OnMove(InputValue value)
    {
        if (value.Get<Vector2>().x != 0)
        {
            PlayerMoveToTheWall = true;
        }
        else
        {
            PlayerMoveToTheWall = false;
        }
    }
}


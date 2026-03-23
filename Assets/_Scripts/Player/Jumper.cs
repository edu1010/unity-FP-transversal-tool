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
    public float TimeToPeak = 2;
    //X
    public float PeakDistance = 2;
    public float MaxSpeedHorizontal = 5;
    private float CurrentSpeedHorizontal = 5;

    public bool X = true;//Si es true las funciones se hacen en funcion de la horizontal sino de la vertical
    public float PressTimeForMaxJump = 0.5f;
    public int _currentJumps;
    public int NumberOfJumps = 1;
    private float _lastVelocitY;
    private float _jumpStartTime;
    WallJump wallJump;
    public float tweakGravity = 1.2f;



    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _detector = GetComponent<CollisionDetection>();
        wallJump = GetComponent<WallJump>();
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
        if (X)
        {
            //CurrentSpeedHorizontal = Math.Min(MaxSpeedHorizontal, rigidbody.velocity.x);
            //rigidbody.velocity = new Vector2(CurrentSpeedHorizontal, rigidbody.velocity.y);
            if (ReachedPeak())
            {
                TweakGravity();
            }
        }
        
        
    }

   

    private bool ReachedPeak()//Cuando la velocidad del jugador es 0 en Y
    {
        bool reached = (_lastVelocitY > 0 && rigidbody.linearVelocity.y <= 0);//Se hace de esta manera por si en ningun frame esta en 0 y pase de 1 a -1 etc
        _lastVelocitY = rigidbody.linearVelocity.y;
        return reached;
    }

    private void TweakGravity()//Cuando llega al punto más alto aumentamos la gravedad para que baje más rapido
    {
        rigidbody.gravityScale *= tweakGravity;
        
    }

    void OnJumpStarted()
    {
        TryJump();
        _jumpStartTime = Time.time;
    }
    void OnJumpFinished()
    {
      //Si es un numero pequeño lo multipla x10 si es alto por 1
    float f = 1 / Mathf.Max( Mathf.Clamp01((Time.time - _jumpStartTime) /
            PressTimeForMaxJump),0.01f);
        
        TweakShortJumpGravity(f);
    }

    private void TweakShortJumpGravity(float f)//Salto más corto si no mantienes el boton
    {
        if (!wallJump.wallJumping)
        {
            rigidbody.gravityScale *= f;
        } 
    }

    private void TryJump()
    {
        if (wallJump.wallJumping)
        {
            transform.localScale= new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            WallJump();
        }
        else
        {
            if (_detector.IsGrounded || _currentJumps < NumberOfJumps)
            {

                Jump();
            }
        }
        
        
    }
    void Jump()
    {
        _currentJumps++;
        SetGravity();
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, getJumpSpeed());
    }
    void WallJump()
    {
        rigidbody.gravityScale = 1;
        _currentJumps = 0;
        SetGravity();
        //rigidbody.AddForce(new Vector2(wallJump.NextWall * wallJump.WallJumpSpeed, 2), ForceMode2D.Impulse);
        //rigidbody.velocity = new Vector2(rigidbody.velocity.x, getJumpSpeed());
        rigidbody.linearVelocity = new Vector2(wallJump.NextWall * wallJump.WallJumpSpeed, getJumpSpeed());
        
        
    }
    private void SetGravity()
    {
        if (X)
        {
            rigidbody.gravityScale *= -2 * JumpHeight*(CurrentSpeedHorizontal*CurrentSpeedHorizontal) 
                / (PeakDistance * PeakDistance)
                / Physics2D.gravity.y;
        }
        else
        {
            //La gravedad es -2 veces la altura dividido entre el tiempo
            //GravityScale multipla por la gravedad del juego en este caso -9,8 
            //por lo tanto tenenemos que dividir el resultado entre la gravedad total
            rigidbody.gravityScale *= -2 * JumpHeight / (TimeToPeak * TimeToPeak)
                / Physics2D.gravity.y;
        }
        
    }

    float getJumpSpeed()
    {
        if (X)//Tenemos en cuenta X e Y
        {
            //velocidad inical = 2 veces la altura multiplicado por la velocidad en x dividido entre la distancia en horizontal
            
            return 2 * JumpHeight * CurrentSpeedHorizontal / PeakDistance;
        }
        else//Tenemos en cuenta solo Y
        {
            //Velocidad = 2 veces la altura maxima divididio entre el tiempo
            return 2 * JumpHeight / TimeToPeak;
        }
        

    }


    void OnLanded()
    {
        _currentJumps = 0;
        rigidbody.gravityScale = 1;
    }
}

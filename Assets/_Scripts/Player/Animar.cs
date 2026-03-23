using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Animar : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rigidbody;
    Jumper jumper;
    CollisionDetection collisionDetection;
    bool wasGrounded = false;
    bool preJump = false;
    WallJump wall;

    private void OnEnable()
    {
        HealthSystem.OnPlayerRecieveDamage += Damage;
    }

    private void OnDisable()
    {
        HealthSystem.OnPlayerRecieveDamage -= Damage;

    }
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        jumper = GetComponent<Jumper>();
        collisionDetection = GetComponent<CollisionDetection>();
        wall = GetComponent<WallJump>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Run();       

        if (preJump)
        {           
            if (!collisionDetection.IsGrounded)
            {
                if (jumper._currentJumps <= 0)
                {
                    Jump();
                }
                else if (jumper._currentJumps >= 1)
                {
                    DoubleJump();
                }
                
            }            
            else if (collisionDetection.IsGrounded && !wall.wallJumping)
            {
                preJump = false;
                //animator.SetBool("DoubleJump", false);
                //PreJump();
                //wasGrounded = collisionDetection.IsGrounded;          
                //para poder subir a collab
            }         
           
        }
    }

    void Run()
    {
        animator.SetFloat("Speed", rigidbody.linearVelocity.magnitude);        
        animator.SetBool("TakeDamage", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        animator.SetBool("PreJump", false);
        animator.SetBool("Jump", false);
        animator.SetBool("DoubleJump", false);
        animator.SetBool("JumpDown", false);
        animator.SetBool("SwordAttack", false);
        animator.SetBool("Death", false);
    }

    void Attack()
    {
        animator.SetBool("Attack", true);
        animator.SetBool("TakeDamage", false);
        animator.SetBool("Idle", false);        
        animator.SetBool("PreJump", false);
        animator.SetBool("Jump", false);
        animator.SetBool("DoubleJump", false);
        animator.SetBool("JumpDown", false);
        animator.SetBool("SwordAttack", false);
        animator.SetBool("Death", false);
    }

    void Idle()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("PreJump", false);
        preJump = false;
    }

    void PreJump()
    {
        //hola
        animator.SetBool("PreJump", true);        
        animator.SetBool("TakeDamage", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);        
        animator.SetBool("Jump", false);
        animator.SetBool("DoubleJump", false);
        animator.SetBool("JumpDown", false);
        animator.SetBool("SwordAttack", false);
        animator.SetBool("Death", false);
    }

    void Jump()
    {
        if (wall.Wallsliding || wall.WallslidingInvers)
        {
            DoubleJump();
        } 
        else
        {

            animator.SetBool("Jump", true);
            animator.SetBool("PreJump", false);
        }
    }

    void DoubleJump()
    {
        animator.SetBool("DoubleJump", true);
        animator.SetBool("Jump", true);
    }

    void Damage(float damage)
    {
        animator.SetBool("TakeDamage", true);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        animator.SetBool("PreJump", false);
        animator.SetBool("Jump", false);
        animator.SetBool("DoubleJump", false);
        animator.SetBool("JumpDown", false);
        animator.SetBool("SwordAttack", false);
        animator.SetBool("Death", false);        
    }

    void Death()
    {
        animator.SetBool("Death", true);
    }

    void Fall() 
    {
        animator.SetBool("JumpDown", true);
        animator.SetBool("TakeDamage", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        animator.SetBool("PreJump", false);
        animator.SetBool("Jump", false);
        animator.SetBool("DoubleJump", false);       
        animator.SetBool("SwordAttack", false);
        animator.SetBool("Death", false);
    }
    
    void OnMove(InputValue input)
    {
        var inVal = input.Get<Vector2>();
        if(inVal.x > 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            if (inVal.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    void OnJumpStarted()
    {
        if (!wall.wallJumping) 
        {
            PreJump();

        }
        preJump = true;

        //wasGrounded = false;
    }     
    void OnAttack()
    {
        animator.SetTrigger("Sword");
    }
       
}

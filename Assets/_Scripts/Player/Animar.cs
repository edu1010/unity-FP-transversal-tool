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
    WallJump wall;
    [SerializeField]
    private float SpeedDeadzone = 0.1f;
    [SerializeField]
    private float JumpDownThreshold = 0.15f;
    [SerializeField]
    private float DoubleJumpHold = 0.2f;
    [SerializeField]
    private float AttackHold = 0.2f;
    [SerializeField]
    private float DamageHold = 0.2f;

    private float _attackTimer;
    private float _swordAttackTimer;
    private float _damageTimer;
    private float _doubleJumpTimer;
    private int _lastJumpCount;
    private bool _dead;
    private AnimState _state;

    private enum AnimState
    {
        Idle,
        Run,
        Jump,
        DoubleJump,
        JumpDown,
        Attack,
        SwordAttack,
        TakeDamage,
        Death
    }

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
        UpdateTimers();
        UpdateState();
        ApplyAnimator();
    }

    void UpdateState()
    {
        float speedX = Mathf.Abs(rigidbody.linearVelocity.x);
        float velY = rigidbody.linearVelocity.y;
        bool grounded = collisionDetection.IsGrounded;

        if (_dead)
        {
            _state = AnimState.Death;
            return;
        }
        if (_damageTimer > 0f)
        {
            _state = AnimState.TakeDamage;
            return;
        }
        if (_swordAttackTimer > 0f)
        {
            _state = AnimState.SwordAttack;
            return;
        }
        if (_attackTimer > 0f)
        {
            _state = AnimState.Attack;
            return;
        }

        if (!grounded)
        {
            if (_doubleJumpTimer > 0f)
            {
                _state = AnimState.DoubleJump;
            }
            else
            {
                if (velY < -JumpDownThreshold)
                {
                    _state = AnimState.JumpDown;
                }
                else
                {
                    _state = AnimState.Jump;
                }
            }
            return;
        }

        _state = speedX > SpeedDeadzone ? AnimState.Run : AnimState.Idle;
    }

    void ApplyAnimator()
    {
        float speedX = Mathf.Abs(rigidbody.linearVelocity.x);

        animator.SetFloat("Speed", speedX);
        animator.SetBool("Idle", _state == AnimState.Idle);
        animator.SetBool("Attack", _state == AnimState.Attack);
        animator.SetBool("SwordAttack", _state == AnimState.SwordAttack);
        animator.SetBool("Jump", _state == AnimState.Jump);
        animator.SetBool("DoubleJump", _state == AnimState.DoubleJump);
        animator.SetBool("JumpDown", _state == AnimState.JumpDown);
        animator.SetBool("TakeDamage", _state == AnimState.TakeDamage);
        animator.SetBool("Death", _state == AnimState.Death);
    }

    void Attack()
    {
        _attackTimer = AttackHold;
    }

    void Idle() { }

    void PreJump()
    {
    }

    void Jump() { }

    void DoubleJump() { }

    void Damage(float damage)
    {
        _damageTimer = DamageHold;
    }

    void Death()
    {
        _dead = true;
    }

    void Fall() { }
    
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
        if (!wall.wallJumping && collisionDetection.IsGrounded)
        {
            animator.SetTrigger("PreJump");
        }

        //wasGrounded = false;
    }     
    void OnAttack()
    {
        _swordAttackTimer = AttackHold;
        animator.SetTrigger("Sword");
    }

    private void UpdateTimers()
    {
        if (_attackTimer > 0f)
        {
            _attackTimer -= Time.fixedDeltaTime;
        }
        if (_swordAttackTimer > 0f)
        {
            _swordAttackTimer -= Time.fixedDeltaTime;
        }
        if (_damageTimer > 0f)
        {
            _damageTimer -= Time.fixedDeltaTime;
        }

        if (collisionDetection.IsGrounded)
        {
            _lastJumpCount = 0;
            _doubleJumpTimer = 0f;
        }

        if (jumper._currentJumps > _lastJumpCount)
        {
            if (jumper._currentJumps >= 2)
            {
                _doubleJumpTimer = DoubleJumpHold;
            }
            _lastJumpCount = jumper._currentJumps;
        }

        if (_doubleJumpTimer > 0f)
        {
            _doubleJumpTimer -= Time.fixedDeltaTime;
        }
    }
       
}

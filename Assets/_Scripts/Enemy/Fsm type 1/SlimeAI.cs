using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    enum EPatrolWithAngleVision
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    FSM<EPatrolWithAngleVision> brain;

    public Transform Player;

    // mushroom characteristics
    Rigidbody2D _rigidbody;
    float Speed = 3.0f;

    //Idle Variables
    float timeIdleState;

    //Patrol Variables
    Vector3 patrolDir = Vector3.right;
    Transform DetectionPoint;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsWall;
    public LayerMask WhatIsPlayer;

    //Chase Variables
    float ChaseDistance = 5.0f;

    //Attack Variables
    public float damage = 1.0f;
    float AttackDistance = 1.0f;

    //Animator controller
    Animator animator;

    //vision detector
    VisionDetector visionDetector;

    //detection edges and walls variables
    public float radiusDetectWalls = 0.45f;
    float distanceDetectEdges = 1.5f;

    void Start()
    {
        InitSFM();
        DetectionPoint = GetComponentInChildren<Transform>();
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();

        visionDetector = GetComponent<VisionDetector>();
    }

    void FixedUpdate()
    {
        brain.Update();
    }

    void InitSFM()
    {
        brain = new FSM<EPatrolWithAngleVision>(EPatrolWithAngleVision.Idle);

        //OnEnter
        brain.SetOnEnter(EPatrolWithAngleVision.Idle, () =>
        {
            timeIdleState = 0.0f;
            animator.SetBool("walking", false);
        });

        //OnStay
        brain.SetOnStay(EPatrolWithAngleVision.Idle, () => IdleUpdate());
        brain.SetOnStay(EPatrolWithAngleVision.Patrol, () => PatrolUpdate());
        brain.SetOnStay(EPatrolWithAngleVision.Chase, () => ChaseUpdate());
        brain.SetOnStay(EPatrolWithAngleVision.Attack, () => AttackUpdate());

        //OnExit
        brain.SetOnExit(EPatrolWithAngleVision.Idle, () =>
        {
            timeIdleState = 0.0f;
            animator.SetBool("walking", true);
        });
    }
    private void IdleUpdate()
    {
        timeIdleState += Time.deltaTime;

        if (timeIdleState >= Random.Range(1.0f, 2.5f))
        {
            brain.ChangeState(EPatrolWithAngleVision.Patrol);
        }
    }

    private void PatrolUpdate()
    {
        timeIdleState += Time.deltaTime;
        transform.position += patrolDir * Speed * Time.deltaTime;

        if (EdgeDetected() || WallDetected())
        {
            ChangeDirection();
        }

        if (IsPlayerCloseByAngleVision()) 
        {
            brain.ChangeState(EPatrolWithAngleVision.Chase);
        }

        if (timeIdleState >= Random.Range(8.0f, 15.0f))
        {
            brain.ChangeState(EPatrolWithAngleVision.Idle);
        }
    }

    private void ChaseUpdate()
    {
        Vector3 playerDir = Player.position - transform.position;
        playerDir = playerDir.normalized;

        transform.position += new Vector3(playerDir.x, 0, 0) * Speed * 2 * Time.deltaTime;

        if (!IsPlayerCloseByAngleVision() || EdgeDetected() || WallDetected()) 
        {
            brain.ChangeState(EPatrolWithAngleVision.Patrol);
        }

        if (IsPlayerCloseByAngleVision() && IsPlayerCloseByDistance(AttackDistance))
        {
            brain.ChangeState(EPatrolWithAngleVision.Attack);
        }
    }

    private void AttackUpdate()
    {
        Player.GetComponent<HealthSystem>().TakeDamage(damage);

        if (!IsPlayerCloseByDistance(AttackDistance) || !IsPlayerCloseByAngleVision() || EdgeDetected() || WallDetected())
        {
            brain.ChangeState(EPatrolWithAngleVision.Patrol);
        }
    }

    private bool IsPlayerCloseByDistance(float distance)
    {
        float d = Vector3.Distance(transform.position, Player.position);
        return d < distance;
    }

    private bool IsPlayerCloseByAngleVision()
    {
        return visionDetector.NumberOfPlayersDetected >= 1;
    }

    private bool EdgeDetected()
    {
        RaycastHit2D hitEndPlatform = Physics2D.Raycast(DetectionPoint.position, Vector2.down, distanceDetectEdges, WhatIsGround);

        if (hitEndPlatform.collider == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool WallDetected()
    {
        bool hitWall = Physics2D.OverlapCircle(DetectionPoint.position, radiusDetectWalls, WhatIsWall);

        return hitWall;
    }

    private void ChangeDirection()
    {
        transform.Rotate(0, 180, 0);

        if (patrolDir == Vector3.right)
        {
            patrolDir = Vector3.left;
        }
        else
        {
            patrolDir = Vector3.right;
        }

    }
}

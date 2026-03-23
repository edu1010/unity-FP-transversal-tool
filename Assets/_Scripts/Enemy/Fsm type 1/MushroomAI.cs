using UnityEngine;

public class MushroomAI : MonoBehaviour
{
    enum EPatrol
    {
        Idle, 
        Patrol,
        Chase, 
        Attack
    }

    FSM<EPatrol> brain;
    
    public Transform Player;

    // mushroom characteristics
    Rigidbody2D _rigidbody;
    float Speed = 3.0f;
    [SerializeField]

    //Idle Variables
    float timeIdleState;

    //Patrol Variables
    Vector3 patrolDir = Vector3.right;
    Transform DetectionPoint;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsWall;
    public LayerMask WhatIsPlayer;

    float lineVisionDistance = 10.0f;

    //Attack Variables
    float AttackDistance = 1.0f;
    public float damage = 1.0f;

    //Animator controller
    Animator animator;

    //detection edges and walls variables
    public float radiusDetectWalls = 0.25f;
    float distanceDetectEdges = 1.5f;

    void Start()
    {
        InitSFM();
        DetectionPoint = GetComponentInChildren<Transform>();
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        brain.Update();
    }

    void InitSFM()
    {
        brain = new FSM<EPatrol>(EPatrol.Idle);

        //OnEnter
        brain.SetOnEnter(EPatrol.Idle, () => 
        {
            timeIdleState = 0.0f;
            animator.SetBool("walking", false);
        });

        //OnStay
        brain.SetOnStay(EPatrol.Idle, () => IdleUpdate());
        brain.SetOnStay(EPatrol.Patrol, () => PatrolUpdate());
        brain.SetOnStay(EPatrol.Chase, () => ChaseUpdate());
        brain.SetOnStay(EPatrol.Attack, () => AttackUpdate());

        //OnExit
        brain.SetOnExit(EPatrol.Idle, () => 
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
            brain.ChangeState(EPatrol.Patrol);
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
        
        if (IsPlayerCloseByLineVision())
        {
            brain.ChangeState(EPatrol.Chase);
        }

        if (timeIdleState >= Random.Range(8.0f, 15.0f))
        {
            brain.ChangeState(EPatrol.Idle);
        }
    }

    private void ChaseUpdate()
    {
        Vector3 playerDir = Player.position - transform.position;
        playerDir = playerDir.normalized;

        transform.position += new Vector3(playerDir.x, 0, 0) * Speed*2 * Time.deltaTime;

        if (!IsPlayerCloseByLineVision() || EdgeDetected() || WallDetected())
        {
            brain.ChangeState(EPatrol.Patrol);
        }
        if (IsPlayerCloseByDistance(AttackDistance) && IsPlayerCloseByLineVision())
        {
            brain.ChangeState(EPatrol.Attack);
        }
    }

    private void AttackUpdate()
    {
        Player.GetComponent<HealthSystem>().TakeDamage(damage);

       if(!IsPlayerCloseByDistance(AttackDistance) || !IsPlayerCloseByLineVision() || EdgeDetected() || WallDetected())
       {
            brain.ChangeState(EPatrol.Patrol);
       }
    }

    private bool IsPlayerCloseByDistance(float distance)
    {
        float d = Vector3.Distance(transform.position, Player.position);
        return d < distance;
    }

    private bool IsPlayerCloseByLineVision()
    {
        RaycastHit2D lineVision;

        if (patrolDir == Vector3.right)
        {
            lineVision = Physics2D.Raycast(DetectionPoint.position, Vector2.right, lineVisionDistance, WhatIsPlayer);
        }
        else
        {
            lineVision = Physics2D.Raycast(DetectionPoint.position, Vector2.left, lineVisionDistance, WhatIsPlayer);
        }
        
        if (lineVision.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }

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

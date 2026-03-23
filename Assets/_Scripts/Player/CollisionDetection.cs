using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public bool IsGrounded => _isGrounded;
    private bool _isGrounded;
    private bool _wasGrounded;

    public bool IsTouchingRightWall => _isTouchingRight;
    private bool _isTouchingRight;
    public bool IsTouchingLeftWall => _isTouchingLeft;
    private bool _isTouchingLeft;
    

    private float checkRadius = 0.15f;
   
    [SerializeField]
    private LayerMask WhatIsGround;
    [SerializeField]
    private LayerMask WhatIsWall;
    
    [SerializeField]
    private Transform GroundCheckPoint;
    [SerializeField]
    private Transform RightCheckPoint;
    [SerializeField]
    private Transform LeftCheckPoint;


    public delegate void LandingDelegate();
    public LandingDelegate OnLanding;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGrounded();
        CheckRight();
        CheckLeft();
    }

    private void CheckGrounded()
    {
        var colliders = Physics2D.OverlapCircleAll(GroundCheckPoint.position, checkRadius, WhatIsGround);
        _isGrounded = colliders.Length > 0;
        if(_wasGrounded && _isGrounded)
        {
            OnLanding?.Invoke();
        }
        _wasGrounded = _isGrounded;
    }  
    private void CheckRight()
    {
        var colliders = Physics2D.OverlapCircleAll(RightCheckPoint.position,
          checkRadius, WhatIsWall);
        _isTouchingRight = colliders.Length > 0;
        
    } 
    private void CheckLeft()
    {
        var colliders = Physics2D.OverlapCircleAll(LeftCheckPoint.position,
          checkRadius, WhatIsWall);
        _isTouchingLeft = colliders.Length > 0;
        
    }

}

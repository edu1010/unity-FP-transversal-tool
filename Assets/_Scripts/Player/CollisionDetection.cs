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
        if(!_wasGrounded && _isGrounded)
        {
            OnLanding?.Invoke();
        }
        _wasGrounded = _isGrounded;
    }  
    private void CheckRight()
    {
        Vector3 rightPosition = GetCheckPosition(RightCheckPoint, LeftCheckPoint, false);
        var colliders = Physics2D.OverlapCircleAll(rightPosition,
          checkRadius, WhatIsWall);
        _isTouchingRight = colliders.Length > 0;
        
    } 
    private void CheckLeft()
    {
        Vector3 leftPosition = GetCheckPosition(LeftCheckPoint, RightCheckPoint, true);
        var colliders = Physics2D.OverlapCircleAll(leftPosition,
          checkRadius, WhatIsWall);
        _isTouchingLeft = colliders.Length > 0;
        
    }

    private Vector3 GetCheckPosition(Transform primaryPoint, Transform mirroredPoint, bool wantLeftSide)
    {
        Transform sourcePoint = primaryPoint != null ? primaryPoint : mirroredPoint;
        if (sourcePoint != null)
        {
            float horizontalOffset = Mathf.Abs(sourcePoint.localPosition.x);
            float verticalOffset = sourcePoint.localPosition.y;
            float side = wantLeftSide ? -1f : 1f;
            return transform.position + new Vector3(horizontalOffset * side, verticalOffset, 0f);
        }

        return transform.position;
    }

}

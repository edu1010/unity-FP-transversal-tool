using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosJump : MonoBehaviour
{
    Jumper jumper;
    [SerializeField]
    ContactFilter2D filter;
    float _JumpHeight = 3;
    private void Start()
    {
        _JumpHeight = jumper.JumpHeight;
    }
    private void OnEnable()
    {
        jumper = GetComponent<Jumper>();

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float h = _JumpHeight - GetDistanceToGround();
        Vector3 start = transform.position + new Vector3(-1, h, 0);
        Vector3 end = transform.position + new Vector3(1, h, 0);
        Gizmos.DrawLine(start, end);
        Gizmos.color = Color.white;
    }

    private float GetDistanceToGround()
    {
        RaycastHit2D[] hit = new RaycastHit2D[3];
         Physics2D.Raycast(transform.position, Vector2.down, filter,hit,10);
        return hit[0].distance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAIPatrol : MonoBehaviour
{
    public bool patrolling;
    private float walkSpeed = 200f;
    private float groundedRadius = .2f;

    [SerializeField] private Rigidbody2D monsterBody;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] Transform groundChecker;

    private void Start()
    {
        patrolling = true;
        monsterBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (patrolling)
        {
            Patrol();
        }
    }

    bool mustTurn;
    private void FixedUpdate()
    {
        if (patrolling)
        {
            mustTurn = !Physics2D.OverlapCircle(groundChecker.transform.position, groundedRadius, whatIsGround);
        }
    }

    private void Patrol()
    {
        //Debug.Log("Patrolling");
        monsterBody.velocity = new Vector2(walkSpeed * Time.deltaTime, monsterBody.velocity.y);
        if (mustTurn)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpeed *= -1;
    }
}

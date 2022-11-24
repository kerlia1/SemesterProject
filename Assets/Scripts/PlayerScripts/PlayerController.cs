using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded = false;
    private float groundedRadius = .2f;
    private float movementSmoothing = .1f;
    private float attackRadius = .3f;

    private PlayerState playerState;
    private Rigidbody2D playerBody;
    private Animator playerAnimator;

    [SerializeField] private LayerMask WhoIsEnemy;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] GameObject groundChecker;
    [SerializeField] GameObject attackPoint;

    Vector3 targetVelocity = Vector3.zero;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        playerBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    Collider2D[] ground;
    void FixedUpdate()
    {
        ground = Physics2D.OverlapCircleAll(groundChecker.transform.position, groundedRadius, whatIsGround);
        foreach (var item in ground)
        {
            if(item.gameObject != gameObject)
            {
                isGrounded = true;
            }
        }
    } 


    public void Movement(float move, bool jump)
    {
        targetVelocity = new Vector2(move * 10f, playerBody.velocity.y);
        playerBody.velocity = targetVelocity;

        if (isGrounded && jump)
        {
            //Debug.Log($"isGrounded: {isGrounded}, JumpForce: {playerState.PlayerJumpForce}.");
            playerBody.AddForce(new Vector2(playerBody.velocity.x, playerState.PlayerJumpForce));
            isGrounded = false;
        }
    }

    Collider2D[] enemies;
    public void MeleeCombatAttack()
    {

        enemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, WhoIsEnemy);
        // play attack animation
        playerAnimator.SetTrigger("playerAttack");

        // Get all enemies in range


        // Damage enemies;
    }

}

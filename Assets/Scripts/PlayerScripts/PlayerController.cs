using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded = false;
    private float groundedRadius = .2f;
    private float movementSmoothing = .1f;
    [SerializeField] private float attackRange = 0.69f;

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
            if (item.gameObject != gameObject)
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
        // Get Enemies in AttackRange
        enemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, WhoIsEnemy);

        foreach (var enemy in enemies)
        {
            Debug.Log(enemy.tag);
            enemy.GetComponent<MonsterController>().GetDamage(playerState.PlayerDamage);
        }

        // play attack animation
        playerAnimator.SetTrigger("playerAttack");

        // Damage enemies
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }

}

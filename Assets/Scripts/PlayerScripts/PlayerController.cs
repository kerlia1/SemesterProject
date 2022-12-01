using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded = false;
    private float groundedRadius = .2f;
    [SerializeField] private float attackRange = 0.69f;

    private PlayerState playerState;
    private Rigidbody2D playerBody;
    private Animator playerAnimator;

    [SerializeField] private LayerMask WhoIsEnemy;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] GameObject groundChecker;
    [SerializeField] GameObject attackPoint;
    [SerializeField] private MonsterController monsterController;

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
                playerAnimator.SetBool("Jump", false);
                isGrounded = true;
            }
        }
    }

    bool isFacingRight = true;
    public void Movement(float move, bool jump)
    {
        if (move > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }

        targetVelocity = new Vector2(move * 10f, playerBody.velocity.y);
        playerBody.velocity = targetVelocity;
        playerAnimator.SetFloat("Speed", Mathf.Abs(move));


        if (isGrounded && jump)
        {
            playerAnimator.SetBool("Jump", true);
            isGrounded = false;
            Debug.Log(isGrounded);
            playerBody.AddForce(new Vector2(playerBody.velocity.x, playerState.PlayerJumpForce));
            
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    Collider2D[] enemies;
    public void MeleeCombatAttack()
    {
        // Get Enemies in AttackRange
        enemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, WhoIsEnemy);

        foreach (var enemy in enemies)
        {
            Debug.Log(enemy.tag);
            if (enemy.tag == "PhysicEnemy")
            {
                enemy.GetComponent<MonsterController>().GetDamage(playerState.PlayerDamage);
            }
            else
            {
                continue;
            }
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
        Gizmos.DrawWireSphere(groundChecker.transform.position, groundedRadius);
    }

}

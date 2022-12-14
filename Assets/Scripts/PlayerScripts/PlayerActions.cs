using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerActions : MonoBehaviour
{
    private float horizontalMove = 0;
    private float groundedRadius = .025f;

    private bool isGrounded = false;
    private bool jump = false;

    private float attackRate = 2.5f;
    private float nextAttackTime = 0f;
    [SerializeField] private float attackRange = 0.69f;

    Vector2 targetVelocity = Vector2.zero;

    [SerializeField] private LayerMask WhoIsEnemy;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private GameObject groundChecker;
    [SerializeField] private GameObject attackPoint;

    private Rigidbody2D playerBody;
    private PlayerState playerState;
    private Animator playerAnimator;


    private void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerState = GetComponent<PlayerState>();
        playerAnimator = GetComponent<Animator>();

    }

    private void Update()
    {
        //horizontalMove = Input.GetAxisRaw("Horizontal") * playerState.PlayerSpeed;
        HorizontalMovement(horizontalMove);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
                jump = true;
        }


        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Attacks");
                MeleeCombatAttack();
                playerAnimator.SetTrigger("playerAttack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

    }

    Collider2D[] ground;
    private void FixedUpdate()
    {
        // Check when grounded
        ground = Physics2D.OverlapCircleAll(groundChecker.transform.position, groundedRadius, whatIsGround);
        foreach (Collider2D item in ground)
        {
            isGrounded = false;
            if (item.gameObject != gameObject)
            {
                isGrounded = true;
                OnLanding();
            }
        }

        if (jump && isGrounded)
        {
            Jump();
            jump = false;
            isGrounded = false;
        }
    }

    private void Jump()
    {
        Debug.Log($"JumpState: {jump} \n GroundState: {isGrounded}");

        // Add force to rigidbody of the player.
        //playerBody.AddForce(new Vector2(0, playerState.PlayerJumpForce), ForceMode2D.Impulse);

        // Start the animation.
        playerAnimator.SetBool("Jump", true);

    }


    Collider2D[] enemies;
    public void MeleeCombatAttack()
    {
        enemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, WhoIsEnemy);

        foreach (var enemy in enemies)
        {
            Debug.Log(enemy.tag);

            if (enemy.tag != "PhysicEnemy")
                continue;

            //enemy.GetComponent<MonsterController>().GetDamage(playerState.PlayerDamage);

        }

    }

    bool isFacingRight = true;

    public void HorizontalMovement(float move)
    {
        // Change direction of the Player
        if (move > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }

        // Start animation of the player:
        playerAnimator.SetFloat("Speed", Mathf.Abs(move));

        // Movement [<-] [->]
        targetVelocity = new Vector2(move, playerBody.velocity.y);
        playerBody.velocity = targetVelocity;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnLanding()
    {
        playerAnimator.SetBool("Jump", false);
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
        Gizmos.DrawWireSphere(groundChecker.transform.position, groundedRadius);
    }
}

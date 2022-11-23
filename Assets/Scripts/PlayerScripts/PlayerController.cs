using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded = false;
    private float groundedRadius = .2f;
    private float movementSmoothing = .1f;

    private PlayerState playerState;
    private Rigidbody2D playerBody;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] GameObject groundChecker;

    Vector3 targetVelocity = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        playerBody = GetComponent<Rigidbody2D>();

    }

    Collider2D[] colls;
    void FixedUpdate()
    {
        colls = Physics2D.OverlapCircleAll(groundChecker.transform.position, groundedRadius, whatIsGround);
        foreach (var item in colls)
        {
            if(item.gameObject != gameObject)
            {
                //Debug.Log(isGrounded);
                isGrounded = true;
            }
        }
    } 


    public void Movement(float move, bool jump)
    {
        targetVelocity = new Vector2(move * 10f, playerBody.velocity.y);
        playerBody.velocity = Vector3.SmoothDamp(playerBody.velocity, targetVelocity, ref velocity, movementSmoothing);


        if (isGrounded && jump)
        {
            Debug.Log($"isGrounded: {isGrounded}, JumpForce: {playerState.JumpForce}.");
            playerBody.AddForce(new Vector2(playerBody.velocity.x, playerState.JumpForce));
            isGrounded = false;
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    private PlayerMovement mov;
    private Animator anim;
    private SpriteRenderer spriteRend;

    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }

    public float currentVelY;

    public float currentVelX;

    void Start()
    {
        mov = GetComponent<PlayerMovement>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        anim = spriteRend.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        CheckAnimationState();
    }

    private void FixedUpdate()
    {
        anim.SetFloat("velY", mov.playerBody.velocity.y);
        anim.SetFloat("velX", Mathf.Abs(mov.playerBody.velocity.x));
    }

    private void CheckAnimationState()
    {
        if (startedJumping)
        {
            anim.SetTrigger("Jump");
            startedJumping = false;
            return;
        }

        if (justLanded)
        {
            anim.SetTrigger("Land");
            justLanded = false;
            return;
        }

        
    }
}

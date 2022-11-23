using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private float horizontalMove = 0;
    private bool jump = false;
    private PlayerState playerState;

    [SerializeField] private PlayerController controller;


    void Start()
    {
        playerState = GetComponent<PlayerState>();
        controller = GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * playerState.PlayerSpeed;

        if (Input.GetButtonDown("Jump"))
            jump = true;


        controller.Movement(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}

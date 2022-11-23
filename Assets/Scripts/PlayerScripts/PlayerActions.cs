using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private float horizontalMove = 0;
    private bool jump = false;
    private PlayerState playerState;

    [SerializeField] private PlayerController controller;


    private void Start()
    {
        playerState = GetComponent<PlayerState>();
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jump = true;

    }

    private void FixedUpdate()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * playerState.PlayerSpeed;

        controller.Movement(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}

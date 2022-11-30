using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerActions : MonoBehaviour
{
    private float horizontalMove = 0;
    private bool jump = false;
    private bool attack = false;
    private PlayerState playerState;

    [SerializeField] private PlayerController playerController;

    [Header("Events")]
    [Space]

    public UnityEvent OnAttackEvent;


    private void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jump = true;

        if (Input.GetKeyDown(KeyCode.Z))
            attack = true;
    }

    private void FixedUpdate()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * playerState.PlayerSpeed;

        playerController.Movement(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;

        if (attack)
        {
            playerController.MeleeCombatAttack();
            attack = false;
        }
    }
}

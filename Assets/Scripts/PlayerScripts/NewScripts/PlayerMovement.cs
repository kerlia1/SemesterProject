using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerState playerState;

    #region Components
    public Rigidbody2D playerBody { get; private set; }
    #endregion

    #region State Parameters
    // Parameters to control player actions
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsSliding { get; private set; }

    // Timers
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnRightTime { get; private set; }
    public float LastOnLeftTime { get; private set; }

    // Jump
    private bool isJumpCut;
    private bool isJumpFalling;

    // Wall Jump
    private float wallJumpStartTime;
    private int lastWallJumpDir;

    // Dash
    private int dashesLeft;
    private bool dasheRefilling;
    private Vector2 lastDashDir;
    private bool isDashAttacking;

    #endregion

    #region Input parameters
    private Vector2 moveInput;

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion

    #region Check parameters
    // Setings for Inspector
    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(.5f, .03f);
    [Space(5)]
    // Walls
    [SerializeField] private Transform frontWallCheckPoint;
    [SerializeField] private Transform backWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(.35f, 1f);

    #endregion

    #region Layers and Tags
    [Header("Layers")]
    [SerializeField] private LayerMask whatIsGround;
    #endregion

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetGravityScale(playerState.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        // Timers
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnRightTime -= Time.deltaTime;
        LastOnLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;


        // Input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");


    }

    private void FixedUpdate()
    {
        if(!IsDashing)
        {
            if (IsWallJumping)
                Run(playerState.wallJumpRunLerp);
            else
                Run(1);
        }
    }

    #region General Methods
    private void SetGravityScale(float scale)
    {
        playerBody.gravityScale = scale;
    }

    #endregion


    private void Run(float lerpAmount)
    {
        float targetSpeed = moveInput.x * playerState.runMaxSpeed;

        targetSpeed = Mathf.Lerp(playerBody.velocity.x, targetSpeed, lerpAmount);

        // Acceleration.
        float accelRate;
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerState.runAccelAmount : playerState.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerState.runAccelAmount * playerState.accelInAir : playerState.runDeccelAmount * playerState.deccelInAir;

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(playerBody.velocity.y) < playerState.jumpTimeThershold)
  {
            accelRate *= playerState.jumpHangAccelMult;
            targetSpeed *= playerState.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        
        if (playerState.doConserveMomentum && Mathf.Abs(playerBody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(playerBody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion
        // Difference between current and desired velocity
        float speedDif = targetSpeed - playerBody.velocity.x;
        float movement = speedDif * accelRate;

        playerBody.AddForce(movement * Vector2.right, ForceMode2D.Force);



    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }


    // Dash Methods
    private IEnumerator StartDash(Vector2 dir)
    {
        yield return null;
    }
    
    private IEnumerator RefilDash()
    {
        dasheRefilling = true;
        yield return new WaitForSeconds(playerState.dashRefillTime);
        dasheRefilling = false;
        dashesLeft = Mathf.Min(playerState.dashAmount, dashesLeft + 1);
    }

    // Checkers
    public void CheckFaceDirection(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool canJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool canWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 &&
            (!IsWallJumping || (LastOnRightTime > 0 && lastWallJumpDir == 1) || (LastOnLeftTime > 0 && lastWallJumpDir == -1));
    }

    private bool canJumpCut()
    {
        return IsJumping && playerBody.velocity.y > 0;
    }

    private bool canWallJumpCut()
    {
        return IsWallJumping && playerBody.velocity.y > 0;
    }

    private bool canDash()
    {
        if (!IsDashing && dashesLeft < playerState.dashAmount && LastOnGroundTime > 0 && !dasheRefilling)
            StartCoroutine(nameof(RefilDash), 1);
        
        return dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;

        return false;
    }

    #region Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(frontWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(backWallCheckPoint.position, wallCheckSize);
    }
    #endregion

}

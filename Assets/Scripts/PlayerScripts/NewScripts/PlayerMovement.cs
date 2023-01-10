using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerState playerState;

    #region Components
    public Rigidbody2D playerBody { get; private set; }

    public PlayerAnimator animHandler { get; private set; }
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

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(.5f, .03f);
    [Space(5)]

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
        animHandler = GetComponent<PlayerAnimator>();
    }

    private void Start()
    {
        SetGravityScale(playerState.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        // Таймеры
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnRightTime -= Time.deltaTime;
        LastOnLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;

        // Ввод
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.x != 0)
            CheckFaceDirection(moveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            OnDashInput();
        }


        if (!IsDashing && !IsJumping)
        {
            // Проверка приземления
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, whatIsGround) && !IsJumping)
            {
                if (LastOnGroundTime < -0.2f)
                {
                    animHandler.justLanded = true;
                    animHandler.wallSliding = false;
                }

                LastOnGroundTime = playerState.coyoteTime;
            }

            // Проверка стены справа
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, whatIsGround) && IsFacingRight)
                    || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, whatIsGround) && !IsFacingRight)) && !IsWallJumping)
                LastOnRightTime = playerState.coyoteTime;
           
            // Проверка стены слева
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, whatIsGround) && !IsFacingRight)
                || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, whatIsGround) && IsFacingRight)) && !IsWallJumping)
                LastOnLeftTime = playerState.coyoteTime;


            LastOnWallTime = Mathf.Max(LastOnLeftTime, LastOnRightTime);
        }

        // Чекеры прыжков
        if (IsJumping && playerBody.velocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping)
                isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - wallJumpStartTime > playerState.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            isJumpCut = false;

            if (!IsJumping)
                isJumpFalling = false;
        }

        if (!IsDashing)
        {
            // Прыжок
            if (canJump() && LastPressedJumpTime > 0)
            {
                IsJumping = true;
                IsWallJumping = false;
                isJumpCut = false;
                isJumpFalling = false;
                IsSliding = false;
                Jump();

                animHandler.startedJumping = true;
            }

            // Прыжок от стены
            else if (canWallJump() && LastPressedJumpTime > 0)
            {
                IsWallJumping = true;
                IsJumping = false;
                isJumpCut = false;
                isJumpFalling = false;

                wallJumpStartTime = Time.time;
                lastWallJumpDir = (LastOnRightTime > 0) ? -1 : 1;

                WallJump(lastWallJumpDir);
            }
        }

        // Чекеры для дэша
        if (canDash() && LastPressedDashTime > 0)
        {

            Sleep(playerState.dashSleepTime);

            if (moveInput != Vector2.zero)
                lastDashDir = moveInput;
            else
                lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



            IsDashing = true;
            IsJumping = false;
            IsWallJumping = false;
            isJumpCut = false;

            StartCoroutine(nameof(StartDash), lastDashDir);
        }

        // Слайдинг по стене
        // Не настроены анимации
        if (CanSlide() && ((LastOnLeftTime > 0 && moveInput.x < 0) || (LastOnRightTime > 0 && moveInput.x > 0)))
        {
            IsSliding = true;
            //animHandler.wallSliding = IsSliding;
        }
        else
        {
            IsSliding = false;
            //animHandler.wallSliding = IsSliding;
        }

        // Гравитация
        if (!isDashAttacking)
        {

            if (IsSliding)
            {
                SetGravityScale(0);
            }
            else if (playerBody.velocity.y < 0 && moveInput.y < 0)
            {

                SetGravityScale(playerState.gravityScale * playerState.fastFallGravityMult);

                playerBody.velocity = new Vector2(playerBody.velocity.x, Mathf.Max(playerBody.velocity.y, -playerState.maxFastFallSpeed));
            }
            else if (isJumpCut)
            {

                SetGravityScale(playerState.gravityScale * playerState.jumpCutGravityMult);
                playerBody.velocity = new Vector2(playerBody.velocity.x, Mathf.Max(playerBody.velocity.y, -playerState.maxFallSpeed));
            }
            else if ((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(playerBody.velocity.y) < playerState.jumpTimeThershold)
            {
                SetGravityScale(playerState.gravityScale * playerState.jumpHangGravityMult);
            }
            else if (playerBody.velocity.y < 0)
            {

                SetGravityScale(playerState.gravityScale * playerState.fallGravityMult);

                playerBody.velocity = new Vector2(playerBody.velocity.x, Mathf.Max(playerBody.velocity.y, -playerState.maxFallSpeed));
            }
            else
            {

                SetGravityScale(playerState.gravityScale);
            }
        }
        else
        {

            SetGravityScale(0);
        }
    }

    private void FixedUpdate()
    {
        if (!IsDashing)
        {
            if (IsWallJumping)
                Run(playerState.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (isDashAttacking)
        {
            Run(playerState.dashEndRunLerp);
        }

        if (IsSliding)
            Slide();
    }

    public void OnJumpInput()
    {
        LastPressedJumpTime = playerState.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (canJumpCut() || canWallJumpCut())
            isJumpCut = true;
    }

    public void OnDashInput()
    {
        LastPressedDashTime = playerState.dashInputBufferTime;
    }



    #region General Methods
    private void SetGravityScale(float scale)
    {
        playerBody.gravityScale = scale;
    }
    private void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    #endregion

    #region Jump Methods
    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        float force = playerState.jumpForce;
        if (playerBody.velocity.y < 0)
            force -= playerBody.velocity.y;

        playerBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnRightTime = 0;
        LastOnLeftTime = 0;

        Vector2 force = new Vector2(playerState.wallJumpForce.x, playerState.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(playerBody.velocity.x) != Mathf.Sign(force.x))
            force.x -= playerBody.velocity.x;

        if (playerBody.velocity.y < 0)
            force.y -= playerBody.velocity.y;

        playerBody.AddForce(force, ForceMode2D.Impulse);

    }

    #endregion

    #region Dash Methods
    private IEnumerator StartDash(Vector2 dir)
    {
        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        dashesLeft--;
        isDashAttacking = true;

        SetGravityScale(0);

        while (Time.time - startTime <= playerState.dashAttackTime)
        {
            playerBody.velocity = dir.normalized * playerState.dashSpeed;
            yield return null;
        }

        startTime = Time.time;

        isDashAttacking = false;
        SetGravityScale(playerState.gravityScale);
        playerBody.velocity = playerState.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= playerState.dashEndTime)
        {
            yield return null;
        }

        // Рывок закончен
        IsDashing = false;
    }

    private IEnumerator RefillDash(int amount)
    {
        dasheRefilling = true;
        yield return new WaitForSeconds(playerState.dashRefillTime);
        dasheRefilling = false;
        dashesLeft = Mathf.Min(playerState.dashAmount, dashesLeft + 1);
    }

    #endregion


    private void Run(float lerpAmount)
    {
        float targetSpeed = moveInput.x * playerState.runMaxSpeed;

        targetSpeed = Mathf.Lerp(playerBody.velocity.x, targetSpeed, lerpAmount);

        // Скорость
        float accelRate;
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerState.runAccelAmount : playerState.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerState.runAccelAmount * playerState.accelInAir : playerState.runDeccelAmount * playerState.deccelInAir;


        if ((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(playerBody.velocity.y) < playerState.jumpTimeThershold)
        {
            accelRate *= playerState.jumpHangAccelMult;
            targetSpeed *= playerState.jumpHangMaxSpeedMult;
        }

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


    // Если мы прыгаем на стену, то разворачиваемся и начинаем соскальзывать
    private void Slide()
    {
        // Разворачиваемся, когда запускается slide
        Turn();

        // Разница в скорости
        float speedDif = playerState.slideSpeed - playerBody.velocity.y;

        // Текущая скорость передвижения
        float movement = speedDif * playerState.slideAccel;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        
        
        
        playerBody.AddForce(movement * Vector2.up);
    }

    #region Check Methods
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
            StartCoroutine(nameof(RefillDash), 1);

        return dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;

        return false;
    }
    #endregion


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

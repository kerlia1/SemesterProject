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

    #endregion

    #region Input parameters
    private Vector2 moveInput;

    public float LastPressedJumpTime { get; private set; }
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
    }

    private void SetGravityScale(float scale)
    {
        playerBody.gravityScale = scale;
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

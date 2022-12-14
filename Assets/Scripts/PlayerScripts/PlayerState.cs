using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState")] // Create a new PlayerState
public class PlayerState : ScriptableObject
{
    #region Gravity
    // Gravity params
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public float gravityScale;

    [Space(5)]
    public float fallGravityMult;
    public float maxFallSpeed;

    [Space(5)]
    public float fastFallGravityMult;

    public float maxFastFallSpeed;
    #endregion
    [Space(15)]

    #region Run
    [Header("Run")]
    public float runMaxSpeed;
    public float runAcceleration;
    [HideInInspector] public float runAccelAmount;
    public float runDecceleration;
    [HideInInspector] public float runDeccelAmount;
    [Space(5)]
    [Range(0f, 1f)] public float accelInAir;
    [Range(0f, 1f)] public float deccelInAir;
    [Space(5)]
    public bool doConserveMomentum = true;
    #endregion
    [Space(15)]

    #region Jump
    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    [HideInInspector] public float jumpForce;

    [Header("Both Jumps")]
    public float jumpCutGravityMult; // Multiplier to increase gravity if the player releases jump button when he still jumping;
    [Range(0f, 1f)] public float jumpHangGravityMult; // Reduces the gravity when player close to apex of jump height;
    public float jumpTimeThershold; // Скорости при которых игрок будет использовать дополнительно зависание в прыжке (скорость игрока ближе всего к вершине прыжка)
    [Space(5)]
    public float jumpHangAccelMult;
    public float jumpHangMaxSpeedMult;
    #endregion
    [Space(15)]

    #region Wall Jump
    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    [Space(5)]
    [Range(0, 1f)] public float wallJumpRunLerp;
    [Range(0, 1f)] public float wallJumpTime;
    public bool doTurnOnWallJump;
    #endregion
    [Space(15)]

    #region Slide
    [Header("Slide")]
    public float slideSpeed;
    public float slideAccel;
    #endregion
    [Space(15)]

    #region Dash
    [Header("Dash")]
    public int dashAmount;
    public float dashSpeed;
    public float dashSleepTime;
    [Space(5)]

    public float dashAttackTime;
    [Space(5)]

    public float dashEndTime;
    public Vector2 dashEndSpeed;
    [Range(0f, 1f)] public float dashEndRunLerp;
    [Space(5)]

    public float dashRefillTime;
    [Space(5)]

    [Range(.01f, .5f)] public float dashInputBufferTime;
    #endregion

    // Called when Inspector updates
    private void OnValidate()
    {
        // Gravity = 2 * jumpHeight / timeToJumpApex^2
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        // GravityScale in curr project
        gravityScale = gravityScale / Physics2D.gravity.y;

        // Amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        // 50 - const for cur project
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        // InitialJumpVelocity = gravity * timeToJumpApex
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        // Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
    }
}

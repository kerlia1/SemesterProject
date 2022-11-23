using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState: MonoBehaviour
{

    private float playerSpeed = 30f;
    public float PlayerSpeed { get => playerSpeed; }

    private float jumpForce = 600f;
    public float JumpForce { get => jumpForce; }

    private int healthPoints;

    public int HealthPoints { get; set; }

    private float manaPoints;

    public float ManaPoints { get; set; }

}

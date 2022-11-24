using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{

    private float playerSpeed = 30f;
    public float PlayerSpeed { get => playerSpeed; }

    private float playerJumpForce = 600f;
    public float PlayerJumpForce { get => playerJumpForce; }

    private float playerDamage = 5f;
    public float PlayerDamage { get => playerDamage; }

    private int playerHealthPoints;

    public int PlayerHealthPoints { get; set; }

    private float playerManaPoints;

    public float PlayerManaPoints { get; set; }

}

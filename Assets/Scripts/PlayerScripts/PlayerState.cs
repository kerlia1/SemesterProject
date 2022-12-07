using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{

    private float playerSpeed = 30f;
    public float PlayerSpeed { get => playerSpeed; }

    private float playerJumpForce = 25f;
    public float PlayerJumpForce { get => playerJumpForce; }

    private int playerDamage = 5;
    public int PlayerDamage { get => playerDamage; }

    private int playerHealthPoints;
    public int PlayerHealthPoints { get; set; }

    private float playerManaPoints;

    public float PlayerManaPoints { get; set; }

}

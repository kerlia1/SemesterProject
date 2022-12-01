using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterState : MonoBehaviour
{
    private Animator monsterAnimator;

    private int healthPoints = 20;
    public int HealthPoints { get => healthPoints; set { healthPoints = value; } }

    private float walkSpeed = 200f;
    public float WalkSpeed { get => walkSpeed; }

    private bool alive;
    public bool Alive { get => alive; set { alive = value; } }

    
    










    private void Start()
    {
        alive = true;
        monsterAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Вызывает тригер в аниматоре согласно переданному параметру.
    /// </summary>
    /// <param name="trigger"></param>
    public void ChangeAnimTrigger(string trigger)
    {
        monsterAnimator.SetTrigger(trigger);
    }

    /// <summary>
    /// Смерть монстра.
    /// </summary>
    public void Die()
    {
        // Die animation
        ChangeAnimTrigger("Die");

        // Disable enemy (delete from scene)
        GetComponentInChildren<Collider2D>().enabled = false;
        Alive = false;
        GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
}

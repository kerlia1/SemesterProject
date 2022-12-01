using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private MonsterState monsterState;
    private Animator monsterAnimator;
    private MonsterAIPatrol patrol;

    // Start is called before the first frame update
    void Start()
    {
        monsterState = GetComponent<MonsterState>();
        monsterAnimator = GetComponent<Animator>();
        patrol = GetComponent<MonsterAIPatrol>();
    }


    public void GetDamage(int damage)
    {
        if (monsterState.HealthPoints > 0)
        {
            monsterAnimator.SetTrigger("Hurt");
            monsterState.HealthPoints -= damage;
        }
        else
        {
            Die();
        }
    }


    public void Die()
    {
        Debug.Log("Enemy Died");

        // Die animation
        monsterAnimator.SetTrigger("Die");

        // Disable enemy (delete from scene)
        GetComponent<Collider2D>().enabled = false;
        patrol.patrolling = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
}

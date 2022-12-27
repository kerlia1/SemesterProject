using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    private float attackRate = 2.5f;
    private float nextAttackTime = 0f;
    [SerializeField] private float attackRange = 0.69f;

    [SerializeField] private LayerMask WhoIsEnemy;
    [SerializeField] private GameObject attackPoint;

    private PlayerState playerState;
    public Animator swordAnimator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Attacks");
                MeleeCombatAttack();
                swordAnimator.SetTrigger("Attack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }


    Collider2D[] enemies;
    public void MeleeCombatAttack()
    {
        enemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, WhoIsEnemy);

        foreach (var enemy in enemies)
        {
            Debug.Log(enemy.tag);

            if (enemy.tag != "PhysicEnemy")
                continue;

            //enemy.GetComponent<MonsterController>().GetDamage(playerState.PlayerDamage);

        }

    }


    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }

}

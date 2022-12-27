using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    
    [SerializeField] private MonsterState monsterState;
    [SerializeField] private Rigidbody2D monsterBody;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundChecker;

    private float groundedRadius = 0.45f;
    [SerializeField] private float localSpeed;

    private void Awake()
    {
        monsterState = GetComponent<MonsterState>();
        monsterBody = GetComponentInChildren<Rigidbody2D>();
        localSpeed = monsterState.WalkSpeed;
    }

    // TODO:
    // Перенести в Машину состояний
    // Получение урона
    public void GetDamage(int damage)
    {
        if (monsterState.HealthPoints > 0)
        {
            monsterState.ChangeAnimTrigger("Hurt");
            monsterState.HealthPoints -= damage;
        }
        else
        {
            monsterState.Die();
        }
    }

    // Считывание состояния монстра сейчас.
    private void FixedUpdate()
    {
        NeedTurn();
        Patrol();
    }


    public bool mustTurn = false;
    Collider2D ground;
    private bool NeedTurn()
    {
        if (monsterState.Alive)
        {
            Debug.Log($"Must turn: {mustTurn}, Cur dir right: {!Physics2D.OverlapCircle(groundChecker.transform.position, groundedRadius, whatIsGround)}");

            mustTurn = !Physics2D.OverlapCircle(groundChecker.transform.position, groundedRadius, whatIsGround);
        }
        return mustTurn;
    }

    private void Patrol()
    {
        if (monsterState.Alive)
            monsterBody.velocity = new Vector2(localSpeed * Time.deltaTime, monsterBody.velocity.y);

        if (mustTurn)
            Flip();


    }

    // Разворот монстра на 180 градусов, изменение скорости
    private void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        localSpeed *= -1;
    }


    // Используется для настройки радиуса проверки нахождения на земле.
    private void OnDrawGizmosSelected()
    {
        if (groundChecker == null)
            return;

        Gizmos.DrawWireSphere(groundChecker.transform.position, groundedRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] Transform test;

    private void FixedUpdate()
    {
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(test.transform.position, 0.6f);
    }
}

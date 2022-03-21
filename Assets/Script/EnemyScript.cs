using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public Transform playerTrans;
    public float attackRange = 3;
    public int attackDamage = 1;
    public float attackInterval = 2f;

    public Image hitColor;

    NavMeshAgent thisAgent;
    Rigidbody thisRB;

    bool canAttack = true;


    private void Awake()
    {
        thisAgent = gameObject.GetComponent<NavMeshAgent>();
        thisRB = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerTrans.position) < 10 && Vector3.Distance(transform.position, playerTrans.position) > 1.25f)
        {
            thisAgent.SetDestination(playerTrans.position);
        }
        else
        {
            thisAgent.SetDestination(transform.position);
        }

        if(Vector3.Distance(playerTrans.position, transform.position) < attackRange)
        {
            if (canAttack)
            {
                canAttack = false;
                playerTrans.gameObject.GetComponent<Health>().health -= attackDamage;
                StartCoroutine(attackPlayer());
            }
        }
    }

    IEnumerator attackPlayer()
    {
        yield return new WaitForSeconds(attackInterval);
        canAttack = true;
    }
}

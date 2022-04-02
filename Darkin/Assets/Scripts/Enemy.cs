using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float viewRadius;
    [Range(0, 360)]
    [SerializeField] public float viewAngle;
    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask obstacleMask;
    [SerializeField] NavMeshAgent navMashAgent;
    [SerializeField] GameObject bloodVfx;



    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;
    bool isAlive;
    Animator animator;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        Transform target = GetTarget();
        if (target != null)
        {
            distanceToTarget = Vector3.Distance(target.position, transform.position);
            if (isProvoked)
            {
                EngageTarget(target);
            }
            else if (distanceToTarget < viewRadius + 1)
            {
                isProvoked = true;
            }
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    private void EngageTarget(Transform target)
    {
        if (distanceToTarget > navMashAgent.stoppingDistance)
        {
            navMashAgent.speed = 8;
            ChaseTarget(target);
            //animator.SetBool("isClose", false);
        }
        else if (distanceToTarget <= navMashAgent.stoppingDistance)
        {
            navMashAgent.speed = 0;
            //animator.SetBool("isClose", true);
        }
    }
    private void ChaseTarget(Transform target)
    {
        navMashAgent.SetDestination(target.position);
    }
    private Transform GetTarget()
    {
        if (isAlive)
        {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 disToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, disToTarget) < viewAngle / 2)
                {
                    float distToTarget = Vector3.Distance(transform.position, disToTarget);
                    if (!Physics.Raycast(transform.position, disToTarget, distToTarget, obstacleMask))
                    {
                        viewAngle = 360;
                        viewRadius = 20;
                        return target.transform;
                    }
                }
            }           
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Knife"))
        {
            var collisionPoint = other.ClosestPoint(transform.position);
            isAlive = false;
            gameObject.tag = "DeadEnemy";
            animator.SetBool("isDead", true);
            navMashAgent.enabled = false;
            GameObject bloodParticle = Instantiate(bloodVfx, collisionPoint, Quaternion.identity);
            bloodParticle.transform.parent = this.gameObject.transform;
        }
    }
}

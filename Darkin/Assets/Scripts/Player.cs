using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject attackButton;
    [SerializeField] BoxCollider daggerCol;
    [SerializeField] Light spotLight;

    GameObject[] allEnemies;
    Transform closestEnemy;

    public FixedJoystick fixedJoystick;
    public Rigidbody rb;

    Vector3 rotation;
    Animator animator;
    float speed = 300f;

    void Start()
    {
        closestEnemy = null;
        daggerCol.enabled = false;
        attackButton.SetActive(false);
        animator = GetComponentInChildren<Animator>();
    }
    private void FixedUpdate()
    {
        Movement();
    }
    void Update()
    {
        closestEnemy = GetClosestEnemy();
        
        if (closestEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, closestEnemy.position);
            ChangeLightColor(closestEnemy, distance);
            if (distance < 0.8f)
            {
                attackButton.SetActive(true);
            }


        }
        else
        {
            attackButton.SetActive(false);
            daggerCol.enabled = false;
        }
    }
    private void Movement()
    {
        Vector3 direction = Vector3.forward * fixedJoystick.Vertical + Vector3.right * fixedJoystick.Horizontal;
        if (Mathf.Abs(fixedJoystick.Vertical) > 0 && Mathf.Abs(fixedJoystick.Horizontal) > 0)
        {
            rb.velocity = direction * speed * Time.fixedDeltaTime;
            animator.SetBool("isRunning", true);
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetBool("isRunning", false);
        }
        rotation = Vector3.right * fixedJoystick.Horizontal + Vector3.forward * fixedJoystick.Vertical;
        transform.LookAt(transform.position + rotation);
    }
    public void Attack()
    {
        daggerCol.enabled = true;
        animator.SetTrigger("isAttacking");
    }

    void ChangeLightColor(Transform enemy, float distance)
    {
        if (distance > 2f)
        {
            spotLight.color = Color.Lerp(spotLight.color, Color.red, 2f * Time.deltaTime);
        }
    }

    private Transform GetClosestEnemy()
    {
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = Mathf.Infinity;
        Transform trans = null;
        foreach (GameObject enemy in allEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < minDistance)
            {
                minDistance = distanceToEnemy;
                trans = enemy.transform;
            }
        }
        return trans;
    }
}

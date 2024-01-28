using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedMobBehavior : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 8f;
    Damageable damageable;
    public Vector3 direction;
    private GameObject target;
    public float distanceToHit = 2f;
    public bool disrupted = false;
    public float attackPower;
    public float attackRange;

    public bool Disrupted {  get { return disrupted; } set { disrupted = value; } }
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    void Start()
    {
        damageable = GetComponent<Damageable>();
        rb = GetComponent<Rigidbody2D>();
        target = FindFirstObjectByType<PlayerController>().gameObject;
    }

    void Update()
    {
        direction = (target.transform.position - transform.position).normalized;
        if (direction.x < 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -8f;
            transform.localScale = localScale;
        }
        else
        {
            Vector3 localScale = transform.localScale;
            localScale.x = 8f;
            transform.localScale = localScale;
        }
        if (Vector3.Distance(transform.position, target.transform.position) < distanceToHit)
        {
            disrupted = true;
            rb.velocity = new Vector2(0, 0);
            animator.SetBool("Walk", false);
            animator.SetTrigger("Attack");

        }
        else
        {
            if (!disrupted)
            {
                rb.velocity = new Vector2(direction.x * speed, 0);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
            if (!IsGrounded() && !disrupted)
            {
                rb.velocity = new Vector2(0, -speed * 2);
            }
            animator.SetBool("Walk", !disrupted);
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
       
    }

    public void Attack()
    {
        Debug.Log("attack");
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D collider in hitPlayer)
        {
            Debug.Log("Collided");
            if (collider.GetComponent<Damageable>() != null)
            {
                collider.GetComponent<Damageable>().Damage(attackPower);
            }
        }
    }

    public void SetDisrupted(int val)
    {
        disrupted = val == 0 ? true : false;
    }


    public void DamagedHandler()
    {
        if (damageable.Hp > 0)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            animator.SetBool("IsDead", true);
        }
    }

    public void HandleDeath()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}

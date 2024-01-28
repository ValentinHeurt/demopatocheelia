using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMobBehavior : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float yPower = 8f;
    [SerializeField] private float ySpeed = 8f;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private float shootCooldown;
    [SerializeField] private GameObject projectilePrefab;

    Damageable damageable;
    public Vector3 direction;
    private GameObject target;
    public float distanceToHit = 2f;
    public bool disrupted = false;
    public float attackPower;
    public float attackRange;

    public bool Disrupted { get { return disrupted; } set { disrupted = value; } }
    [SerializeField] private Transform attackPoint;
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
            if (canShoot)
            {
                animator.SetTrigger("Attack");
                Attack();
            }
        }
        if (!disrupted)
        {
            rb.velocity = new Vector2(direction.x * speed, Mathf.Sin(Time.time * ySpeed) * yPower);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    public void Attack()
    {
        Debug.Log("attack");
        if (canShoot)
        {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            foreach (Collider2D collider in hitPlayer)
            {
                if (collider.GetComponent<Damageable>() != null)
                {
                    StartCoroutine(ShootCooldown());
                    Vector3 direction = (target.transform.position - attackPoint.position).normalized;
                    GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

                    projectile.transform.forward = direction;
                }
            }
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
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

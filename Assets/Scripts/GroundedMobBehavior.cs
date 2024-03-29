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
    public float lootPercentage;
    public GameObject prefabLoot;
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
        if (target == null) return;
        //Calcule de la direction du mob
        direction = (target.transform.position - transform.position).normalized;
        //Tourne le sprite du mob en fonction de sa direction
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
            //Gère les déplacement du mob, s'il est "disrupted" alors il ne bouge pas.
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
    //Vérifie que le mob est au sol ou non
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
       
    }
    // Cette méthode est appelée dans un animation event
    public void Attack()
    {
        Debug.Log("attack");
        // récupère les cibles qui sont dans sa range
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D collider in hitPlayer)
        {
            Debug.Log("Collided");
            //Si le collider est un Damageable il lui inflige des dégats
            if (collider.GetComponent<Damageable>() != null)
            {
                collider.GetComponent<Damageable>().Damage(attackPower);
            }
        }
    }
    // Ici on utilise un int et non un boolean simplement car cette méthode est appelée lors d'un animation event
    // On ne peut pas passer de bool dans un animation event
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
    //Détruit le mob s'il meurt
    public void HandleDeath()
    {
        int rand = Random.Range(0, 100);
        if (rand <= lootPercentage)
        {
            Instantiate(prefabLoot, transform.position, Quaternion.identity);    
        }
        Destroy(gameObject);
    }
    //Méthode utilisée dans l'éditeur pour montrer l'attack range du mob
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}

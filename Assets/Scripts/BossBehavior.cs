using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb; // RigidBody du mob
    [SerializeField] private Animator animator; // objet gérant les animations du mob
    [SerializeField] private float speed = 8f;
    [SerializeField] private float yPower = 8f;
    [SerializeField] private float ySpeed = 8f;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private float shootCooldown;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject endScreen;

    [SerializeField] private Vector3 aimOffset;

    bool isDead = false;
    bool disabled = true;
    Damageable damageable;
    public Vector3 direction;
    private GameObject target;
    public float distanceToHit = 2f;
    public float distanceToShoot = 5f;
    public bool disrupted = false;
    public float attackPower;
    public float attackRange;
    public float shootRange;

    public bool Disrupted { get { return disrupted; } set { disrupted = value; } }
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    void Start()
    {
        disabled = true;
        damageable = GetComponent<Damageable>();
        rb = GetComponent<Rigidbody2D>();
        target = FindFirstObjectByType<PlayerController>().gameObject;
        StartCoroutine(StartCooldown());
    }

    void Update()
    {
        if (isDead || disabled) return;
        if (target == null) return;
        //On trouve la diréction de déplacement du mob en fonction de la position du joueur (.normalized = donne le vecteur direction)
        direction = (target.transform.position - transform.position).normalized;
        //Tourne le sprite du mob en fonction de sa direction
        if (direction.x < 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -0.03f;
            transform.localScale = localScale;
        }
        else
        {
            Vector3 localScale = transform.localScale;
            localScale.x = 0.03f;
            transform.localScale = localScale;
        }

        //Calcule la distance entre le mob et le joueur, si elle est dans un certain range, le mob attaque
        if (Vector3.Distance(transform.position, target.transform.position) < distanceToHit)
        {
            disrupted = true;
            rb.velocity = new Vector2(0, 0);
            animator.SetBool("run", false);
            animator.SetTrigger("skill");
        }
        else if (Vector3.Distance(transform.position, target.transform.position) < distanceToShoot)
        {
            if (canShoot)
            {
                Shoot();
            }
        }
        else
        {
            animator.SetBool("run", !disrupted);
            if (!disrupted)
            {
                // le mob se déplace verticalement en suivant une courbe sin
                rb.velocity = new Vector2(direction.x * speed, 0);//Mathf.Sin(Time.time * ySpeed) * yPower);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
        }
        //Gère les déplacement du mob, s'il est "disrupted" alors il ne bouge pas.

    }

    public void Attack()
    {
        // récupère les cibles qui sont dans sa range
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D collider in hitPlayer)
        {
            //Si le collider est un Damageable il lance un projectile
            if (collider.GetComponent<Damageable>() != null)
            {
                collider.GetComponent<Damageable>().Damage(attackPower);
            }
        }
    }

    public void Shoot()
    {
        if (canShoot)
        {
            // récupère les cibles qui sont dans sa range
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, shootRange, playerLayer);
            foreach (Collider2D collider in hitPlayer)
            {
                //Si le collider est un Damageable il lance un projectile
                if (collider.GetComponent<Damageable>() != null)
                {
                    //Lance la coroutine de gestion du cooldown
                    StartCoroutine(ShootCooldown());
                    //Direction vers le joueur
                    Vector3 direction = ((target.transform.position + aimOffset)  - attackPoint.position).normalized;
                    //Génère un gameobject de projectile avec Instantiate
                    GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
                    //Donne la bonne direction au aprojectile
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
    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(1f);
        disabled = false;
    }

    public void SetDisrupted(int val)
    {
        disrupted = val == 0 ? true : false;
    }

    public void DamagedHandler()
    {
        if (damageable.Hp > 0)
        {
            animator.SetTrigger("hit_1");
        }
        else
        {
            animator.SetTrigger("death");
            isDead = true;
            StartCoroutine(StartDisplayEnd());
            rb.velocity = new Vector2(0, 0);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    IEnumerator StartDisplayEnd()
    {
        yield return new WaitForSeconds(1f);
        DisplayEnd();
    }

    public void DisplayEnd()
    {
        endScreen.SetActive(true);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMobBehavior : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb; // RigidBody du mob
    [SerializeField] private Animator animator; // objet g�rant les animations du mob
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
    // Cette fonction est automatiquement execut�e quand le jeu se lance
    void Start()
    {
        damageable = GetComponent<Damageable>();
        rb = GetComponent<Rigidbody2D>();
        target = FindFirstObjectByType<PlayerController>().gameObject;
    }
    void Update()
    {
        if (target == null) return;
        //On trouve la dir�ction de d�placement du mob en fonction de la position du joueur (.normalized = donne le vecteur direction)
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

        //Calcule la distance entre le mob et le joueur, si elle est dans un certain range, le mob attaque
        if (Vector3.Distance(transform.position, target.transform.position) < distanceToHit)
        {
            if (canShoot)
            {
                animator.SetTrigger("Attack");
                Attack();
            }
        }
        //G�re les d�placement du mob, s'il est "disrupted" alors il ne bouge pas.
        if (!disrupted)
        {
            // le mob se d�place verticalement en suivant une courbe sin
            rb.velocity = new Vector2(direction.x * speed, Mathf.Sin(Time.time * ySpeed) * yPower);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    public void Attack()
    {
        if (canShoot)
        {
            // r�cup�re les cibles qui sont dans sa range
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            foreach (Collider2D collider in hitPlayer)
            {
                //Si le collider est un Damageable il lance un projectile
                if (collider.GetComponent<Damageable>() != null)
                {
                    //Lance la coroutine de gestion du cooldown
                    StartCoroutine(ShootCooldown());
                    //Direction vers le joueur
                    Vector3 direction = (target.transform.position - attackPoint.position).normalized;
                    //G�n�re un gameobject de projectile avec Instantiate
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
    // Ici on utilise un int et non un boolean simplement car cette m�thode est appel�e lors d'un animation event
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
    //D�truit le mob s'il meurt
    public void HandleDeath()
    {
        Destroy(gameObject);
    }
    //M�thode utilis�e dans l'�diteur pour montrer l'attack range du mob
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

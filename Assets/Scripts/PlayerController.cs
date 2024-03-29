using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;
    private float horizontal;
    private float lastHorizontal;
    public float manaLossMultiplier;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float dashPower = 2f;
    private bool isFacingRight = true;
    private bool canShoot = true;
    public bool disrupted = false;
    public int potionHealAmount = 10;
    public int currentProjectilePrefab = 0;
    public UnityEvent<string> onPotionAmountChanged;
    [SerializeField] private bool canControlWhileAirborn = true;
    [SerializeField] Damageable damageable;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float airbornSpeed = 8f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private GameObject[] projectilePrefabs;
    [SerializeField] private GameObject hideDuringDash;
    [SerializeField] private GameObject dashTrail;
    [SerializeField] private GameObject dashPop;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 offset = new Vector3(0f,1f);
    [SerializeField] private Vector3 laserOffset = new Vector3(1f,1f);
    [SerializeField] private float shootCooldown;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float dashDuration;
    [SerializeField] private Transform skillSpawnPoint;
    [SerializeField] private float skillRadius;
    [SerializeField] private float skillDamage;
    public float laserMaxLength;
    public Vector3 endTest;
    private bool isDashing = false;
    private Vector3 laserDirection;
    private Quaternion laserRotation;

    [SerializeField] private GameObject[] Prefabs;
    public int prefabIndex;
    private GameObject currentLaserPrefab;

    private Ray RayMouse;

    public float maxMana;
    public UnityEvent<float> onManaUpdated; // Event appelé quand le mana est mis à jour

    private void Start()
    {
        // On se mets à l'écoute de l'event giveManaToPlayerOnHit et on lui affecte la méthode AddMana
        ProjectileMoveScript.giveManaToPlayerOnHit += AddMana;
        onPotionAmountChanged.Invoke(LevelManager.Instance.potionAmount.ToString());
        onManaUpdated.Invoke(playerData.mana / maxMana);
    }
    public void AddMana(float mana)
    {
        playerData.mana = Mathf.Clamp(playerData.mana + mana, 0, maxMana);
        onManaUpdated.Invoke(playerData.mana / maxMana);
    }

    public void SetPotionAmount(int value)
    {
        LevelManager.Instance.potionAmount = value;
        onPotionAmountChanged.Invoke(LevelManager.Instance.potionAmount.ToString());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (LevelManager.Instance.potionAmount > 0 && damageable.Hp < damageable.maxHp)
            {
                LevelManager.Instance.potionAmount -= 1;
                onPotionAmountChanged.Invoke(LevelManager.Instance.potionAmount.ToString());
                damageable.Heal(potionHealAmount);
            }
        }
        // Si le joueur est disrupted il ne peut rien faire
        if (!disrupted)
        {
            //Si un interactable est dans une certaine range, le joueur peut intéragire avec lui en appuyant sur F
            if (Input.GetKeyDown(KeyCode.F))
            {

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.GetComponent<Interactable>() != null)
                    {
                        collider.GetComponent<Interactable>().Interact();
                    }


                }
            }
            // On récupère les input horizontaux du joueur (flèches gauche droit et q/d)
            horizontal = Input.GetAxisRaw("Horizontal");

            //Pour test
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                currentProjectilePrefab = currentProjectilePrefab == projectilePrefabs.Count()-1 ? currentProjectilePrefab : currentProjectilePrefab + 1;
            }
            //Pour test
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                currentProjectilePrefab = currentProjectilePrefab == 0 ? currentProjectilePrefab : currentProjectilePrefab - 1;
            }

            if (horizontal != 0 && IsGrounded())
                animator.SetBool("isRun", true);
            else
                animator.SetBool("isRun", false);

            animator.SetBool("isJump", !IsGrounded());

            //Impulsion du saut
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                lastHorizontal = horizontal;
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            if (!IsGrounded() && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(StartDash());
            }

            //if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            //{
            //    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            //}

            //if (Input.GetMouseButtonDown(1))
            //{
            //    animator.SetTrigger("attack");
            //}
            // Tire de projectile
            if (Input.GetMouseButtonDown(0) && canShoot)
            {
                StartCoroutine(ShootCooldown());
                //On récupère la position de la souris
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                //Calcule de la direction entre le joueur et la position de la souris
                Vector3 direction = (mousePosition - (transform.position + offset)).normalized;
                //Création du projectile
                GameObject projectile = Instantiate(projectilePrefabs[currentProjectilePrefab], transform.position + offset + 2 * direction, Quaternion.identity);
                //On lui donne sa direction
                projectile.transform.forward = direction;
                //projectile.GetComponent<Rigidbody>().AddForce(projectileSpeed * direction, ForceMode.Impulse);
            }
            if(IsGrounded() || canControlWhileAirborn)
                Flip();

            //Début du laser
            if (Input.GetMouseButtonDown(1) && playerData.mana >= 100)
            {
                currentLaserPrefab = Instantiate(Prefabs[prefabIndex], skillSpawnPoint.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                StartCoroutine(StartSkillDamage());
                playerData.mana = 0;
                onManaUpdated.Invoke(playerData.mana);
            }
            //Disable lazer prefab
            //if (Input.GetMouseButtonUp(1))
            //{
            //    Destroy(currentLaserPrefab, 1);
            //}
        }
    }
    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        destination.z = 0;
        print(destination);
        laserDirection = destination - obj.transform.position;
        if (!isFacingRight)
        {
            laserDirection.x = -laserDirection.x;
        }
        laserRotation = Quaternion.LookRotation(laserDirection);
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, laserRotation, 1);
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = new Vector2((horizontal == 0 ? (isFacingRight ? 1 : -1) : horizontal) * airbornSpeed * dashPower, rb.velocity.y);
        }
        else
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((canControlWhileAirborn ? horizontal : lastHorizontal) * airbornSpeed, rb.velocity.y);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    public void SetDisrupted(int val)
    {
        disrupted = val == 0 ? true : false;
    }

    public void inverseDisrupted()
    {
        disrupted = !disrupted;
    }

    public void DamagedHandler()
    {
        if (playerData.hp > 0)
        {
            animator.SetTrigger("hurt");
        }
        else
        {
            disrupted = true;
            animator.SetBool("die", true);
            Destroy(gameObject);
        }
    }

    IEnumerator StartDash()
    {
        isDashing = true;
        hideDuringDash.SetActive(false);
        dashTrail.SetActive(true);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        dashTrail.SetActive(false);
        hideDuringDash.SetActive(true);
        Instantiate(dashPop,transform.position + new Vector3(0,1,0), Quaternion.identity);
    }

    IEnumerator StartSkillDamage()
    {
        yield return new WaitForSeconds(1f);
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(skillSpawnPoint.position, skillRadius, enemyLayer);
        foreach (var enemy in allEnemies)
        {
            enemy.GetComponent<Damageable>().Damage(skillDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(skillSpawnPoint.position, skillRadius);
    }

}
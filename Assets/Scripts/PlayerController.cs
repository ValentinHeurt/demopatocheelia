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
    [SerializeField] private float airbornSpeed = 8f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private GameObject[] projectilePrefabs;
    [SerializeField] private GameObject hideDuringDash;
    [SerializeField] private GameObject dashTrail;
    [SerializeField] private VisualEffect dashPop;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 offset = new Vector3(0f,1f);
    [SerializeField] private Vector3 laserOffset = new Vector3(1f,1f);
    [SerializeField] private float shootCooldown;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float dashDuration;
    public float laserMaxLength;
    public Vector3 endTest;
    private bool isDashing = false;
    private Vector3 laserDirection;
    private Quaternion laserRotation;

    [SerializeField] private GameObject[] Prefabs;
    public int prefabIndex;
    private GameObject currentLaserPrefab;

    private Hovl_Laser LaserScript;
    private Hovl_Laser2 LaserScript2;
    private Ray RayMouse;

    public float maxMana;
    public float mana;
    public UnityEvent<float> onManaUpdated; // Event appelé quand le mana est mis à jour

    private void Start()
    {
        // On se mets à l'écoute de l'event giveManaToPlayerOnHit et on lui affecte la méthode AddMana
        ProjectileMoveScript.giveManaToPlayerOnHit += AddMana;
        mana = 0;
        onPotionAmountChanged.Invoke(LevelManager.Instance.potionAmount.ToString());
        onManaUpdated.Invoke(mana);
        //StartCoroutine(StartManaRegen());
    }
    public void AddMana(float mana)
    {
        this.mana = Mathf.Clamp(this.mana + mana, 0, maxMana);
        onManaUpdated.Invoke(this.mana/maxMana);
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
            if (Input.GetMouseButtonDown(1) && mana != 0)
            {
                Destroy(currentLaserPrefab);
                Vector3 pos = isFacingRight ? (transform.position + laserOffset) : (transform.position + new Vector3(-2, 2, 0));
                currentLaserPrefab = Instantiate(Prefabs[prefabIndex], pos, Quaternion.Euler(0,90,0));
                print(currentLaserPrefab.transform.rotation);
                currentLaserPrefab.transform.parent = transform;
                LaserScript = currentLaserPrefab.GetComponent<Hovl_Laser>();
                LaserScript2 = currentLaserPrefab.GetComponent<Hovl_Laser2>();
            }
            if (Input.GetMouseButton(1) && currentLaserPrefab != null)
            {
                mana -= Time.deltaTime * manaLossMultiplier;
                onManaUpdated.Invoke(mana / maxMana);
            }
            //Disable lazer prefab
            if (Input.GetMouseButtonUp(1) || mana <= 0)
            {
                if (LaserScript) LaserScript.DisablePrepare();
                if (LaserScript2) LaserScript2.DisablePrepare();
                Destroy(currentLaserPrefab, 1);
            }
        }
        //Current fire point
        if (mainCamera != null && currentLaserPrefab != null)
        {
            var mousePos = Input.mousePosition;
            RayMouse = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit2D hit = Physics2D.Raycast(RayMouse.origin, RayMouse.direction, laserMaxLength);
            if (hit.collider != null)
            {
                RotateToMouseDirection(currentLaserPrefab, hit.point);
                //LaserEndPoint = hit.point;
            }
            else
            {
                var pos = RayMouse.GetPoint(laserMaxLength);
                RotateToMouseDirection(currentLaserPrefab, pos);
                //LaserEndPoint = pos;
            }
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
        if (damageable.Hp > 0)
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
        dashPop.SendEvent(Shader.PropertyToID("Play"));
    }
    IEnumerator StartManaRegen()
    {
        yield return new WaitForSeconds(2);
        mana = (mana + 5) > maxMana ? maxMana : mana + 5;
        onManaUpdated.Invoke(mana/maxMana);
        StartCoroutine(StartManaRegen());
    }

}
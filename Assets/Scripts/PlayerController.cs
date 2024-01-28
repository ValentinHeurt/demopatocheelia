using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float lastHorizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float dashPower = 2f;
    private bool isFacingRight = true;
    private bool canShoot = true;
    public bool disrupted = false;
    public int currentProjectilePrefab = 0;
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
    [SerializeField] private float shootCooldown;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float dashDuration;
    public Vector3 endTest;
    private bool isDashing = false;

    void Update()
    {
        if (!disrupted)
        {
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

            horizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                currentProjectilePrefab = currentProjectilePrefab == projectilePrefabs.Count()-1 ? currentProjectilePrefab : currentProjectilePrefab + 1;
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                currentProjectilePrefab = currentProjectilePrefab == 0 ? currentProjectilePrefab : currentProjectilePrefab - 1;
            }

            if (horizontal != 0 && IsGrounded())
                animator.SetBool("isRun", true);
            else
                animator.SetBool("isRun", false);

            animator.SetBool("isJump", !IsGrounded());

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

            if (Input.GetMouseButtonDown(1))
            {
                animator.SetTrigger("attack");
            }

            if (Input.GetMouseButtonDown(0) && canShoot)
            {
                StartCoroutine(ShootCooldown());
                print("ddd");
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                Vector3 direction = (mousePosition - (transform.position + offset)).normalized;
                GameObject projectile = Instantiate(projectilePrefabs[currentProjectilePrefab], transform.position + offset + 2 * direction, Quaternion.identity);

                projectile.transform.forward = direction;
                //projectile.GetComponent<Rigidbody>().AddForce(projectileSpeed * direction, ForceMode.Impulse);
            }
            if(IsGrounded() || canControlWhileAirborn)
                Flip();
        }
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

}
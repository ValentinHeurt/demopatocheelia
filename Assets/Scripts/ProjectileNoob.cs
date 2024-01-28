using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileNoob : MonoBehaviour
{
    Vector3 pos;
    public float speed;
    public float damage;
    public LayerMask ignoreLayer;

    private void FixedUpdate()
    {
        pos = (transform.forward) * (speed * Time.deltaTime);
        pos.z = 0;
        transform.position += (transform.forward) * (speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != ignoreLayer)
        {
            if (collision.gameObject.GetComponent<Damageable>() != null)
            {
                collision.gameObject.GetComponent<Damageable>().Damage(damage);
            }
            Destroy(gameObject);
        }
    }
}

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using ClearSky;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMoveScript : MonoBehaviour {

    public bool rotate = false;
    public float rotateAmount = 45;
    public bool bounce = false;
    public float bounceForce = 10;
    public float speed;
    public float damage;
    public float manaBoost;
	[Tooltip("From 0% to 100%")]
	public float accuracy;
	public float fireRate;
	public GameObject muzzlePrefab;
	public GameObject hitPrefab;
	public List<GameObject> trails;
    public LayerMask ignoreLayer;

    Vector3 pos;
    private Vector3 startPos;
	private float speedRandomness;
	private Vector3 offset;
	private bool collided;
	[SerializeField] private Rigidbody2D rb;
    private RotateToMouseScript rotateToMouse;
    private GameObject target;

    public static Action<float> giveManaToPlayerOnHit;

	void Start () {
        startPos = transform.position;

		//used to create a radius for the accuracy and have a very unique randomness
		if (accuracy != 100) {
			accuracy = 1 - (accuracy / 100);

			for (int i = 0; i < 2; i++) {
				var val = 1 * UnityEngine.Random.Range (-accuracy, accuracy);
				var index = UnityEngine.Random.Range (0, 2);
				if (i == 0) {
					if (index == 0)
						offset = new Vector3 (0, -val, 0);
					else
						offset = new Vector3 (0, val, 0);
				} else {
					if (index == 0)
						offset = new Vector3 (0, offset.y, -val);
					else
						offset = new Vector3 (0, offset.y, val);
				}
			}
		}
			
		if (muzzlePrefab != null) {
			var muzzleVFX = Instantiate (muzzlePrefab, transform.position, Quaternion.identity);
			muzzleVFX.transform.forward = gameObject.transform.forward + offset;
			var ps = muzzleVFX.GetComponent<ParticleSystem>();
			if (ps != null)
				Destroy (muzzleVFX, ps.main.duration);
			else {
				var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy (muzzleVFX, psChild.main.duration);
			}
		}
	}

	void FixedUpdate () {
        if (speed != 0 && rb != null)
        {
            pos = (transform.forward + offset) * (speed * Time.deltaTime);
            pos.z = 0;
            transform.position += (transform.forward + offset) * (speed * Time.deltaTime);
        }
    }
    public void OnChildCollisionEnter2D(Collision2D collision)
    {
        if (1 << collision.gameObject.layer != ignoreLayer)
        {
            print("YAZAA");
            if (collision.gameObject.GetComponent<Damageable>() != null)
            {
                collision.gameObject.GetComponent<Damageable>().Damage(damage);
                giveManaToPlayerOnHit.Invoke(manaBoost);
            }
            if (collision.gameObject.tag != "Bullet" && !collided)
            {
                collided = true;

                if (trails.Count > 0)
                {
                    for (int i = 0; i < trails.Count; i++)
                    {
                        trails[i].transform.parent = null;
                        var ps = trails[i].GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Stop();
                            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                        }
                    }
                }

                speed = 0;
                rb.isKinematic = true;

                ContactPoint2D contact = collision.contacts[0];
                Vector3 pos = contact.point;

                if (hitPrefab != null)
                {
                    var hitVFX = Instantiate(hitPrefab, pos, transform.rotation) as GameObject;

                    var ps = hitVFX.GetComponent<ParticleSystem>();
                    if (ps == null)
                    {
                        var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                        Destroy(hitVFX, psChild.main.duration);
                    }
                    else
                        Destroy(hitVFX, ps.main.duration);
                }

                StartCoroutine(DestroyParticle(0f));
            }
        }
        //else
        //{
        //    rb.drag = 0.5f;
        //    ContactPoint contact = co.contacts[0];
        //    rb.AddForce(Vector3.Reflect((contact.point - startPos).normalized, contact.normal) * bounceForce, ForceMode.Impulse);
        //    Destroy ( this );
        //}
    }

	public IEnumerator DestroyParticle (float waitTime) {

		if (transform.childCount > 0 && waitTime != 0) {
			List<Transform> tList = new List<Transform> ();

			foreach (Transform t in transform.GetChild(0).transform) {
				tList.Add (t);
			}		

			while (transform.GetChild(0).localScale.x > 0) {
				yield return new WaitForSeconds (0.01f);
				transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				for (int i = 0; i < tList.Count; i++) {
					tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				}
			}
		}
		
		yield return new WaitForSeconds (waitTime);
		Destroy (gameObject);
	}

    public void SetTarget (GameObject trg, RotateToMouseScript rotateTo)
    {
        target = trg;
        rotateToMouse = rotateTo;
    }
}

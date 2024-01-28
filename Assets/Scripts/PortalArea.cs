using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalArea : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject portal;
    bool entered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null && entered == false)
        {
            Debug.Log("aaaa");
            entered = true;
            portal.SetActive(true);
        }
    }


}

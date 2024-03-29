using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lootable : MonoBehaviour
{
    public float waitDurationBeforeLootable = 1f;
    private bool canPick = false;
    private void Awake()
    {
        StartCoroutine(WaitDuration());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canPick) return;
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().SetPotionAmount(LevelManager.Instance.potionAmount + 1);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canPick) return;
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().SetPotionAmount(LevelManager.Instance.potionAmount + 1);
            Destroy(gameObject);
        }
    }

    public IEnumerator WaitDuration()
    {
        yield return new WaitForSeconds(waitDurationBeforeLootable);
        canPick = true;
    }
}

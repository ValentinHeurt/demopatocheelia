using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public float maxHp;
    [SerializeField] private float hp;
    public float Hp {  get { return hp; } }
    public UnityEvent damagedEvent;
    public UnityEvent<float> onHealthUpdated;
    public bool invincible = false;
    public float invincibleTime;
    void Start()
    {
        hp = maxHp;
    }

    public void Damage(float damage)
    {
        if (!invincible)
        {
            StartCoroutine(InvincibilityFrames());
            hp -= damage;
            if (hp < 0) { hp = 0; }
            damagedEvent.Invoke();
            onHealthUpdated.Invoke(hp / maxHp);
        }
    }

    IEnumerator InvincibilityFrames()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
    }
}

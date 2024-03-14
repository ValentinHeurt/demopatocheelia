using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Class représentant toute entités pouvant prendre des dégats
public class Damageable : MonoBehaviour
{
    public float maxHp;
    [SerializeField] private float hp;
    public float Hp {  get { return hp; } }
    public UnityEvent damagedEvent; // évenement executé quand l'entité reçoit des dégats
    public UnityEvent<float> onHealthUpdated; //évenement executé quand la vie est mise à jour
    public bool invincible = false;
    public float invincibleTime;
    // Cette fonction est automatiquement executée quand le jeu se lance
    void Start()
    {
        hp = maxHp;
    }
    // Cette fonction est utilisé par les composant exterieurs voulant infliger des dégats à l'entité
    public void Damage(float damage)
    {
        if (!invincible && hp != 0f)
        {
            // Lance la coroutine de gestion des frams d'invincibilités
            StartCoroutine(InvincibilityFrames());
            hp -= damage;
            if (hp < 0) { hp = 0; }
            //Lance les events
            damagedEvent.Invoke();
            onHealthUpdated.Invoke(hp / maxHp);
        }
    }

    public void Heal(float heal)
    {
        if (hp != 0f)
        {
            hp += heal;
            if (hp > maxHp) { hp = maxHp; }
            onHealthUpdated.Invoke(hp / maxHp);
        }
    }
    // Quand cette coroutine est lancée, elle rend le joueur invincible pour "invincibleTime" secondes puis le rend à nouveau vulnérable
    IEnumerator InvincibilityFrames()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Class repr�sentant toute entit�s pouvant prendre des d�gats
public class Damageable : MonoBehaviour
{
    public PlayerData playerData;
    public float maxHp;
    [SerializeField] private float hp;
    public float Hp {  get { return hp; } }
    public UnityEvent damagedEvent; // �venement execut� quand l'entit� re�oit des d�gats
    public UnityEvent<float> onHealthUpdated; //�venement execut� quand la vie est mise � jour
    public bool invincible = false;
    public float invincibleTime;
    // Cette fonction est automatiquement execut�e quand le jeu se lance
    void Start()
    {
        if (gameObject.GetComponent<PlayerController>() != null)
        {
            hp = playerData.hp;
            onHealthUpdated.Invoke(playerData.hp / maxHp);
        }
        else
        {
            hp = maxHp;
        }
    }
    // Cette fonction est utilis� par les composant exterieurs voulant infliger des d�gats � l'entit�
    public void Damage(float damage)
    {
        if (gameObject.GetComponent<PlayerController>() != null)
        {
            if (!invincible && playerData.hp != 0f)
            {
                // Lance la coroutine de gestion des frams d'invincibilit�s
                StartCoroutine(InvincibilityFrames());
                playerData.hp -= damage;
                if (playerData.hp < 0) { playerData.hp = 0; }
                //Lance les events
                damagedEvent.Invoke();
                onHealthUpdated.Invoke(playerData.hp / maxHp);
            }
        }
        else
        {
            if (!invincible && hp != 0f)
            {
                // Lance la coroutine de gestion des frams d'invincibilit�s
                StartCoroutine(InvincibilityFrames());
                hp -= damage;
                if (hp < 0) { hp = 0; }
                //Lance les events
                damagedEvent.Invoke();
                onHealthUpdated.Invoke(hp / maxHp);
            }
        }

    }

    public void Heal(float heal)
    {

        if (gameObject.GetComponent<PlayerController>() != null)
        {
            if (hp != 0f)
            {
                playerData.hp += heal;
                if (playerData.hp > maxHp) { playerData.hp = maxHp; }
                onHealthUpdated.Invoke(playerData.hp / maxHp);
            }
        }
        else
        {
            if (hp != 0f)
            {
                hp += heal;
                if (hp > maxHp) { hp = maxHp; }
                onHealthUpdated.Invoke(hp / maxHp);
            }
        }
    }
    // Quand cette coroutine est lanc�e, elle rend le joueur invincible pour "invincibleTime" secondes puis le rend � nouveau vuln�rable
    IEnumerator InvincibilityFrames()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
    }
}

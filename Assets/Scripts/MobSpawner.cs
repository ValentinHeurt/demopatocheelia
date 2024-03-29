using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private int mobAmount;
    [SerializeField] private GameObject mobPrefab;
    private int currentMobsSpawned = 0;
    private void Start()
    {
        //On lance le spawn des mobs au start
        Debug.Log("mob amount before: " + mobAmount);
        mobAmount = (int)(mobAmount * LevelManager.Instance.difficultyMultiplier);
        Debug.Log("mob amount after: " + mobAmount);
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        // On attend x secondes avant de spawn un mob
        yield return new WaitForSeconds(timer);
        currentMobsSpawned++;
        Instantiate(mobPrefab, transform.position, Quaternion.identity);
        // Quand le nombre de mob à spawn est atteint on ne rentre plus dans ce if et donc on stop le spawn
        if (currentMobsSpawned < mobAmount)
        {
            StartCoroutine(Spawn());
        }
    }
}

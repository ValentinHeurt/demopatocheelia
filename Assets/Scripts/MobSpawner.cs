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
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(timer);
        currentMobsSpawned++;
        Instantiate(mobPrefab, transform.position, Quaternion.identity);
        if (currentMobsSpawned < mobAmount)
        {
            StartCoroutine(Spawn());
        }
    }
}

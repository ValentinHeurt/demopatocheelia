using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.LucidEditor;
using DG.Tweening;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Chest : MonoBehaviour
    {

        public int potionAmount = 2;
        public GameObject prefab;
        public GameObject prefabMob;
        public float borneMin = -5f;
        public float borneMax = 5f;
        public float jumpPower = 1f;
        public float jumpDuration = 1f;
        public int numJump = 1;
        public float durationBetweenPotion = 1f;
        public Vector3 offset;
        public IEnumerator StartSpawn()
        {
            if (potionAmount == 0) yield return null;
            for (int i = 0; i < potionAmount; i++)
            {
                GameObject potion = Instantiate(prefab, transform.position + offset, Quaternion.identity);
                potion.transform.DOJump(new Vector3(transform.position.x + Random.Range(borneMin, borneMax), transform.position.y, transform.position.z) + offset, jumpPower, numJump, jumpDuration);
                yield return new WaitForSeconds(durationBetweenPotion);
            }
        }
        public IEnumerator StartSpawnMob()
        {
            if (potionAmount == 0) yield return null;
            for (int i = 0; i < potionAmount; i++)
            {
                GameObject potion = Instantiate(prefabMob, transform.position + offset, Quaternion.identity);
                potion.transform.DOJump(new Vector3(transform.position.x + Random.Range(borneMin, borneMax), transform.position.y, transform.position.z) + offset, jumpPower, numJump, jumpDuration);
                yield return new WaitForSeconds(durationBetweenPotion);
            }
        }
        [FoldoutGroup("Reference")]
        public Animator animator;

        [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                animator.SetBool("IsOpened", isOpened);
            }
        }
        private bool isOpened;

        [FoldoutGroup("Runtime"),Button("Open"), HorizontalGroup("Runtime/Button")]
        public void Open()
        {
            if (isOpened) return;
            IsOpened = true;
            if (Random.Range(0,100) < 1000)
            {
                StartCoroutine(StartSpawnMob());
            }
            else
            {
                StartCoroutine(StartSpawn());
            }
        }

        [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
        public void Close()
        {
            IsOpened = false;
        }
    }
}

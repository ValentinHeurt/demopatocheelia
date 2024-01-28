using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent e;

    public void Interact()
    {
        Debug.Log("aabb");
        e.Invoke();
    }
}

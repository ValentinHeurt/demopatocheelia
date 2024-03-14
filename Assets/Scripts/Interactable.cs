using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Cette classe permet simplement d'activer un évent lié au gameobject quand on appel la méthode "Interact()"
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent e;

    public void Interact()
    {
        Debug.Log("aabb");
        e.Invoke();
    }
}

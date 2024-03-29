using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Cette classe permet simplement d'activer un �vent li� au gameobject quand on appel la m�thode "Interact()"
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent e;

    public void Interact()
    {
        Debug.Log("aabb");
        e.Invoke();
    }
}

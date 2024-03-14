using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionDetector : MonoBehaviour
{
    // Le gameobject du parent
    [SerializeField] private ProjectileMoveScript parent;

    // Cette fonction s'execute quand un collider entre en collision avec le collider de cet objet
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //On appel la fonction de collision du parent
        parent.OnChildCollisionEnter2D(collision);
    }

}

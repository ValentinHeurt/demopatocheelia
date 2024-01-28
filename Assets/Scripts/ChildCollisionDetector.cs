using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionDetector : MonoBehaviour
{
    [SerializeField] private ProjectileMoveScript parent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        parent.OnChildCollisionEnter2D(collision);
    }

}

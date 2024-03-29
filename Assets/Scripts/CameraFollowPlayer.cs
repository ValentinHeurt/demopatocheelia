using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    //Vitesse de suivi de la cam�ra
    public float FollowSpeed = 2f;
    //OffSet horizontal
    public float yOffSet = 1f;
    //Variable repr�sentant 
    public Transform target;
    //S'execute � chaque frame
    void Update()
    {
        if (target != null)
        {
            //Position suivante de la cam�ra
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffSet, -10f);
            //La position de la camera sera �gale � l'interpolation entre la position actuelle et la position suivante.
            // On utilise Time.deltaTime pour lisser le r�sultat quel que soit le framerate de l'utilisateur car Time.deltaTime = le temps �coul� depuis
            // la derni�re frame
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
    }
}

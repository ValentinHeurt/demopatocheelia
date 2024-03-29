using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    //Vitesse de suivi de la caméra
    public float FollowSpeed = 2f;
    //OffSet horizontal
    public float yOffSet = 1f;
    //Variable représentant 
    public Transform target;
    //S'execute à chaque frame
    void Update()
    {
        if (target != null)
        {
            //Position suivante de la caméra
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffSet, -10f);
            //La position de la camera sera égale à l'interpolation entre la position actuelle et la position suivante.
            // On utilise Time.deltaTime pour lisser le résultat quel que soit le framerate de l'utilisateur car Time.deltaTime = le temps écoulé depuis
            // la dernière frame
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
    }
}

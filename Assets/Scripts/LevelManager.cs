using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cette class est un singleton, il ne peut en avoir qu'une instance
// Elle gère les niveau
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int currentLevel = 0;
    public int potionAmount = 0;
    public bool firstLevel = false;
    public bool secondLevel = false;
    public bool thirdLevel = false;
    private void Awake()
    {
        // Si l'instance existe déjà on Destroy l'object actuel
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        //Sinon on affect à la variable instance l'object actuel
        Instance = this;
        //Cet objet ne doit pas être détruit en changeant de scene
        DontDestroyOnLoad(Instance);
    }
}

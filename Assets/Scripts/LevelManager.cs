using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cette class est un singleton, il ne peut en avoir qu'une instance
// Elle g�re les niveau
public class LevelManager : MonoBehaviour
{
    public PlayerData playerData;
    public static LevelManager Instance;
    public int currentLevel = 0;
    public int potionAmount = 0;
    public float difficultyMultiplier = 1;
    public bool firstLevel = false;
    public bool secondLevel = false;
    public bool thirdLevel = false;
    private void Awake()
    {
        // Si l'instance existe d�j� on Destroy l'object actuel
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        //Sinon on affect � la variable instance l'object actuel
        Instance = this;
        playerData.mana = 0f;
        playerData.hp = 50f;
        //Cet objet ne doit pas �tre d�truit en changeant de scene
        DontDestroyOnLoad(Instance);
    }
}

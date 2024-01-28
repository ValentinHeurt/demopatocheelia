using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!LevelManager.Instance.secondLevel)
        {
            LevelManager.Instance.currentLevel = 2;
        }
    }

}

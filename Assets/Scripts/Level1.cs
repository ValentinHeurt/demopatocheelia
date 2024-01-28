using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!LevelManager.Instance.firstLevel)
        {
            LevelManager.Instance.currentLevel = 1;
        }
    }

}

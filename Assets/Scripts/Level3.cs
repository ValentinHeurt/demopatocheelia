using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!LevelManager.Instance.thirdLevel)
        {
            LevelManager.Instance.currentLevel = 3;
        }
    }

}

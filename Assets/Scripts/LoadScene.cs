using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Cette class permet de changer de scene
public class LoadScene : MonoBehaviour
{
    [SerializeField] private int sceneToLoad;

    public void Load()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}

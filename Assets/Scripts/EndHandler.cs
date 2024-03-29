using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndHandler : MonoBehaviour
{
    public void Continue()
    {
        LevelManager.Instance.difficultyMultiplier += 0.5f;
        SceneManager.LoadScene(2);
    }
    public void Stop()
    {
        LevelManager.Instance.difficultyMultiplier =1f;
        SceneManager.LoadScene(0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HubManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoses;
    [SerializeField] GameObject player;
    [SerializeField] GameObject portal1First;
    [SerializeField] GameObject portal1After;
    [SerializeField] GameObject portal2First;
    [SerializeField] GameObject portal2After;
    [SerializeField] GameObject megaPortalFirst;
    [SerializeField] GameObject m_Camera;
    [SerializeField] PlayableDirector director;
    void Start()
    {
        player.transform.position = spawnPoses[LevelManager.Instance.currentLevel].transform.position;
        m_Camera.transform.position = new Vector3(spawnPoses[LevelManager.Instance.currentLevel].transform.position.x, m_Camera.transform.position.y, m_Camera.transform.position.z);
        switch (LevelManager.Instance.currentLevel)
        {
            case 1:
                InitFirstPortal();
                LevelManager.Instance.firstLevel = true;
                break;
            case 2:
                InitFirstPortal();
                InitSecondPortal();
                LevelManager.Instance.secondLevel = true;
                break;
            case 3:
                InitFirstPortal();
                InitSecondPortal();
                InitMegaPortal();
                LevelManager.Instance.thirdLevel = true;
                break;
            default:
                break;
        }

    }

    private void InitFirstPortal()
    {
        if (LevelManager.Instance.firstLevel)
        {
            portal1After.SetActive(true);
        }
        else
        {
            portal1First.SetActive(true);
        }
    }
    private void InitSecondPortal()
    {
        if (LevelManager.Instance.secondLevel)
        {
            portal2After.SetActive(true);
        }
        else
        {
            portal2First.SetActive(true);
        }
    }
    private void InitMegaPortal()
    {
        director.Play();
    }
}

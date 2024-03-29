using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Cette class gère le hub, l'initialise et ouvre les portails en fonction du niveau.
public class HubManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoses;
    [SerializeField] GameObject player;
    [SerializeField] GameObject megaPortalFirst;
    [SerializeField] GameObject m_Camera;
    [SerializeField] PlayableDirector director;
    void Start()
    {
        player.transform.position = spawnPoses[LevelManager.Instance.currentLevel].transform.position;
        m_Camera.transform.position = new Vector3(spawnPoses[LevelManager.Instance.currentLevel].transform.position.x, m_Camera.transform.position.y, m_Camera.transform.position.z);
        InitMegaPortal();

    }

    private void InitMegaPortal()
    {
        director.Play();
    }
}

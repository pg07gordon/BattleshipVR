using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - 
* Notes: 
* By Gordon Niemann
* Build - Jan 29th 2017
*/

public class RightVRController : MonoBehaviour
{
    SteamVR_TrackedController controller;

    // Use this for initialization
    void Start ()
    {

        controller = GetComponent<SteamVR_TrackedController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (controller != null && controller.triggerPressed)
        {
            GameManager.Instance.m_PlayersShip.GetComponent<MissileController>().RequsetLaunch();
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Barrel Exit Location
* Notes: Reference Only
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class BarrelExit : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        transform.rotation = transform.parent.rotation;
	}
	
}

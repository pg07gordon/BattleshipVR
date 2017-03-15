using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Beam Turret
* Notes:  
* By Gordon Niemann
* Build - 
*/

public class BeamTurret : TurretBase
{
    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        RotateTurretToTarget();
    }
}

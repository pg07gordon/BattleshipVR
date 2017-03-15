using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Missle Launcher Ejection Cap
* Notes: Effects and reference 
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class EjectionCap : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 torque;

    // Use this for initialization
    void Awake ()
    {
        rb = GetComponent<Rigidbody>();

        torque = new Vector3();
        torque.x = Random.Range(-180, 180);
        torque.y = Random.Range(-180, 180);
        torque.z = Random.Range(-180, 180);
    }

    public void Eject()
    {
        rb.AddForce(-transform.up * Random.Range(0.05f,0.1f), ForceMode.Impulse);
        rb.AddForce(-transform.right * Random.Range(0.01f, 0.05f), ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }
}

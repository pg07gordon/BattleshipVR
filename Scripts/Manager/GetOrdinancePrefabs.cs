using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - 
* Notes: 
* By Gordon Niemann
* Build - Jan 29th 2017
*/

[ExecuteInEditMode]
public class GetOrdinancePrefabs : Singleton<GetOrdinancePrefabs>
{
    static internal GameObject[] m_GameOrdinance;
    int m_GameOrdinanceSize = 0;

    void Awake()
    {
        GetPrefabs();
    }

    void Update ()
    {
        if (!Application.isPlaying)
        {
            GetPrefabs();
            print("Game Ordinance Prefabs Array Updated with: " + m_GameOrdinanceSize + " items.");
        }
    }

    void GetPrefabs()
    {
        //m_GameOrdinance = Resources.LoadAll("OrdinancePrefabs") as GameObject[];
        m_GameOrdinance = Resources.LoadAll<GameObject>("OrdinancePrefabs");
        //m_GameOrdinanceSize = m_GameOrdinance.Length;
    }
}

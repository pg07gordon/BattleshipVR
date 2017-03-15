using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Grid Controller
* Notes: Gide Material updater (right now)
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class GridV2 : MonoBehaviour
{

    public Material     m_SelectedMaterial;
    private Material    m_UnselectedMaterial;
    private Renderer    m_MyRenderer;

    private void Start()
    {
        m_MyRenderer = GetComponent<Renderer>();
        m_UnselectedMaterial = m_MyRenderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_MyRenderer.material = m_SelectedMaterial;

        if (GameManager.m_SelectedShip != null)
        {
            float x = transform.position.x;
            float y = 0.5f;
            float z = transform.position.z;
            GameManager.m_SelectedShip.transform.position = new Vector3(x,y,z);

            GameManager.m_SelectedShip = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_MyRenderer.material = m_UnselectedMaterial;
    }

}

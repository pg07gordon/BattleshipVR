using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
* VR Battleship - Starship Object
* Notes: Not doing much much right now
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class Starship : Targetable
{
    public Material     m_SelectedMaterial;
    private Material    m_MyMaterial;
    private Renderer    m_MyRenderer;

    // Use this for initialization
    new void Start ()
    {
        base.Start();

        m_MyRenderer = GetComponent<Renderer>();
        m_MyMaterial = m_MyRenderer.material;
    }

	// Update is called once per frame
	void Update ()
    {
        //UpdateMyVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        m_MyRenderer.material = m_SelectedMaterial;
        GameManager.SetSelectedShip(this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        m_MyRenderer.material = m_MyMaterial;
    }
}

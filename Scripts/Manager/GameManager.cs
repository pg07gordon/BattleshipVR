using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
* VR Battleship - Singleton Game Manager
* Notes: Right now just a holder for communal functions
* By Gordon Niemann
* Build - Jan 29th 2017
*/

public class GameManager : Singleton<GameManager>
{
    public int m_iTurret2ProjectileTotal = 100;
    public GameObject pointerToken;

    public GameObject m_PlayersShip;

    public static GameObject m_MapOrientationUp;
    internal static GameObject m_SelectedShip;

    //internal GameObject[] m_Turrets;
    //internal GameObject[][] m_ProjectilePool;
    //internal GameObject m_ProjectilePoolContainer;

    private void Awake()
    {
        m_MapOrientationUp = GameObject.Find("MapOrientationUp");

        GameObject projectilePoolContainer = GameObject.Find("ResourcePool");

        if (projectilePoolContainer == null)
        {
            projectilePoolContainer = new GameObject();
            projectilePoolContainer.name = "ResourcePool";
        }

        //GameObject projectile;
        //int totalShips = FindObjectsOfType<Ship>().Length;

        //List<GameObject> turrets = new List<GameObject>();

        //foreach (GameObject t in FindObjectsOfType<GameObject>())
        //{
        //    TurretV2 script = t.GetComponent<TurretV2>();

        //    if (script != null)
        //    {
        //        turrets.Add(t);
        //    }
        //}

        //m_Turrets = turrets.ToArray();
        //m_ProjectilePool = new GameObject[m_Turrets.Length][];
        //m_ProjectilePoolContainer = GameObject.Find("ProjectilePool");

        //for (int i = 0; i < m_Turrets.Length; i++)
        //{
        //    projectile = m_Turrets[i].GetComponent<TurretV2>().m_Projectile;
        //    if (projectile != null)
        //    {
        //        GameObject[] projectiles = new GameObject[m_iTurret2ProjectileTotal];

        //        for (int x = 0; x < m_iTurret2ProjectileTotal; x++)
        //        {
        //            projectiles[x] = Instantiate(projectile, new Vector3(0, 0, 0), Quaternion.identity, m_ProjectilePoolContainer.transform);
        //            projectiles[x].SetActive(false);
        //        }
        //        m_ProjectilePool[i] = projectiles;
        //    }
        //}
    }

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    static public GameObject[] getChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();

        List<GameObject> objList = new List<GameObject>();

        foreach (Transform t in ts)
        {
            if (t.gameObject.name == withName)
            {
                objList.Add(t.gameObject);
            } 
        }

        if (objList.Count > 0)
        {
            return objList.ToArray();
        }
            
        return null;
    }

    public float Wait(float duration)
    {
        duration = duration - Time.deltaTime;

        if (duration < 0)
            duration = 0;

        return duration;
    }


    public int malloc(GameObject obj, string type)
    {
        int index = 0;


        return index;
    }

    public int free(int index)
    {
        return 0;
    }

    public void PointerLoc(Vector3 location)
    {
        pointerToken.transform.position = location;
    }

    public static void SetSelectedShip(GameObject selected)
    {
        m_SelectedShip = selected;
    }

}


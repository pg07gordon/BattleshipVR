using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Projectile Turret
* Notes: Turret that fires self-contained gameObjects 
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class ProjectileTurret : TurretBase
{
    [Header("Fire Control")]
    public GameObject   m_Projectile;
    public float        m_fAutoDetonateRange = 0f; // Change to Internal
    public int          m_iMaxAmmo = 10;
    public float        m_iReloadRatePreSec = 1f;

    private GameObject[]    m_MyProjectilePool;
    private int             m_iProjectilePoolIndex = 0;

    // Use this for initialization
    new void Start ()
    {
        base.Start();

        if (m_Barrel != null)
        {
            m_vLocalBarrelStartPos = m_Barrel.transform.localPosition;

            if (m_fFireRatePerSecond < m_fBarrelRecoilTime)
            {
                m_fBarrelRecoilTime = m_fFireRatePerSecond - 0.1f;
            }
        }

        int maxProjectileLifeTime = (int)Mathf.Ceil(m_Projectile.GetComponent<Projectile>().m_fMaxLifeTime);
        int totalProjectitleCalc = (int)Mathf.Ceil(m_fFireRatePerSecond) * maxProjectileLifeTime;

        m_MyProjectilePool = new GameObject[totalProjectitleCalc];

        GameObject projectilePoolContainer = GameObject.Find("ResourcePool");

        for (int x = 0; x < totalProjectitleCalc; x++)
        {
            m_MyProjectilePool[x] = Instantiate(m_Projectile, new Vector3(0, 0, 0), Quaternion.identity, projectilePoolContainer.transform);
            m_MyProjectilePool[x].GetComponent<Projectile>().SetupExplosion();
            m_MyProjectilePool[x].SetActive(false);
        }

        StartCoroutine(AlterMuzzleFlare());

    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateTargetLoc();
        RotateTurretToTarget();

        m_fFireRateCountDownTimer = GameManager.Instance.Wait(m_fFireRateCountDownTimer);

        if (m_bWeaponsFree && m_fFireRateCountDownTimer <= 0)
        {
            m_vTargetDir = m_TargetPoint - m_Barrel.transform.position;

            if (Vector3.Angle(m_vTargetDir, m_Barrel.transform.forward) < m_fMinFireAngle)
            {
                fireCannon();
            }
        }
    }

    public void fireCannon()
    {
        m_fFireRateCountDownTimer = m_fFireRatePerSecond;

        if (m_Barrel != null)
        {
            StartCoroutine(BarrelRecoil());
        }

        float vol = Random.Range(m_fVolLowRange, m_fVolHighRange);

        int soundRand = Random.Range(0, m_ShootSounds.Length - 1);
        m_AudioSource.PlayOneShot(m_ShootSounds[soundRand], vol);

        if (m_iProjectilePoolIndex >= m_MyProjectilePool.Length)
        {
            m_iProjectilePoolIndex = 0;
        }

        m_MuzzleFlare[m_iBarrelIndex].SetActive(true);

        if (m_FlareLight != null)
        {
            m_FlareLight.SetActive(true);
        }

        Invoke("TurnOffMuzzleFlare", m_iMuzzleFlareTimeOut);

        m_MyProjectilePool[m_iProjectilePoolIndex].transform.position = m_BarrelExits[m_iBarrelIndex].transform.position;
        m_MyProjectilePool[m_iProjectilePoolIndex].transform.rotation = m_BarrelExits[m_iBarrelIndex].transform.rotation;
        m_MyProjectilePool[m_iProjectilePoolIndex].SetActive(true);
        m_MyProjectilePool[m_iProjectilePoolIndex].GetComponent<Projectile>().m_FiredFrom = gameObject;
        m_MyProjectilePool[m_iProjectilePoolIndex].GetComponent<Projectile>().m_fAutoDetonateRange = m_fAutoDetonateRange;
        m_MyProjectilePool[m_iProjectilePoolIndex].GetComponent<Projectile>().StartUp();
        m_iProjectilePoolIndex++;

        if (m_BarrelExits.Length > 1)
        {
            m_iBarrelIndex++;

            if (m_iBarrelIndex >= m_BarrelExits.Length)
            {
                m_iBarrelIndex = 0;
            }
        }
    }
}

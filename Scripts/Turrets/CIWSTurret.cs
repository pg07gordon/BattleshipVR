using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - CIWS Turret
* Notes: Turret that fires particle systems 
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class CIWSTurret : TurretBase
{
    public ParticleSystem m_FireParticles;
    public int m_iTimeBetweenFireBursts = 1;
    private ParticleSystem.EmissionModule m_FireParticalEm;

    // Use this for initialization
    new void Start ()
    {
        base.Start();
        m_FireParticalEm = m_FireParticles.emission;

        CIWSFire fireScript = m_FireParticles.GetComponent<CIWSFire>();

        fireScript.m_iRoundsPerSecond = Mathf.CeilToInt(m_fFireRatePerSecond);
        fireScript.SetupExplosions();

        StartCoroutine(AlterMuzzleFlare());
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateTargetLoc();
        RotateTurretToTarget();
        AlterMuzzleFlare();

        m_fFireRateCountDownTimer = GameManager.Instance.Wait(m_fFireRateCountDownTimer);

        if (m_fFireRateCountDownTimer <= m_fFireRateCountDownTimer * 0.5f)
        {
            m_FireParticalEm.rateOverTime = 0f;
            m_MuzzleFlare[m_iMuzzleFlareIndex].SetActive(false);

            if (m_FlareLight != null)
            {
                m_FlareLight.SetActive(false);
            }
        }

        if (m_bWeaponsFree && m_fFireRateCountDownTimer <= 0)
        {
            m_vTargetDir = m_TargetPoint - m_Barrel.transform.position;

            if (Vector3.Angle(m_vTargetDir, m_Barrel.transform.forward) < m_fMinFireAngle)
            {
                m_FireParticalEm.rateOverTime = m_fFireRatePerSecond;
                m_fFireRateCountDownTimer = m_iTimeBetweenFireBursts;
                m_MuzzleFlare[m_iMuzzleFlareIndex].SetActive(true);

                if (m_FlareLight != null)
                {
                    m_FlareLight.SetActive(true);
                }
            }
        }
    }
}
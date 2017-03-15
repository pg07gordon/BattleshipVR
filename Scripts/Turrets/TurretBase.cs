using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

/*
* VR Battleship - Turret Base Controller (Ver2)
* Notes: Base turret rotation and range limit controls
* By Gordon Niemann
* Build - Feb 8rd 2017
*/

public class TurretBase : MonoBehaviour
{
    public Targetable   m_Target; // TEMP
    protected Vector3   m_TargetPoint;

    public float        m_fUpdateTargetFirePointTimeMax = 10f;
    public float        m_fUpdateTargetFirePointTimeMin = 5f;
    protected float     m_fUpdateTargetFirePointTimer = 0f;
    protected int       m_iTargetPointNumb = 0;

    public bool         m_bWeaponsFree = true;
    
    [Header("Turret Settings")]
    public GameObject   m_TurretBase;
    public float        m_fTurretSpeedY = 1f;
    public Vector2      m_vYawLimits;
    public bool         m_bParallelOrientation = true;

    [Header("Barrel Settings")]
    public GameObject   m_BarrelBase;
    public GameObject   m_Barrel; // Used for Recoil Animation only (can be null)
    public float        m_fBarrelSpeedX = 1f;
    public Vector2      m_vPitchLimits;
    public float        m_fBarrelRecoilTime = 1.5f; // Lower = faster

    [Header("Fire Control")]
    public float        m_fMinFireAngle = 2f;
    public AudioClip[]  m_ShootSounds;
    public float        m_fFireRatePerSecond = 2f;
    protected float     m_fFireRateCountDownTimer = 0f;

    protected AudioSource   m_AudioSource;
    protected float         m_fVolLowRange = 0.5f;
    protected float         m_fVolHighRange = 1.0f;

    protected float         m_fTurretAngleY;
    protected float         m_fTargetAngleY;

    protected Vector3       m_vTargetPosX;
    protected Vector3       m_vTargetPosY;
    protected Vector3       m_vTargetDir;

    protected Quaternion    m_qTargetRotX;
    protected Quaternion    m_qTargetRotY;

    protected bool          m_bTurretLock = false;

    protected GameObject[]  m_BarrelExits;
    protected GameObject[]  m_MuzzleFlare;
    protected GameObject    m_FlareLight;

    protected Vector3       m_vLocalBarrelStartPos;
    protected Vector3[]     m_vLocalEulerMuzzleFlareRot;
    protected Vector3[]     m_vLocalMuzzleFlareSize;

    protected int   m_iBarrelIndex = 0;
    protected int   m_iMuzzleFlareIndex = 0;
    protected float m_iMuzzleFlareTimeOut = 0.15f;

    protected int   m_iTrackingCorrection;
    protected int   m_iRotationCorrection;

    // Use this for initialization
    protected void Start ()
    {
        m_AudioSource = GetComponent<AudioSource>();

        int correction = 1;

        if (m_bParallelOrientation)
        {
            correction = (int)transform.right.x;
        }
        else
        {
            correction = -(int)transform.right.z;
        }

        m_iTrackingCorrection = correction;
        m_iRotationCorrection = correction * (int)transform.up.y;

        m_BarrelExits =  GameManager.getChildGameObject(this.gameObject, "BarrelExit");
        m_MuzzleFlare = GameManager.getChildGameObject(this.gameObject, "MuzzleFlare");
        GameObject[] flareLight = GameManager.getChildGameObject(this.gameObject, "FlareLight");

        if (flareLight != null)
        {
            m_FlareLight = flareLight[0];
            m_FlareLight.SetActive(false);
        }

        m_vLocalEulerMuzzleFlareRot = new Vector3[m_MuzzleFlare.Length];
        m_vLocalMuzzleFlareSize = new Vector3[m_MuzzleFlare.Length];

        for (int i = 0; i < m_MuzzleFlare.Length; i++)
        {
            m_MuzzleFlare[i].SetActive(false);
            m_vLocalEulerMuzzleFlareRot[i] = m_MuzzleFlare[i].transform.localEulerAngles;
            m_vLocalMuzzleFlareSize[i] = m_MuzzleFlare[i].transform.localScale;
        }
    }

    protected void UpdateTargetLoc()
    {
        m_fUpdateTargetFirePointTimer -= Time.deltaTime;
        m_TargetPoint = m_Target.GetTargetPoint(m_iTargetPointNumb);

        // Needs to be updated
        //m_TargetPoint = PredictTargetFuturePos(m_TargetPoint, m_fFireRateCountDownTimer);

        if (m_fUpdateTargetFirePointTimer < 0)
        {
            m_fUpdateTargetFirePointTimer = Random.Range(m_fUpdateTargetFirePointTimeMin, m_fUpdateTargetFirePointTimeMax);
            m_iTargetPointNumb = Random.Range(0, m_Target.m_iAmountOfPoints);
        }
    }

    protected void RotateTurretToTarget()
    {
        // Local Turret Angle
        m_fTurretAngleY = (m_TurretBase.transform.localEulerAngles.y > 180) ? m_TurretBase.transform.localEulerAngles.y - 360 : m_TurretBase.transform.localEulerAngles.y;
        m_fTurretAngleY = m_fTurretAngleY * m_iTrackingCorrection;

        // Local Target Angle
        m_vTargetPosY = transform.InverseTransformPoint(m_TargetPoint);
        m_qTargetRotY.SetLookRotation(m_vTargetPosY, m_TurretBase.transform.up);
        m_fTargetAngleY = m_qTargetRotY.eulerAngles.y;
        if (m_fTargetAngleY > 180) m_fTargetAngleY -= 360;
        m_fTargetAngleY = m_fTargetAngleY * m_iTrackingCorrection;

        //print(transform.right + "  " + transform.up);
        //print(gameObject.name + " Turret: " + m_fTurretAngleY);
        //print(gameObject.name + " Target: " + m_fTargetAngleY);

        if (m_bTurretLock || (m_fTurretAngleY - 5f) > m_vYawLimits.y || (m_fTurretAngleY + 5f) < m_vYawLimits.x)
        {
            m_bTurretLock = true;

            RotateToY(0);

            if (m_fTurretAngleY > -10f && m_fTurretAngleY < 10f)
            {
                m_bTurretLock = false;
            }
        }
        else
        {
            if (m_fTargetAngleY > m_vYawLimits.y)
            {
                RotateToY(m_vYawLimits.y * m_iRotationCorrection);
            }
            else if (m_fTargetAngleY < m_vYawLimits.x)
            {
                RotateToY(m_vYawLimits.x * m_iRotationCorrection);
            }
            else
            {
                RotateToY(m_fTargetAngleY * m_iRotationCorrection);

                // Another Way of doing it
                //m_vTargetPosY = m_Target.transform.position;
                //m_vTargetPosY.y = m_BarrelBase.transform.position.y;
                //m_vTargetPosY = m_vTargetPosY - m_TurretBase.transform.position;
                //m_qTargetRotY = Quaternion.LookRotation(m_vTargetPosY, m_TurretBase.transform.up);
                //m_TurretBase.transform.rotation = Quaternion.Slerp(m_TurretBase.transform.rotation, m_qTargetRotY, m_fTurretSpeedY * Time.deltaTime);
            }
        }

        m_TurretBase.transform.localEulerAngles = new Vector3(0f, m_TurretBase.transform.localEulerAngles.y, 0f);

        m_vTargetPosX = m_TargetPoint - m_BarrelBase.transform.position;
        m_qTargetRotX = Quaternion.LookRotation(m_vTargetPosX, m_TurretBase.transform.up);

        m_BarrelBase.transform.rotation = Quaternion.Slerp(m_BarrelBase.transform.rotation, m_qTargetRotX, m_fBarrelSpeedX * Time.deltaTime);
        m_BarrelBase.transform.localEulerAngles = new Vector3(m_BarrelBase.transform.localEulerAngles.x, 0f, 0f);

        // Limits Barrel Movement Range
        if (m_BarrelBase.transform.localEulerAngles.x >= 180f && m_BarrelBase.transform.localEulerAngles.x < (360f - m_vPitchLimits.y))
        {
            m_BarrelBase.transform.localEulerAngles = new Vector3(360f - m_vPitchLimits.y, 0f, 0f);
        }
        else if (m_BarrelBase.transform.localEulerAngles.x < 180f && m_BarrelBase.transform.localEulerAngles.x > -m_vPitchLimits.x)
        {
            m_BarrelBase.transform.localEulerAngles = new Vector3(-m_vPitchLimits.x, 0f, 0f);
        }

        //Debug.DrawRay(m_BarrelBase.transform.position, m_BarrelBase.transform.forward * 5, Color.red); // TEMP
    }

    protected void RotateToY(float angle)
    {
        m_vTargetPosY = Vector3.RotateTowards(m_BarrelBase.transform.forward, Quaternion.Euler(0, angle, 0) * transform.forward, Time.deltaTime * m_fTurretSpeedY, 0);
        m_TurretBase.transform.rotation = Quaternion.LookRotation(m_vTargetPosY, transform.up);
    }

    protected void TurnOffMuzzleFlare()
    {
        m_MuzzleFlare[m_iMuzzleFlareIndex].transform.localEulerAngles = m_vLocalEulerMuzzleFlareRot[m_iMuzzleFlareIndex];
        m_MuzzleFlare[m_iMuzzleFlareIndex].transform.localScale = m_vLocalMuzzleFlareSize[m_iMuzzleFlareIndex];
        m_MuzzleFlare[m_iMuzzleFlareIndex].SetActive(false);
        m_iMuzzleFlareIndex++;

        if (m_FlareLight != null)
        {
            m_FlareLight.SetActive(false);
        }
            
        if (m_iMuzzleFlareIndex >= m_MuzzleFlare.Length)
        {
            m_iMuzzleFlareIndex = 0;
        }
    }

    protected IEnumerator BarrelRecoil()
    {
        float recoilDist = Vector3.Distance(m_BarrelExits[m_iBarrelIndex].transform.position, m_BarrelBase.transform.position) * 0.2f;
        float recoilTime = m_fBarrelRecoilTime * 0.4f;
        float returnTime = m_fBarrelRecoilTime * 0.6f;
        float step;

        Vector3 recoilToPos = -m_Barrel.transform.worldToLocalMatrix.MultiplyVector(m_Barrel.transform.forward) * recoilDist;

        while (Application.isPlaying && recoilTime > 0)
        {
            step =+ Time.deltaTime * 10; 
            m_Barrel.transform.localPosition = Vector3.Lerp(m_Barrel.transform.localPosition, recoilToPos, step);
            recoilTime -= Time.deltaTime;
            yield return null;
        }

        while (Application.isPlaying && returnTime > 0)
        {
            step = +Time.deltaTime * 4;
            m_Barrel.transform.localPosition = Vector3.Lerp(m_Barrel.transform.localPosition, m_vLocalBarrelStartPos, step);
            returnTime -= Time.deltaTime;
            yield return null;
        }

        m_Barrel.transform.localPosition = m_vLocalBarrelStartPos;
        yield return null;
    }

    protected Vector3 PredictTargetFuturePos(Vector3 targetPointPos, float sec) // Needs to be updated
    {
        // return m_Target.transform.position + m_Target.transform.forward + m_Target.m_MyVelocity * sec;
        return targetPointPos + m_Target.transform.forward * sec;
    }

    protected IEnumerator AlterMuzzleFlare()
    {
        while (Application.isPlaying)
        {
            Vector3 eulerRot = m_vLocalEulerMuzzleFlareRot[m_iMuzzleFlareIndex];

            float x = m_vLocalMuzzleFlareSize[m_iMuzzleFlareIndex].x;
            float y = m_vLocalMuzzleFlareSize[m_iMuzzleFlareIndex].y;
            float z = m_vLocalMuzzleFlareSize[m_iMuzzleFlareIndex].z;

            m_MuzzleFlare[m_iMuzzleFlareIndex].transform.localEulerAngles = new Vector3(eulerRot.x, eulerRot.y, Random.Range(0, 360));
            m_MuzzleFlare[m_iMuzzleFlareIndex].transform.localScale = new Vector3(Random.Range(x * 0.6f, x * 1.4f), Random.Range(y * 0.6f, y * 1.4f), Random.Range(z * 0.6f, z * 1.4f));

            yield return new WaitForSeconds(0.1f);
        }
    }
}

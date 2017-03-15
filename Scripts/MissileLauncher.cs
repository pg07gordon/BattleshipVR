using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - MissleLauncher Script
* Notes: Missile Launce Animations, effects and timing
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class MissileLauncher : MonoBehaviour
{
    public GameObject m_Target;

    public EjectionCap  m_Cap;
    private Vector3     m_CapStartLoc;
    private Quaternion  m_CapStartRot;
    public GameObject   m_missile;
    public GameObject   m_LaunchFlareLight;
    public GameObject   m_CapEjectionParticles;
    public GameObject   m_CapEjectionFlash;
    public GameObject   m_LaunchBastParticles;
    public GameObject   m_LaunchSmokeParticles;
    internal bool       m_bReadyToLaunch = true;
    private float       m_fMainTimer;

    private void Awake()
    {
        // Needs to be updated
        transform.parent.parent.GetComponent<Starship>().GetComponent<MissileController>().AddMissileLauncher(this);
    }

    private void Start ()
    {
        m_CapStartLoc = m_Cap.transform.localPosition;
        m_CapStartRot = m_Cap.transform.localRotation;
    }

    private void FixedUpdate()
    {
        m_fMainTimer += Time.deltaTime;
    }

    public void Launch()
    {
        if (m_bReadyToLaunch)
        {
            m_bReadyToLaunch = false;
            m_fMainTimer = 0;
            m_Cap.Eject();
            m_CapEjectionParticles.SetActive(true);
            m_CapEjectionFlash.SetActive(true);
            StartCoroutine(FireInTheHole());
            StartCoroutine(BastOff());
        }
    }

    protected IEnumerator FireInTheHole()
    {
        while (m_fMainTimer < 1f)
        {
            yield return null;
        }
        m_LaunchBastParticles.SetActive(true);
        m_LaunchFlareLight.SetActive(true);
    }

    protected IEnumerator BastOff()
    {
        while (m_fMainTimer < 1.5f)
        {
            yield return null;
        }

        m_LaunchSmokeParticles.SetActive(true);

        //TEMP
        GameObject missile = Instantiate(m_missile, transform.position, Quaternion.identity);
        missile.transform.rotation = transform.rotation;
        missile.GetComponent<Missile>().m_Target = m_Target;

        StartCoroutine(Reset());
    }

    protected IEnumerator Reset()
    {
        yield return new WaitForSeconds(2f);
        m_LaunchFlareLight.SetActive(false);

        yield return new WaitForSeconds(10f);

        Material myMaterial = m_Cap.GetComponent<Renderer>().material;
        Color currentColor = myMaterial.color;
        Color transparent = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);

        m_fMainTimer = 0;
        while (m_fMainTimer < 3f)
        {
            myMaterial.color = Color.Lerp(myMaterial.color, transparent, Time.deltaTime);
            yield return null;
        }

        m_Cap.GetComponent<Rigidbody>().isKinematic = true;
        
        m_Cap.transform.localPosition = m_CapStartLoc;
        m_Cap.transform.localRotation = m_CapStartRot;

        m_fMainTimer = 0;
        while (m_fMainTimer < 3f)
        {
            myMaterial.color = Color.Lerp(myMaterial.color, currentColor, Time.deltaTime);
            yield return null; 
        }

        m_Cap.GetComponent<Rigidbody>().isKinematic = false;
        m_CapEjectionParticles.SetActive(false);
        m_CapEjectionFlash.SetActive(false);
        m_LaunchSmokeParticles.SetActive(false);
        m_LaunchBastParticles.SetActive(false);
        m_LaunchFlareLight.SetActive(false);

        m_bReadyToLaunch = true;
    }
}

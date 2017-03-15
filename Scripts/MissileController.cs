using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Missile Controller
* Notes: Controls how Often grouped missle launchers (m_SetupMissileLaunchers) can fire. Also keeps controls fire order
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class MissileController : MonoBehaviour {

    private List<MissileLauncher>   m_SetupMissileLaunchers;
    private bool                    m_bSetupComplete = false;

    public float                    m_fLaunchCooldownTime = 10f;
    private float                   m_fLaunchTimer = 0f;

    internal MissileLauncher[]      m_MissileLaunchers;
    internal int                    LauncherIndex = 0;

    private void Start()
    {
        m_MissileLaunchers = m_SetupMissileLaunchers.ToArray();
        m_bSetupComplete = true;
    }

    public void AddMissileLauncher(MissileLauncher missileLauncher)
    {
        if (!m_bSetupComplete)
        {
            if (m_SetupMissileLaunchers == null)
            {
                m_SetupMissileLaunchers = new List<MissileLauncher>();
            }

            m_SetupMissileLaunchers.Add(missileLauncher);
        }
    }

    public bool RequsetLaunch()
    {
        if (m_fLaunchTimer <= 0f)
        {
            if (m_MissileLaunchers[LauncherIndex].m_bReadyToLaunch)
            {
                m_MissileLaunchers[LauncherIndex].Launch();
                LauncherIndex++;

                if (LauncherIndex >= m_MissileLaunchers.Length)
                {
                    LauncherIndex = 0;
                }

                StopAllCoroutines();
                StartCoroutine(LaunchCountdown());
                return true;
            }
        }

        return false;
    }

    private IEnumerator LaunchCountdown()
    {
        m_fLaunchTimer = m_fLaunchCooldownTime;

        while (m_fLaunchTimer > 0)
        {
            m_fLaunchTimer -= Time.deltaTime;
            yield return null;
        }
    }
}

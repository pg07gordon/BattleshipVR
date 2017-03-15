using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Detonate Timer
* Notes: Simple lifetime script
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class Detonate : MonoBehaviour
{
    public float m_fMaxLifeTime = 3f;
    private float m_fLifeCountDown = 0f;

    private void FixedUpdate()
    {
        m_fLifeCountDown = Wait(m_fLifeCountDown);

        if (m_fLifeCountDown <= 0)
        {
            ShutDown();
        }
    }

    // Simple Countdown Timer
    protected float Wait(float duration)
    {
        duration = duration - Time.deltaTime;

        if (duration < 0)
            duration = 0;

        return duration;
    }

    public void StartUp()
    {
        m_fLifeCountDown = m_fMaxLifeTime;
    }

    public void ShutDown()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - CISW (Close-in weapon system) Anti Fighter and Missile
* Notes: Particle System based projectiles 
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class CIWSFire : MonoBehaviour
{
    public GameObject m_ExplosionPrefabs;
    private GameObject[] m_MyExplosionPool;

    internal int m_iRoundsPerSecond = 0;
    internal int m_iExplosionIndex = 0;

    private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
        //GetComponent<ParticleSystem>().GetParticles(particles);

        List<ParticleCollisionEvent> collEvent = new List<ParticleCollisionEvent>();

        int events = GetComponent<ParticleSystem>().GetCollisionEvents(other, collEvent);

        for (int i = 0; i < collEvent.Count; i++)
        {
            //Color c = i == 0 ? Color.blue : Color.green;
            //Debug.DrawRay(collEvent[i].intersection, Vector3.up * 5f, c, 2f);

            if (m_iExplosionIndex >= m_MyExplosionPool.Length)
            {
                m_iExplosionIndex = 0;
            }

            m_MyExplosionPool[m_iExplosionIndex].transform.position = collEvent[i].intersection;
            m_MyExplosionPool[m_iExplosionIndex].SetActive(true);
            m_MyExplosionPool[m_iExplosionIndex].GetComponent<Detonate>().StartUp();

            m_iExplosionIndex++;

        }

        float distanceToCollPoint = Vector3.Distance(collEvent[collEvent.Count - 1].intersection, transform.position);

        for (int i = 0; i < particles.Length; i++)
        {
            if (Vector3.Distance(transform.TransformPoint(particles[i].position), transform.position) > distanceToCollPoint)
                particles[i].remainingLifetime = 0f;
        }

        GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
    }

    public void SetupExplosions()
    {
        int maxExplosionLifeTime = (int)Mathf.Ceil(m_ExplosionPrefabs.GetComponent<Detonate>().m_fMaxLifeTime);
        int totalExplosionCalc = (int)Mathf.Ceil(m_iRoundsPerSecond) * maxExplosionLifeTime;

        m_MyExplosionPool = new GameObject[totalExplosionCalc];

        GameObject explosionPoolContainer = GameObject.Find("ResourcePool");

        for (int x = 0; x < totalExplosionCalc; x++)
        {
            m_MyExplosionPool[x] = Instantiate(m_ExplosionPrefabs, new Vector3(0, 0, 0), Quaternion.identity, explosionPoolContainer.transform);
            m_MyExplosionPool[x].SetActive(false);
        }
    }
}

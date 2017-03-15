using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Projectile Script
* Notes: Movement, Collision and Explosion setup and detonation
* By Gordon Niemann
* Build - Jan 29th 2017
*/

public class Projectile : MonoBehaviour
{
    public GameObject[] m_ExplosionPrefabs;
    private GameObject m_MyExplosionPrefab;

    public LayerMask    m_RayCastHitLayerMask;
    public float        m_fRaycastLength = 2f;
    public float        m_fMaxLifeTime = 10f;
    public float        m_fVelocity = 100f;
    
    protected float     m_fLifeTimer = 0f;
    internal GameObject m_FiredFrom;
    internal float      m_fAutoDetonateRange = 0f;
    internal bool       m_bHit = false;
    private Vector3     m_vStep;

    private RaycastHit  m_RayCastHitLoc;

    private void Start()
    {
        m_RayCastHitLoc = new RaycastHit();
    }

    private void FixedUpdate()
    {
        m_fLifeTimer = GameManager.Instance.Wait(m_fLifeTimer);
        m_vStep = transform.forward * Time.deltaTime * m_fVelocity;
        transform.position += m_vStep;

        if (!m_bHit && m_fAutoDetonateRange > 0 && Vector3.Distance(transform.position, m_FiredFrom.transform.position) > m_fAutoDetonateRange)
        {
            Detonate(transform.position);
        }

        if (m_fLifeTimer <= 0 || m_bHit)
        {
            ShutDown();
        }

        if (Physics.Raycast(transform.position, transform.forward, out m_RayCastHitLoc, m_vStep.magnitude * m_fRaycastLength, m_RayCastHitLayerMask))
        {
            //print(m_RayCastHitLoc.collider.gameObject.name);
            Detonate(m_RayCastHitLoc.point + m_RayCastHitLoc.normal * -0.2f);
        }
    }

    public void StartUp()
    {
        m_fLifeTimer = m_fMaxLifeTime;
        m_bHit = false;
    }

    public void ShutDown()
    {
        gameObject.SetActive(false);
    }

    public void SetupExplosion()
    {
        int index = Random.Range(0, m_ExplosionPrefabs.Length);
        GameObject projectilePoolContainer = GameObject.Find("ResourcePool");
        m_MyExplosionPrefab = Instantiate(m_ExplosionPrefabs[index], transform.position, Quaternion.identity, projectilePoolContainer.transform);
        m_MyExplosionPrefab.SetActive(false);
    }

    public void Detonate(Vector3 detonateLoc)
    {
        float x = Random.Range(0f, 0.5f) + detonateLoc.x;
        float y = Random.Range(0f, 0.5f) + detonateLoc.y;
        float z = Random.Range(0f, 0.5f) + detonateLoc.z;

        m_MyExplosionPrefab.transform.position = new Vector3(x, y, z);
        m_MyExplosionPrefab.SetActive(true);
        m_MyExplosionPrefab.GetComponent<Detonate>().StartUp();
        m_bHit = true;
    }
}

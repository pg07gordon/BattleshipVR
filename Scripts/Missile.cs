using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Missiles
* Notes: Missle Guidance and movement
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class Missile : MonoBehaviour
{
    // Needs updating
    internal GameObject m_Target;
    public Targetable   m_MyOwner;
    public float m_fVelocity = 30f;

    private Vector3     m_vStep;
    private float       m_fMainTimer = 0;
    private bool        m_friendlyDetected = false;

    // Coroutine State Controllers
    protected delegate IEnumerator StateMethod();
    protected delegate IEnumerator SecondState(StateMethod nextState);
    protected StateMethod   m_CurrentState;
    protected SecondState   m_NextState;
    
    public LayerMask    m_RayCastHitLayerMask;
    private RaycastHit  m_RayCastHitLoc;
    internal bool       m_bHit = false;

    public float    m_fRaycastDetonateLength = 2f;
    public float    m_fRaycastScanLength = 300f;

    // TEMP
    public GameObject m_Explosion;

    /// <summary>
    /// Starts a new coroutine and stops all previous ones | Important: "this" on IEnumerator defined in children
    /// </summary>
    /// <param name="newState">One Coroutine to rule them all</param>
    protected void SetState(StateMethod newState)
    {
        StateDefaults();
        m_CurrentState = newState;
        StartCoroutine(m_CurrentState());
    }
    /// <param name="nextState">The next Coroutine in the stack to be run</param>
    protected void SetState(SecondState newState, StateMethod nextState)
    {
        StateDefaults();
        m_NextState = newState;
        StartCoroutine(m_NextState(nextState));
    }

    protected void StateDefaults()
    {
        StopAllCoroutines();
    }

    // Use this for initialization
    void Start ()
    {
        //m_RayCastHitLoc = new RaycastHit();
        SetState(OnStart);
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        m_fMainTimer += Time.deltaTime;

        m_vStep = transform.forward * Time.deltaTime * m_fVelocity;
        transform.position += m_vStep;

        if (Physics.Raycast(transform.position, transform.forward, out m_RayCastHitLoc, m_vStep.magnitude * m_fRaycastDetonateLength, m_RayCastHitLayerMask))
        {
            GameObject explosion = Instantiate(m_Explosion, m_RayCastHitLoc.point + m_RayCastHitLoc.normal * -0.2f, Quaternion.identity);
            Destroy(gameObject);
        }

        if (Physics.Raycast(transform.position, transform.forward, out m_RayCastHitLoc, m_vStep.magnitude * m_fRaycastScanLength, m_RayCastHitLayerMask))
        {
            if (m_RayCastHitLoc.collider.gameObject.tag == "friendly" && m_friendlyDetected == false)
            {
                m_friendlyDetected = true;
                print("ALERT: Friendly Detected");
                SetState(FlyUpwards);
            }
        }

        //Debug.DrawRay(transform.position, transform.forward * m_vStep.magnitude * m_fRaycastScanLength, Color.red); // TEMP
    }

    protected IEnumerator OnStart()
    {
        yield return new WaitForSeconds(Random.Range(1f, 1.5f));

        if (Vector3.Angle(transform.forward, m_Target.transform.position) > 100)
        {
            SetState(FlyUpwards);
        }
        else
        {
            SetState(FlyToTarget);
        }
    }

    protected IEnumerator FlyUpwards()
    {
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(GameManager.m_MapOrientationUp.transform.position - transform.position), Time.deltaTime);
            yield return null;
        }
        m_friendlyDetected = false;
        SetState(FlyToTarget);

    }

    protected IEnumerator FlyToTarget()
    {
        while (Application.isPlaying)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_Target.transform.position - transform.position), Time.deltaTime);
            yield return null;
        }
    }
}

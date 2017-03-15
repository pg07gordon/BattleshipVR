using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* VR Battleship - Targetable Object (Ver2)
* Notes: Target "points" setup.
* By Gordon Niemann
* Build - Feb 4rd 2017
*/

public class Targetable : MonoBehaviour
{
    private Point[]     m_MyTargetPoints;
    public int          m_iAmountOfPoints = 50;
    internal float      m_MyVelocity = 0f;
    protected Vector3   m_MyLastPos;

    // Use this for initialization
    protected void Start ()
    {
        m_MyLastPos = transform.position;

        MeshFilter mF = GetComponent<MeshFilter>();
        m_MyTargetPoints = new Point[m_iAmountOfPoints];

        for (int i = 0; i < m_iAmountOfPoints; i++)
        {
            Vector3 point = mF.mesh.GetRandomPointInsideConvex();
            m_MyTargetPoints[i] = new Point(point);
        }
    }

    public Vector3 GetTargetPoint(int pointNumb)
    {
        return transform.TransformPoint(m_MyTargetPoints[pointNumb].pos);
    }

    struct Point
    {
        public Point(Vector3 pos)
        {
            this.pos = pos;
        }
        public Vector3 pos;
    }

    protected void UpdateMyVelocity()
    {
        m_MyVelocity = ((transform.position - m_MyLastPos).magnitude) / Time.deltaTime;
        m_MyLastPos = transform.position;
    }

    void OnDrawGizmos()
    {
        if (m_MyTargetPoints == null || m_MyTargetPoints.Length == 0)
            return;

        foreach (Point p in m_MyTargetPoints)
        {
            Gizmos.color = Color.red; // The_Helper.InterpolateColor(Color.red, Color.green, p.pos.magnitude); 
            Gizmos.DrawSphere(transform.TransformPoint(p.pos), transform.lossyScale.magnitude / 100);
        }
    }
}

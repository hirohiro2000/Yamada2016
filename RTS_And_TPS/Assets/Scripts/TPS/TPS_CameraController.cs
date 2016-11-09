using UnityEngine;
using System.Collections;

public class TPS_CameraController : MonoBehaviour
{
    private Transform m_target       = null;
    private float     m_cameraHeight = 2.424826f;

    private Vector3   m_idenLocalPosition   = Vector3.zero;
    private Vector3   m_camPosition         = Vector3.zero;
    private Vector3   m_camLookAt           = Vector3.zero;
    private Vector3   m_camTarget           = Vector3.zero;

    //
    void Start()
    {
        m_target = transform.parent;

        m_idenLocalPosition = transform.localPosition;
        m_camPosition       = transform.position;

        Vector3 targetPosition = m_target.transform.position + new Vector3(0.0f, m_cameraHeight, 0.0f);

        m_camLookAt = targetPosition;
        m_camTarget = targetPosition;


    }

    void LateUpdate()
    {
        if ( m_target == null )
        {
            Destroy(gameObject);
            return;
        }

        Vector3 position = m_camPosition;
        Vector3 lookAt   = m_camLookAt;


        {
            Vector3 targetPosition = m_target.transform.position + new Vector3(0.0f, m_cameraHeight, 0.0f);
            lookAt = Vector3.Lerp( lookAt, targetPosition, 0.1f );
        }


		{
            transform.localPosition = m_idenLocalPosition;
            position = Vector3.Slerp( position, transform.position, 0.15f );
        }

        m_camPosition = position;
        m_camLookAt = lookAt;
        transform.position = m_camPosition;
        transform.LookAt( m_camLookAt );
        
    }

    Vector3 ToVector(float horizontal, float vertical, float length)
    {
        float sinV = Mathf.Sin(vertical);
        float cosV = Mathf.Cos(vertical);
        float sinH = Mathf.Sin(horizontal);
        float cosH = Mathf.Cos(horizontal);

        Vector3 o;
        o.x = cosV * sinH * length;
        o.y = sinV * length;
        o.z = cosV * cosH * length;
        return o;
    }
    Vector3 ToPolar(Vector3 vec)
    {
        float length = vec.magnitude;
        float angleHorizontal = Mathf.Atan2(vec.x, vec.z);
        float angleVertical   = Mathf.Atan2(vec.y, Mathf.Sqrt(vec.x * vec.x + vec.z * vec.z));

        return new Vector3(angleHorizontal, angleVertical, length);

    }

}


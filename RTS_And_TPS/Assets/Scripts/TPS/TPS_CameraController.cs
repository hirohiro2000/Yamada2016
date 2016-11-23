using UnityEngine;
using System.Collections;

public class TPS_CameraController : MonoBehaviour
{
    [SerializeField]
    private float     m_maxVerticalDeg  = 80.0f;

    [SerializeField]
    private float     m_minVerticalDeg  = -80.0f;

    [SerializeField, Range(0.01f, 1.0f)]
    private float     m_sensitivity     = 1.0f;

    private Transform m_target       = null;
    private float     m_cameraHeight = 2.424826f;

    private Vector3   m_idenLocalPosition   = Vector3.zero;
    private Vector3   m_camPosition         = Vector3.zero;
    private Vector3   m_camLookAt           = Vector3.zero;
    private Vector3   m_camTarget           = Vector3.zero;

    private Vector3   m_camPolar            = Vector3.zero;

    private bool      m_isInvert            = false;

    private Vector2   m_shake               = Vector2.zero;

	[SerializeField, Range(0.005f, 0.1f)]
	private float m_rotateSpeed = 0.04f;

	//
	void Start()
    {
        m_target = transform.parent;

        m_idenLocalPosition = transform.localPosition;
        m_camPosition       = transform.position;

        Vector3 targetPosition = m_target.transform.position + new Vector3(0.0f, m_cameraHeight, 0.0f);

        m_camPolar = ToPolar( ( transform.position - targetPosition ) );

        m_camLookAt = targetPosition;
        m_camTarget = targetPosition;

        transform.parent = null;

    }

    void Update()
    {        
        if ( m_target == null )
        {
            Destroy(gameObject);
            return;
        }

        Vector3 position = transform.position;
        Vector3 lookAt   = m_camLookAt;


        {
            Vector3 targetPosition = m_target.transform.position + new Vector3(0.0f, m_cameraHeight, 0.0f);
            lookAt = Vector3.Lerp( lookAt, targetPosition, 0.1f );
        }


		{
			float inputH = Input.GetAxis("Mouse X") * m_rotateSpeed;
			float inputV = Input.GetAxis("Mouse Y") * m_rotateSpeed;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                // 回転を行わない
            }
            else
            {
                if (m_isInvert == false)
                {
                    inputV = -inputV;
                }
                m_camPolar.x += inputH;
                m_camPolar.y = Mathf.Clamp( m_camPolar.y + inputV, m_minVerticalDeg*Mathf.Deg2Rad, m_maxVerticalDeg*Mathf.Deg2Rad );
            }

            Vector3 p = lookAt + ToVector( m_camPolar.x + m_shake.x, m_camPolar.y + m_shake.y, m_camPolar.z );

            Vector3     dir         = ( p - m_camLookAt );
            RaycastHit  rHit        = new RaycastHit();
            Ray         rRay        = new Ray( m_camLookAt, dir.normalized );
            float       maxDist     = dir.magnitude;
            
            //  レイ判定
            if( Physics.Raycast( rRay, out rHit, maxDist ) )
            {
                p = rHit.point;
            }

            position = Vector3.Slerp( position, p, m_sensitivity );


        }

        m_camLookAt = lookAt;
        transform.position = position;
        transform.LookAt( m_camLookAt );
        
        m_shake = Vector2.zero;

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
    
    public void Shake( Vector2 value )
    {
        m_shake += value;
    }

    public Vector3 GetPolar() { return m_camPolar; }

}


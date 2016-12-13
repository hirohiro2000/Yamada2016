using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour 
{
	public  Transform	m_target = null;
    public  Vector3     m_dir;

    [SerializeField, Range(5, 100)]
    public  float       m_targetDistance = 20.0f;

    public  bool        m_isForcus      = true;

    private Vector3     m_camLookAt     = Vector3.zero;

    public enum ActionState
    {
        eNone,
        eMoveHorizontal,
        eMoveVertical,
        eNumValues,
    }
	public ActionState			m_actionState	= ActionState.eNone;

    
	// Use this for initialization
	void Start ()
	{
	}

    // Update is called once per frame
    void Update()
    {
		if( m_target == null )
		{
			transform.position = Vector3.zero + m_dir*m_targetDistance;
			transform.LookAt( Vector3.zero );
			return;
		}

        if ( m_isForcus == true && m_actionState != ActionState.eMoveHorizontal )
        {
            ForcusOnPlayer();
        }

        switch (m_actionState)
        {
        case ActionState.eMoveHorizontal:   MoveHorizontal( ( m_isForcus ) ? 10.0f : float.MaxValue); break;
        case ActionState.eMoveVertical:     MoveVertical();     break;
        default: break;
        }

        Vector3 dir = m_dir.normalized * m_targetDistance;
        transform.position = m_camLookAt + dir;
        transform.LookAt( transform.position - m_dir );
        
    }


    void MoveHorizontal( float limitDistance )
    {
        Vector2 mouseAxis = new Vector2( Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") );
        Vector3 move = ( transform.right*mouseAxis.x ) + ( transform.up*mouseAxis.y );
        move.y = 0.0f;
        m_camLookAt += move;

        Vector3 target = m_target.position - m_camLookAt;

        Vector3 sub = m_target.position - m_camLookAt;
        sub.y = 0.0f;
        if ( sub.sqrMagnitude > limitDistance*limitDistance )
        {
            m_camLookAt   = m_target.position - sub.normalized * limitDistance;
            m_camLookAt.y = m_target.position.y;
        }

    }
    void MoveVertical()
    {
        m_targetDistance += Input.GetAxis("Mouse Y");
    }
    void ForcusOnPlayer()
    {
        m_camLookAt = m_target.position;
    }


//  矩形によるカメラ移動
//    void MoveHorizontal( float limitDistance )
//    {
//        Vector2 mousePosition = Input.mousePosition;
//        Vector2 centerPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
//        Vector2 subCenterToMouse = (mousePosition - centerPosition);
//
//        // horizontal move
//        float   maxSpeed    = 1.0f;
//        int     pow         = 3;
//        Vector3 move        = Vector3.zero;
//
//        float marginVertical   = Screen.height * 0.3f;
//        float marginHorizontal = Screen.width  * 0.3f;
//        
//        if (subCenterToMouse.x > marginHorizontal)  move.x += Margin01( subCenterToMouse.x, marginHorizontal,  Screen.width*0.5f   );
//        if (subCenterToMouse.x < -marginHorizontal) move.x -= Margin01( subCenterToMouse.x, -marginHorizontal, -Screen.width*0.5f  );
//        if (subCenterToMouse.y > marginVertical)    move.y += Margin01( subCenterToMouse.y, marginVertical,    Screen.height*0.5f  );
//        if (subCenterToMouse.y < -marginVertical)   move.y -= Margin01( subCenterToMouse.y, -marginVertical,   -Screen.height*0.5f );
//        
//        float speed = Mathf.Min( Mathf.Pow(move.magnitude, pow ), maxSpeed );
//
//        move = ( ( transform.right*move.x )+( transform.up*move.y ) ).normalized * speed;
//        move.y = 0.0f;
//
//        m_camLookAt += move;
//
//        Vector3 target = m_target.position - m_camLookAt;
//
//        Vector3 sub = m_target.position - m_camLookAt;
//        sub.y = 0.0f;
//        if ( sub.sqrMagnitude > limitDistance*limitDistance )
//        {
//            m_camLookAt   = m_target.position - sub.normalized * limitDistance;
//            m_camLookAt.y = m_target.position.y;
//        }
//
//    }
//    // 計算
//    float Margin01( float val, float start, float goal )
//    {
//        return ( (val-start) / (goal-start) );
//    }
    

}



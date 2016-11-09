using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public  float       c_AnglePower        =   1.0f;
    public  float       c_Distance          =   5.0f;
    public  float       c_VerticalDeadAngle =   10.0f;

    private GameObject  m_rPlayer           =   null;
    private Rigidbody   m_rRigidbody        =   null;

    private Vector3     m_TargetCenter      =   new Vector3( 0.0f, 0.0f, 0.0f );
    private Vector3     m_CurCenter         =   new Vector3( 0.0f, 0.0f, 0.0f );

    private float       m_Distance          =   0.0f;
    private float       m_CurDisntace       =   0.0f;

	// Use this for initialization
	void    Start()
    {
	    m_rPlayer       =   GameObject.Find( "Player" );
        m_rRigidbody    =   GetComponent< Rigidbody >();

        m_TargetCenter  =   m_rPlayer.transform.position;
        m_CurCenter     =   m_TargetCenter;

        m_Distance      =   c_Distance;
        m_CurDisntace   =   c_Distance;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  向きの更新
        {
            Transform   rCamTrans       =   Camera.main.transform;
            float       angularPower    =   c_AnglePower * Time.deltaTime;

            //  パッド
            float   axisX  =   Input.GetAxis( "CamAxisX" );
            float   axisY  =   Input.GetAxis( "CamAxisY" );

            //  マウス
            //axisX   +=   Input.GetAxis( "Mouse X" ) * 0.75f;
            //axisY   +=  -Input.GetAxis( "Mouse Y" ) * 0.75f;

            //  キー
            if( Input.GetKey( KeyCode.L ) ) axisX   =    1.0f;
            if( Input.GetKey( KeyCode.J ) ) axisX   =   -1.0f;
            if( Input.GetKey( KeyCode.I ) ) axisY   =   -1.0f;
            if( Input.GetKey( KeyCode.K ) ) axisY   =    1.0f;

            //  入力を正規化
            {
                Vector2 inputAxis   =   new Vector2( axisX, axisY );
                        inputAxis   =   inputAxis.normalized;

                axisX   =   inputAxis.x;
                axisY   =   inputAxis.y;
            }

            //  反映
            m_rRigidbody.AddTorque( Vector3.up.normalized      * angularPower * axisX );
            m_rRigidbody.AddTorque( rCamTrans.right.normalized * angularPower * axisY );

            //  マウス
            //float   mAxisX  =   Input.GetAxis( "Mouse X" );
            //float   mAxisY  =   Input.GetAxis( "Mouse Y" );
            //m_rRigidbody.AddTorque( Vector3.up.normalized * angularPower * mAxisX );
            //m_rRigidbody.AddTorque( rCamTrans.right.normalized * angularPower * -mAxisY );
        }

        //if( Input.GetMouseButtonDown( 2 )
        //||  Input.GetKeyDown( KeyCode.U ) ){
        //    transform.rotation  =   m_rPlayer.transform.rotation;
        //    transform.Rotate( new Vector3( -80.0f, 0.0f, 0.0f ) );
        //}

        //  １人称視点
        //if( Input.GetButton( "Aim" ) ){
        //    //  姿勢の同期
        //    transform.rotation  =   m_rPlayer.transform.rotation;
        //
        //    //  現在の座標更新
        //    transform.position  =   m_CurCenter + m_rPlayer.transform.forward.normalized * 1.0f;
        //}
        ////  通常視点
        //else
        {
            //  斜めを向かないように補正
            Quaternion  dQuat   =   new Quaternion();
            dQuat.SetLookRotation( transform.forward, Vector3.up );
            transform.rotation  =   dQuat;

            //  現在の座標更新
            transform.position  =   m_CurCenter - transform.forward.normalized * c_Distance;
        }

        //  壁チェック
        {
            RaycastHit  hitInfo;
            if( Physics.Raycast( m_CurCenter, -transform.forward.normalized, out hitInfo, c_Distance, 1 ) ){
                m_Distance  =   hitInfo.distance * 0.99f;
            }
            else{
                m_Distance  =   c_Distance;
            }

            m_CurDisntace       +=  ( m_Distance - m_CurDisntace ) * 1.0f;
            transform.position  =   m_CurCenter - transform.forward.normalized * m_CurDisntace;
        }
	}
    void    FixedUpdate()
    {
        //  中心座標更新（目標）
        m_TargetCenter      =   m_rPlayer.transform.position;
        //  中心座標更新（現在）
        m_CurCenter         +=  ( m_TargetCenter - m_CurCenter ) / 5.0f;


        //  角度制限
        float       c_AngleLimit    =   c_VerticalDeadAngle * Mathf.Deg2Rad;
        Vector3[]   checkDir        =   new Vector3[]{  Vector3.up.normalized,  Vector3.down.normalized     };
        for( int i = 0; i < 2; i++ ){
            Vector3 useDir          =   checkDir[ i ];
            float   downDot         =   Mathf.Acos( Vector3.Dot( transform.forward.normalized, useDir ) );
            float   absDot          =   Mathf.Abs( downDot );

            //  垂直方向を向いていた場合
            if( absDot == 0.0f ){}
            //  制限角度を超えていないかチェック
            else
            if( absDot < c_AngleLimit ){
                Vector3 rotAxis     =   Vector3.Cross( transform.forward, useDir );
                        rotAxis     =   rotAxis.normalized;
                float   rotAngle    =   c_AngleLimit - absDot;
                        rotAngle    =   rotAngle * Mathf.Rad2Deg;
            
                transform.Rotate( rotAxis, -rotAngle, Space.World );
            }
        }
    }
}

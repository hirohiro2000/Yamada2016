
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TPSRotationController : MonoBehaviour
{
	[SerializeField]
	Transform YAxisRotater = null;

    [SerializeField]
	Transform m_neckTransform = null;

    [SerializeField]
    Vector3   m_neckAdjustEulerAngles = Vector3.zero;

	[SerializeField]
	Transform m_eyeTransform = null;

	[SerializeField]
    Vector3   m_eyeAdjustEulerAngles = Vector3.zero;



	private PlayerRecoil        m_playerRecoil  = null ;
    private NetworkIdentity     m_rIdentity     = null;
    private CharacterController m_character     = null;

	// Use this for initialization
	void Start()
	{
		m_rIdentity     = GetComponent< NetworkIdentity >();
		m_playerRecoil  = GetComponent<PlayerRecoil>();
        m_character     = GetComponent<CharacterController>();
    }

	// Update is called once per frame
	void Update()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rIdentity.isLocalPlayer )    return;

		if(Time.timeScale != .0f)
		{
            Transform               rCamTrans   =   Camera.main.transform.parent;
            TPS_CameraController    cam         =   null;
            if( rCamTrans )         cam         =   rCamTrans.GetComponent< TPS_CameraController >();
            
            if( rCamTrans )
            {
                Vector3 f = m_character.velocity;
                f.y = 0.0f;
                if (f.sqrMagnitude > 0.0f)
                {
                    Quaternion qt = Quaternion.LookRotation(f);
                    YAxisRotater.localRotation = qt;
                }


                // 首と目の処理
                Vector3 lookDir = rCamTrans.forward.normalized;

                Vector3 lookDirHorizontal = new Vector3( lookDir.x, 0.0f, lookDir.z );
                if ( lookDirHorizontal.sqrMagnitude > 0.0f )
                {
                    m_neckTransform.rotation = Quaternion.LookRotation( lookDirHorizontal.normalized );
                    m_neckTransform.localEulerAngles += m_neckAdjustEulerAngles;
                }

                Vector3 eulerAngles = m_eyeAdjustEulerAngles;
                eulerAngles.y -= Mathf.Asin(lookDir.y)*Mathf.Rad2Deg;
                m_eyeTransform.localEulerAngles = eulerAngles;


                // リコイル処理変更時の一時しのぎ用
                //　パラメーターを変更する際に[m_playerRecoil.cntRecoil]を直接代入できるようにしてください
                Vector2 s = m_playerRecoil.cntDisplayRecoiil * Mathf.Deg2Rad;
                s.y = -s.y;

                //
                if( cam ){
                    cam.Shake(s);
                }
            }

		}

	}

	public void Recoil(float val)
	{
		m_playerRecoil.Shot();
    }
}
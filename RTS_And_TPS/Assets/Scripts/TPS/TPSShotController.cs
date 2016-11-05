
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TPSShotController : NetworkBehaviour {

	[SerializeField]
	Transform[] firePoints = null;

	[SerializeField]
	GameObject emitter = null;

	[SerializeField]
	float shotCooldown =.0f;

	int fireCnt;
	float cntCoolDown;

	[SerializeField]
	float shotDistance = 100.0f;

	[SerializeField]
	float singleRecoil = 0.2f;

	//bool Targeted;

	float AimDistance;

	//[SerializeField]
	//TPSRotationController tpsRotationController = null;

	[SerializeField]
	PlayerRecoil playerRecoil = null;


	//  親へのアクセス
	private NetworkIdentity     m_rParentIdentity   =   null;
    private NetPlayer_Control   m_rNPControl        =   null;
    //  外部へのアクセス
    private LinkManager         m_rLinkManager      =   null;

    // サウンド
    private SoundController     m_seShot            =   null;

	// Use this for initialization
	void    Start()
    {
        //  アクセスの取得
	    m_rParentIdentity   =   transform.parent.parent.GetComponent< NetworkIdentity >();
        m_rNPControl        =   transform.parent.parent.GetComponent< NetPlayer_Control >();
        m_rLinkManager      =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        TPSWeaponBar.Initialize(100);
        m_seShot            =   SoundController.CreateShotController(transform);

    }
	
	// Update is called once per frame
	void    Update () {
        //  自分のキャラクター以外は処理を行わない
        if( !m_rParentIdentity.isLocalPlayer )  return;

		cntCoolDown -= Time.deltaTime;
		//そのままの前方向
		Transform firePoint = firePoints[fireCnt];
		Vector3 targetPoint = firePoints[fireCnt].position + firePoints[fireCnt].forward;
		//画面中央に飛ばす
		{
			//画面中央を取得(Near,Far)
			Vector3 shotPointNear, shotPointFar;

			//発射点の画面震度を取得
            if( !Camera.main )   return;
            float worldnearZ = Camera.main.WorldToViewportPoint(firePoint.position).z;

			//画面右上 = (1,1)
			Vector2 shotViewportPos = new Vector2(0.5f, 0.5f);
			shotPointNear = Camera.main.ViewportToWorldPoint(new Vector3(shotViewportPos.x, shotViewportPos.y, worldnearZ));
			shotPointFar = Camera.main.ViewportToWorldPoint(new Vector3(shotViewportPos.x, shotViewportPos.y, Camera.main.farClipPlane));
			Vector3 direction = (shotPointFar - shotPointNear).normalized;
			Ray ray = new Ray(shotPointNear, direction);

			RaycastHit hit;
			if(Physics.Raycast(ray,out hit, 1000.0f))
			{
				AimDistance = hit.distance;
				if (AimDistance <= shotDistance)
				{
					targetPoint = hit.point;
				}

            }
        }

        if (Input.GetMouseButton(0))
		{
			if(cntCoolDown < 0)
			{
				cntCoolDown = shotCooldown;
				
				Shot( firePoint.position, targetPoint );
				fireCnt++;
				if(firePoints.Length <= fireCnt)
				{
					fireCnt = 0;
                }

                TPSWeaponBar.Consumption(1.0f);
                // 発砲音
                // m_seShot.PlayOneShot();
            }
		}    
	}

    void    Shot( Vector3 firePoint, Vector3 target )
	{

		Vector3 forward;
		forward = (target - firePoint).normalized;

		//レティクルをクォータニオンに変換して回転
		forward = playerRecoil.GetReticleVector(forward);

		GameObject emit = Instantiate(emitter, firePoint, Quaternion.LookRotation(forward)) as GameObject;
		float speed = 200.0f;
		emit.GetComponent<Rigidbody>().velocity = forward * speed;

		//距離に合わせて寿命を設定
		emit.GetComponent<TPSNormalGun>().Shot_Start(shotDistance / speed);

        //  所属設定
        TPSNormalGun    rGun    =   emit.GetComponent< TPSNormalGun >();
        rGun.c_ShooterID    =   m_rLinkManager.m_LocalPlayerID;

		//リコイル処理
		playerRecoil.Shot();

        //  他のクライアントでも発射
        m_rNPControl.CmdFire_Client( firePoint, target, m_rLinkManager.m_LocalPlayerID );
    }
    public  void    Shot_ByRequest( Vector3 firePoint, Vector3 target, int _ShooterID )
    {
        Vector3 forward;
		forward = (target - firePoint).normalized;
		GameObject emit = Instantiate(emitter, firePoint, Quaternion.LookRotation(forward)) as GameObject;
		float speed = 200.0f;
		emit.GetComponent<Rigidbody>().velocity = forward * speed;

		//距離に合わせて寿命を設定
		emit.GetComponent<TPSNormalGun>().Shot_Start(shotDistance / speed);

        //  所属設定
        TPSNormalGun    rGun    =   emit.GetComponent< TPSNormalGun >();
        rGun.c_ShooterID    =   _ShooterID;
    }

	void OnGUI()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rParentIdentity.isLocalPlayer )  return;

		//照準の描画
		GUIStyle style = new GUIStyle();
		if(AimDistance <= shotDistance)
		{
			style.normal.textColor = new Color(1.0f,0.5f,.0f);
		}
		else
		{
			style.normal.textColor = Color.white;
		}
		style.fontSize =(int)( Screen.height * 0.05f);
		style.alignment = TextAnchor.MiddleCenter;

		GUI.Label(new Rect(.0f, .0f, Screen.width, Screen.height), "+", style);
		style.fontSize = (int)(Screen.height * 0.02f);
		style.alignment = TextAnchor.MiddleLeft;
		GUI.Label(new Rect(Screen.height * 0.02f + Screen.width  * 0.5f  , Screen.height * 0.02f, Screen.width, Screen.height), AimDistance.ToString(), style);


	}

}

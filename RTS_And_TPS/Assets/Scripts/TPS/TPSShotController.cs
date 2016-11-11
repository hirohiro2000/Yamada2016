
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


//Utilityに追加してもいいかも
[System.Serializable]
public class TransformList
{
	public Transform value;
}

[System.Serializable]
public class TransformReorderableList : ReorderableList<TransformList> { }


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


	[SerializeField]
	KeyCode weaponChangeKey = KeyCode.None;

	[SerializeField]
	KeyCode shotKey = KeyCode.None;

	[SerializeField, ReorderableList(new int[] { 200 })]
	TransformReorderableList weapons;
	int cntWeaponIndex;

	public class WeaponData
	{

		Transform _weapon = null;
		bool[] shotInfoFlag = new bool[(int)ShotInfoFlag.FLAG_MAX];
		float[] shotInfoValue = new float[(int)ShotInfoValue.VALUE_MAX];
		WeaponRecoilData _weaponRecoilData = null;

		public WeaponData(Transform weapon)
		{
			_weapon = weapon;
			//ShotInfomationから情報をとる
			ShotInfomation info = weapon.GetComponent<ShotInfomation>();
			if(info != null)
			{
				foreach(ShotInfoFlagParam flag in info.shot_info_flags.list)
				{
					shotInfoFlag[(int)flag.type] = true;
				}
				foreach (ShotInfoValueParam value in info.shot_info_values.list)
				{
					shotInfoValue[(int)value.type] = value.value;
				}

			}

			_weaponRecoilData = weapon.GetComponent<WeaponRecoilData>();
		}

		public bool GetInfoFlag (ShotInfoFlag type) { return shotInfoFlag[(int)type]; }
		public float GetInfoValue(ShotInfoValue type) { return shotInfoValue[(int)type]; }
		public Transform weapon
		{
			get
			{
				return _weapon;
			}
		}
		public WeaponRecoilData weaponRecoilData
		{
			get
			{
				return _weaponRecoilData;
			}
		}

        public SoundController CreateSoundController( GameObject user )
        {
			ShotInfomation info = _weapon.GetComponent<ShotInfomation>();
			if(info != null)
            {
                SoundController controller = SoundController.Create( info.m_seFileName, user.transform );
                return controller;
            }

            return null;
        }

	}
	WeaponData cntWeaponData = null;


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

    //  内部パラメータ
    private int                 m_MyChildID         =   0;

    // サウンド
    private SoundController     m_seShot            =   null;

	// Use this for initialization
	void    Start()
    {
        //  アクセスの取得
	    m_rParentIdentity   =   transform.parent.parent.GetComponent< NetworkIdentity >();
        m_rNPControl        =   transform.parent.parent.GetComponent< NetPlayer_Control >();
        m_rLinkManager      =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

		cntWeaponData = new WeaponData(weapons[0].value);
        //  自分が何番目の子か調べる（他のクライアントで発射させるときにどこから発射されたかを識別するため）
        m_MyChildID         =   CheckMyChildID();

//        TPSWeaponBar.Initialize(100);
//        m_seShot            =   SoundController.CreateShotController(transform);

        TPSWeaponBar.Initialize(100);
        m_seShot            =   cntWeaponData.CreateSoundController(this.gameObject);

    }
	
	// Update is called once per frame
	void    Update () {
        //  自分のキャラクター以外は処理を行わない
        if( !m_rParentIdentity.isLocalPlayer )  return;

		if(Input.GetKeyDown(weaponChangeKey))
		{
			WeaponChange();
		}
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
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                AimDistance = hit.distance;
//                if (AimDistance <= shotDistance)
                {
                    targetPoint = hit.point;
                }
            }
            else
            {
                GameObject cam = Camera.main.gameObject;
                Vector3 lookVec = cam.transform.forward;
                float dot = Vector3.Dot( lookVec, ( firePoint.position - cam.transform.position) );
                targetPoint = cam.transform.position + lookVec*( dot + 40.0f );

            }
        }
		//単発orフルオート
		bool shotRequest = false;
		if (Input.GetKey(shotKey) && cntWeaponData.GetInfoFlag(ShotInfoFlag.FullAutoShot))
			shotRequest = true;
		else if(Input.GetKeyDown(shotKey))
			shotRequest = true;



		if (shotRequest == true)
		{
			if(cntCoolDown < 0)
			{
				cntCoolDown = cntWeaponData.GetInfoValue(ShotInfoValue.CoolDownTime);
				
				Shot( firePoint.position, targetPoint );
				fireCnt++;
				if(firePoints.Length <= fireCnt)
				{
					fireCnt = 0;
                }

                TPSWeaponBar.Consumption(1.0f);

                // 発砲音
                if (m_seShot != null)
                {
                    m_seShot.transform.position = firePoint.position;
                    m_seShot.PlayOneShot();
                }
            }
		}

    }

    void    Shot( Vector3 firePoint, Vector3 target )
	{
		//リコイル再設定
		playerRecoil.holdingWeapon = cntWeaponData.weaponRecoilData;

		Vector3 forward;
		forward = (target - firePoint).normalized;

		//レティクルをクォータニオンに変換して回転
		forward = playerRecoil.GetReticleVector(forward);

		GameObject emit = Instantiate(cntWeaponData.weapon.gameObject, firePoint, Quaternion.LookRotation(forward)) as GameObject;

		//cloneをまとめる
        string parentName = emit.name + "s";
		GameObject parent = GameObject.Find(parentName);
		if (parent == null)
		{
			parent = new GameObject(parentName);
		}
		emit.transform.parent = parent.transform;

        //  所属を設定
        TPSAttack_Net   rAttack =   emit.GetComponent< TPSAttack_Net >();
        rAttack.c_AttackerID    =   m_rLinkManager.m_LocalPlayerID;

		//リコイル処理
		playerRecoil.Shot();

        //  他のクライアントでも発射
        m_rNPControl.CmdFire_Client( firePoint, target, m_rLinkManager.m_LocalPlayerID, cntWeaponIndex, m_MyChildID );
    }
    public  void    Shot_ByRequest( Vector3 firePoint, Vector3 target, int _ShooterID, int _WeaponID )
    {
        Vector3 forward;
		forward = (target - firePoint).normalized;

		GameObject  emit  = Instantiate( weapons[ _WeaponID ].value.gameObject, firePoint, Quaternion.LookRotation(forward)) as GameObject;

		//cloneをまとめる
        string parentName = emit.name + "s";
		GameObject parent = GameObject.Find(parentName);
		if (parent == null)
		{
			parent = new GameObject(parentName);
		}
		emit.transform.parent = parent.transform;

        //  所属を設定
        TPSAttack_Net   rAttack =   emit.GetComponent< TPSAttack_Net >();
        rAttack.c_AttackerID    =   m_rLinkManager.m_LocalPlayerID;
    }

	void WeaponChange()
	{
		cntWeaponIndex++;
		if (weapons.Length <= cntWeaponIndex)
			cntWeaponIndex = 0;
		cntWeaponData = new WeaponData(weapons[cntWeaponIndex].value);

		playerRecoil.holdingWeapon = cntWeaponData.weaponRecoilData;

        m_seShot = cntWeaponData.CreateSoundController(this.gameObject);

    }

	void OnGUI()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rParentIdentity.isLocalPlayer )  return;

		if (playerRecoil.holdingWeapon == null) return;


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
		style.fontSize =(int)( Screen.height * 0.03f);
		style.alignment = TextAnchor.MiddleCenter;

		//GUI.Label(new Rect(.0f, .0f, Screen.width, Screen.height), "・", style);
		style.fontSize = (int)(Screen.height * 0.02f);
		style.alignment = TextAnchor.MiddleLeft;
		GUI.Label(new Rect(Screen.height * 0.02f + Screen.width  * 0.5f  , Screen.height * 0.02f, Screen.width, Screen.height), AimDistance.ToString(), style);

		//レティクルの描画
		float reticle = playerRecoil.GetReticle();
		style.fontSize = (int)(Screen.height * 0.015f);
		style.fontStyle = FontStyle.Bold;
		style.alignment = TextAnchor.MiddleCenter;

		for (int i = 0; i < 4; i++)
		{
			Rect rect;
			int mul;
			if (i % 2 == 0)
				mul = 1;
			else
				mul = -1;
			if(i / 2 == 0)
			{
				rect = new Rect((reticle + 1) * mul * Screen.height * 0.006f, - Screen.height * 0.001f, Screen.width, Screen.height);
				GUI.Label(rect, "―" ,style);

			}
			else
			{
				rect = new Rect(.0f, (reticle + 1) * mul * Screen.height * 0.006f, Screen.width, Screen.height);
				GUI.Label(rect, "|", style);
			}
		}
	}

    //  自分が何番目の子かを調べる
    int     CheckMyChildID()
    {
        int     numChild    =   transform.parent.childCount;
        for( int i = 0; i < numChild; i++ ){
            if( transform.parent.GetChild( i ) != transform )   continue;

            return  i;
        }

        return  -1;
    }
}

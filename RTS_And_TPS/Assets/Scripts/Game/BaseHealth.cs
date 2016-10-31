
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class BaseHealth : NetworkBehaviour {
    
	[SerializeField]
	float maxHp = 0;

    [ SyncVar ]
	float hp;

	bool isDeath;

	[SerializeField]
	string text = null;

	[SerializeField]
	Rect rect;

    //  外部へのアクセス
    private GameManager     m_rGameManager  =   null;

	// Use this for initialization
	void    Start()
	{
		hp  =   maxHp;

        //  アクセス取得
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
	}

	// Update is called once per frame
	void    Update()
	{

	}

    void    OnTriggerEnter( Collider _rCollider )
    {
        //  サーバーでのみ処理を行う
        if( !isServer )                                             return;

        //  突っ込んできたのがエネミーかどうかチェック
        TPS_Enemy       rEnemy  =   _rCollider.GetComponent< TPS_Enemy >();
        if( !rEnemy )   rEnemy  =   _rCollider.transform.GetComponentInParent< TPS_Enemy >();
        if( !rEnemy )   return;

        //  ダメージを受ける
        hp  =   Mathf.Max( --hp, 0 );
        if( hp == 5 ){
            m_rGameManager.RpcMainMessage( "拠点が深刻な被害を受けています", 2.7f, 0.0f );
        }

        //  ゲームオーバー処理（ゲーム中のみ）
        if( hp <= 0.0f
        &&  m_rGameManager.GetState() == GameManager.State.InGame ){
            //  ゲームオーバー
            m_rGameManager.GameOver();

            //  メッシュを非アクティブ化
            Transform   rCylinder   =   transform.FindChild( "Cylinder" );
            GameObject  rObj        =   rCylinder.gameObject;
            rObj.SetActive( false );

            //  クライアント側でも非表示にする
            RpcDisableCylinder();
        }

        //  エネミーを破棄
        Destroy( rEnemy.gameObject );
    }

	public void OnGUI()
	{
        GUIStyle style2 = new GUIStyle();
        style2.normal.textColor = Color.white;
        style2.fontSize = (int)(Screen.height * 0.03f);

        GUI.Label(new Rect(
            Screen.width * rect.x, Screen.height * rect.y,
            Screen.width * rect.width, Screen.height * rect.height),text + hp, style2);



	}

    //  リクエスト
    [ ClientRpc ]
    void    RpcDisableCylinder()
    {
        if( isServer )  return;

        //  メッシュを非アクティブ化
        Transform   rCylinder   =   transform.FindChild( "Cylinder" );
        GameObject  rObj        =   rCylinder.gameObject;
        rObj.SetActive( false );
    }
}


using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RTSEnemy : NetworkBehaviour
{
    private LinkManager m_rLinkManager  =   null;

	// Use this for initialization
	void    Start()
	{
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        GameObject  esp     =   GameObject.Find( "EnemySpawnPoints" );
		transform.position  =   esp.transform.GetChild( Random.Range( 0, esp.transform.childCount ) ).transform.position;

		GameObject  dt  =   GameObject.Find( "Homebase/Cylinder" );
		GetComponent< NavMeshAgent >().Warp( transform.position );
        GetComponent< NavMeshAgent >().SetDestination( dt.transform.position );

        //  クライアント側ではナビメッシュを使用しない
        if( !isServer ){
            GetComponent< NavMeshAgent >().enabled  =   false;
        }
	}
    public  override    void    OnStartServer()
    {
        transform.parent    =   GameObject.Find( "EnemyFactory" ).transform;
    }
    public  override    void    OnStartClient()
    {
        transform.parent    =   GameObject.Find( "EnemyFactory" ).transform;
    }
	
	// Update is called once per frame
	void Update ()
	{

	}

    public  void    Death_Proc()
    {
        //	test
        int getcost = 5;
        
        ItemController  rItem   =   FunctionManager.GetAccessComponent< ItemController >( "RTSPlayer_Net(Clone)" );
        if( rItem ) rItem.AddResourceCost( getcost );
        //GameObject.Find( "MechanicalGirl" ).GetComponent<ItemController>().AddResourceCost( getcost );
        GameObject.Find("NumberEffectFactory").GetComponent<NumberEffectFactory>().Create( transform.position, getcost, Color.green );
        
        Destroy( gameObject );
    }
	//	↓仮
	void CollisionCommon( GameObject obj )
	{
        //  当たり判定はサーバーでのみ行う
        if( !isServer ) return;

		var a = GameObjectExtension.GetComponentInParentAndChildren<CollisionParam>( obj.gameObject );
		var d = GetComponent<CollisionParam>();

		CollisionParam.ComputeDamage( a, ref d, true );

		if( d.m_hp <= 0 )
		{
            Death_Proc();
		}
	}
	void OnCollisionEnter( Collision collision )
	{
		if ( collision.gameObject.tag == "ResourceOn")
			CollisionCommon( collision.gameObject );

        //  ＴＰＳプレイヤーからのダメージ
        {
            GameObject      rObj        =   collision.gameObject;
            TPSNormalGun    rGunControl =   rObj.GetComponent< TPSNormalGun >();
            if( !rGunControl )                                              return;
            //  発射したプレイヤーのクライアントでのみ処理を行う
            if( rGunControl.c_ShooterID != m_rLinkManager.m_LocalPlayerID ) return;

            //  ダメージ処理
            DamageSource    rDamage     =   rObj.GetComponent< DamageSource >();
            if( !rDamage )                                                  return;

            //  発射したプレイヤーを介してサーバーにダメージを送信
            NetPlayer_Control   rNPControl  =   m_rLinkManager.m_rLocalPlayer.GetComponent< NetPlayer_Control >();
            rNPControl.CmdSendDamageEnemy_RTS( netId, rDamage.damage );
        }

	}
	void OnCollisionStay( Collision collision )
	{
		if ( collision.gameObject.tag == "ResourceStay")
			CollisionCommon(collision.gameObject);
	}
	void OnTriggerEnter( Collider collision )
	{
		if ( collision.gameObject.tag == "ResourceOn" )
			CollisionCommon( collision.gameObject );
	}
	void OnTriggerStay( Collider collision )
	{
		if ( collision.gameObject.tag == "ResourceStay" )
			CollisionCommon( collision.gameObject );
	}
}


using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TPS_Enemy : NetworkBehaviour {
    [ SyncVar ]
    public  float   m_MaxHP     =   1.0f;
	[ SerializeField, SyncVar ]
	public  float   hp          =   0.0f;

	[SerializeField]
	HealthBar3D healthBar3D;

	[SerializeField]
	Transform hpBar;


    //  外部へのアクセス
    private LinkManager m_rLinkManager  =   null;


	// Use this for initialization
	void    Start()
    {
		if (healthBar3D != null)
			healthBar3D.setValue(1.0f);

        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        UIRadar.AddEnemy(this.gameObject);


        //  パラメータ初期化
        hp  =   m_MaxHP;
    }
    public  override    void    OnStartServer()
    {
        transform.parent    =   GameObject.Find( "Enemy_Shell" ).transform;
    }
    public  override    void    OnStartClient()
    {
        transform.parent    =   GameObject.Find( "Enemy_Shell" ).transform;
    }
	
	// Update is called once per frame
	void    Update()
    {
	    //  ゲージ更新
        {
            if( healthBar3D ){
			    healthBar3D.setValue( hp / m_MaxHP );
            }
        }
	}

	public void GiveDamage(float damage)
	{
		if (hp <= .0f)
			return;

		hp -= damage;
		if (healthBar3D != null)
			healthBar3D.setValue(hp / m_MaxHP);
		if (hp <= .0f)
		{
			if (hpBar != null)
			{
				hpBar.SetParent(null);
				Destroy(hpBar.gameObject, 0.5f);
			}

            EnemyKillCounter.killCount++;
                        
			Destroy(this.gameObject);
		}
    }

	void    OnCollisionEnter( Collision collision )
	{
        GameObject      rObj        =   collision.gameObject;
        TPSNormalGun    rGunControl =   rObj.GetComponent< TPSNormalGun >();
        if( !rGunControl )                                              return;
        //  発射したプレイヤーのクライアントでのみ処理を行う
        if( rGunControl.c_ShooterID != m_rLinkManager.m_LocalPlayerID ) return;

		DamageSource source = collision.gameObject.GetComponentInParent<DamageSource>();
		if (source != null)
		{
			//  一番倍率の高い衝突を探す
			float maxDamageMultiple = .0f;

			foreach ( ContactPoint colider in collision.contacts)
			{
				WeakPoint weakPoint = colider.thisCollider.GetComponent<WeakPoint>();

				if(weakPoint != null)
				{
					Debug.Log(weakPoint.gameObject.name + ":" + weakPoint.damageMultiple);

					if(weakPoint.damageMultiple > maxDamageMultiple)
					{
						maxDamageMultiple = weakPoint.damageMultiple;
                    }
				}
			}

            //  プレイヤーを介してサーバーにダメージを送信
            NetPlayer_Control   rNPControl  =   m_rLinkManager.m_rLocalPlayer.GetComponent< NetPlayer_Control >();
            rNPControl.CmdSendDamageEnemy( netId, maxDamageMultiple * source.damage );
		}
	
	}

    void    OnDestroy()
    {
        UIRadar.Remove(this.gameObject);
    }

}

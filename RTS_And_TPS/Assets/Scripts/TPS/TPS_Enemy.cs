
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TPS_Enemy : NetworkBehaviour {
	[ SerializeField, SyncVar ]
	float hp;

	[SerializeField]
	HealthBar3D healthBar3D = null;

	[SerializeField]
	Transform hpBar = null;


    //  外部へのアクセス
    private LinkManager m_rLinkManager  =   null;


	// Use this for initialization
	void    Start()
    {
		if (healthBar3D != null)
			healthBar3D.setValue(1.0f);

//            UIRadar.AddEnemy(this.gameObject);

        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
    }
	
	// Update is called once per frame
	void    Update()
    {
	    //  ゲージ更新
        {
            if( healthBar3D ){
			    healthBar3D.setValue( hp / 20.0f );
            }
        }
	}

	public void GiveDamage(float damage)
	{
		if (hp <= .0f)
			return;

		hp -= damage;
		if (healthBar3D != null)
			healthBar3D.setValue(hp / 20.0f);
		if (hp <= .0f)
		{
			if (hpBar != null)
			{
				hpBar.SetParent(null);
				Destroy(hpBar.gameObject, 0.5f);
			}

            EnemyKillCounter.killCount++;

//            UIRadar.Remove(this.gameObject);
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


			//GiveDamage(maxDamageMultiple * source.damage);
            //  プレイヤーを介してサーバーにダメージを送信
            NetPlayer_Control   rNPControl  =   m_rLinkManager.m_rLocalPlayer.GetComponent< NetPlayer_Control >();
            rNPControl.CmdSendDamageEnemy( netId, maxDamageMultiple * source.damage );
		}
	
	}
}

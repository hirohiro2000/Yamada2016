using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AttackPointList))]
[RequireComponent(typeof(SphereCollider))]
public class ExplosionAttack : MonoBehaviour {
    public  int     c_DestroyCounter        =   0;
    public  bool    c_UseExplosionForce     =   false;

	[SerializeField]
	AnimationCurve hitPowerLengthRate;

	AttackPointList _attackPointList = null;

	AttackPointList attackPointList
	{
		get
		{
			if(_attackPointList == null)
			{
				_attackPointList = GetComponent<AttackPointList>();
            }
			return _attackPointList;
		}
	}

	SphereCollider _sphereCollider = null;

	SphereCollider sphereCollider
	{
		get
		{
			if (_sphereCollider == null)
			{
				_sphereCollider = GetComponent<SphereCollider>();
			}
			return _sphereCollider;
		}
	}



	// Use this for initialization
	void Awake () {

		attackPointList.BeforeCalcDamegeCallBack += (ref AttackPointListData atk, CollisionInfo info) =>
		{
			//距離により修正
			//当たり判定の半径を取得
			float colliderGlobalsize = sphereCollider.bounds.size.x;

			//敵の距離により割合を出す
			float rate = (transform.position - info.damagedObject.position).magnitude / colliderGlobalsize;

			//割合を攻撃力に反映
			atk.baseAttackPoint *= hitPowerLengthRate.Evaluate(rate);
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	void    FixedUpdate()
	{
        //  カウンター数分フレームが経過してから破棄
        if( c_DestroyCounter-- <= 0 ){
            //  爆風反映
            if( c_UseExplosionForce ){
                float       sphereSize  =   0.0f;
                            sphereSize  =   Mathf.Max( transform.lossyScale.x, sphereSize );
                            sphereSize  =   Mathf.Max( transform.lossyScale.y, sphereSize );
                            sphereSize  =   Mathf.Max( transform.lossyScale.z, sphereSize );
                            sphereSize  =   sphereSize * GetComponent< SphereCollider >().radius;
                float       expRadius   =   sphereSize;

                int         layerMask   =   LayerMask.GetMask( "MoverAndDefender" )
                                        |   LayerMask.GetMask( "Defender" );
                Collider[]  rColliders  =   Physics.OverlapSphere( transform.position, expRadius, layerMask );
                for( int i = 0; i < rColliders.Length; i++ ){            
                    Rigidbody   rRigid  =   rColliders[ i ].GetComponentInParent< Rigidbody >();
                    if( !rRigid )   continue;
            
                    //  力を加える 
                    float   power   =   25.0f;
                    float   upper   =   50.0f;
                    Vector3 vEXP    =   rRigid.transform.position - transform.parent.position;
                            vEXP    =   new Vector3( vEXP.x, 0.0f, vEXP.z );
                            vEXP    =   vEXP.normalized * power;
                            vEXP.y  +=  upper;
                    rRigid.AddForce( vEXP, ForceMode.Impulse );
                }
            }

            //  オブジェクトを破棄
            Destroy( this.gameObject );
        }
	}
}

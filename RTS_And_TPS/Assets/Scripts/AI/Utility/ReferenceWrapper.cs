using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReferenceWrapper : MonoBehaviour {

 //   public GameObject m_home_base { get; set; }
    public GameObject m_attack_object_root { get; private set; }
    public List<GameObject> m_active_enemy_list { get; private set; }

    void Awake()
    {
        //m_home_base = GameObject.Find("Homebase");
        m_attack_object_root = GameObject.Find("EnemyAttackObjectRoot");
        m_active_enemy_list = GetComponent<EnemyGenerator>().GetCurrentHierachyList();
    }
	void Update()
    {
        //  アクセスの取得
        if( !m_attack_object_root ) m_attack_object_root    =   GameObject.Find("EnemyAttackObjectRoot");

       // UserLog.Terauchi(m_active_enemy_list.Count);
    }

	public bool IsExistEnemy()
	{
		return m_active_enemy_list.Count > 0;
	}

	public bool CheckWhetherWithinTheRange( Vector3 target, float rangeDist )
	{
		float	near	= rangeDist*rangeDist;

		for( int i=0; i<m_active_enemy_list.Count; ++i )
		{
            if( !m_active_enemy_list[ i ] ) continue;

			Vector3 pos		    =	m_active_enemy_list[i].transform.position;

            Vector3 vToTarget   =   target - pos;
                    vToTarget.y =   0.0f;   //  高さを考慮しない
			float	dist	    =   vToTarget.sqrMagnitude;

			if( dist < near )
			{
				return true;
			}
		}

		return false;
	}
	public Transform GetNearEnemyTransform( Vector3 target, float maxDist )
	{
		int		nearID	= -1;
		float	near	= maxDist*maxDist;

		for( int i=0; i<m_active_enemy_list.Count; ++i )
		{
            if( !m_active_enemy_list[ i ] ) continue;

			Vector3 pos		    =	m_active_enemy_list[i].transform.position;

            Vector3 vToTarget   =   target - pos;
                    vToTarget.y =   0.0f;   //  高さを考慮しない
			float	dist	    =   vToTarget.sqrMagnitude;

			if( dist < near )
			{
				near	= dist;
				nearID	= i;
			}
		}

		if( nearID != -1 )
		{
			return m_active_enemy_list[ nearID ].transform;
		}

		return null;
	}
    public  Transform   GetNearEnemyTransform_CheckWall( Vector3 target, float maxDist )
	{
		int		nearID	= -1;
		float	near	= maxDist*maxDist;

		for( int i=0; i<m_active_enemy_list.Count; ++i )
		{
            if( !m_active_enemy_list[ i ] ) continue;

			Vector3 pos		    =	m_active_enemy_list[i].transform.position;

            Vector3 vToTarget   =   target - pos;
                    vToTarget.y =   0.0f;   //  高さを考慮しない
			float	dist	    =   vToTarget.sqrMagnitude;

            //  壁判定
            {
                Vector3 eyePos      = m_active_enemy_list[ i ].transform.FindChild( "Eye" ).position;
			    var		vector		= eyePos - target;
			    Vector3	dir			= vector.normalized;
			    int     layerMask   = LayerMask.GetMask( "Field" );

                RaycastHit  hitInfo;
			    if( Physics.Raycast(  target, dir, out hitInfo, vector.magnitude, layerMask ) ){
                    //  周辺情報を計算
                    Vector3 vToHit      =   hitInfo.point - target;
                    Vector3 vToHit2D    =   new Vector3( vToHit.x, 0.0f, vToHit.z );
                    float   upAngle     =   Vector3.Angle( Vector3.up, hitInfo.normal );
                    float   hitDist     =   vToHit2D.magnitude;

                    //  直近の床だけ無視 
                    if( upAngle < 30.0f
                    &&  hitDist < 3.0f ){}
                    //  壁はだめ
                    else                                                        continue;
                }
		    }

			if( dist < near )
			{
				near	= dist;
				nearID	= i;
			}
		}

		if( nearID != -1 )
		{
			return m_active_enemy_list[ nearID ].transform;
		}

		return null;
    }
	public List<Transform> GetNearEnemyTransforms( Vector3 target, float maxDist )
	{
		List<Transform> list	= new List<Transform>();
		float			near	= maxDist*maxDist;

		for( int i=0; i<m_active_enemy_list.Count; ++i )
		{
            if( !m_active_enemy_list[ i ] ) continue;

			Vector3 pos		    =	m_active_enemy_list[i].transform.position;

            Vector3 vToTarget   =   target - pos;
                    vToTarget.y =   0.0f;   //  高さを考慮しない
			float	dist	    =   vToTarget.sqrMagnitude;

			if( dist < near )
			{
				list.Add( m_active_enemy_list[ i ].transform );
			}
		}

		return list;
	}
}

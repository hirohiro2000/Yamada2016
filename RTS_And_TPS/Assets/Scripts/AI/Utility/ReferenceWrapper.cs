using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReferenceWrapper : MonoBehaviour {

    public GameObject m_home_base { get; private set; }
    public GameObject m_attack_object_root { get; private set; }
    public List<GameObject> m_active_enemy_list { get; private set; }

    void Awake()
    {
        m_home_base = GameObject.Find("Homebase");
        m_attack_object_root = GameObject.Find("EnemyAttackObjectRoot");
        m_active_enemy_list = GetComponent<EnemyGenerator>().GetCurrentHierachyList();
    }
	void Update()
    {
      //  UserLog.Terauchi(m_active_enemy_list.Count);
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
	public List<Transform> GetNearEnemyTransforms( Vector3 target, float maxDist )
	{
		List<Transform> list	= new List<Transform>();
		float			near	= maxDist*maxDist;

		for( int i=0; i<m_active_enemy_list.Count; ++i )
		{
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

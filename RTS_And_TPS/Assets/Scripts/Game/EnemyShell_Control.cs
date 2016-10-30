using UnityEngine;
using System.Collections;

public class EnemyShell_Control : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsExistEnemy()
	{
		return transform.childCount > 0;
	}
	public bool CheckWhetherWithinTheRange( Vector3 target, float rangeDist )
	{
		float	near	= rangeDist*rangeDist;

		for( int i=0; i<transform.childCount; ++i )
		{
			Vector3 pos		    = transform.GetChild(i).transform.position;

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
	public bool GetNearEnemyTransform( ref Transform trans, Vector3 target, float maxDist )
	{
		int		nearID	= -1;
		float	near	= maxDist*maxDist;

		for( int i=0; i<transform.childCount; ++i )
		{
			Vector3 pos		    = transform.GetChild(i).transform.position;

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
			trans = transform.GetChild( nearID ).transform;
			return true;
		}

		trans = null;
		return false;
	}
}

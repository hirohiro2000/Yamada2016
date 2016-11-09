
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSNormalGun : NetworkBehaviour
{
	[SerializeField]
	GameObject particle = null;

	float cntTime;
	float destroyTime;

    //  外部へのアクセス
    private LinkManager m_rLinkManager  =   null;

	public void Shot_Start(float time)
	{
		destroyTime = time;
	}

	// Use this for initialization
	void    Start()
	{
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
	}

	// Update is called once per frame
	void Update()
	{
		cntTime += Time.deltaTime;
		if (cntTime > destroyTime)
		{
			if (destroyTime != .0f)
				Destroy(this.gameObject);
		}
	}

	public  void    OnCollisionEnter( Collision collision )
	{
        //  ヒットエフェクト
		if( particle != null )
		{
			GameObject emit = (GameObject)Instantiate( particle, transform.position, transform.rotation * Quaternion.AngleAxis( 180, Vector3.right ) );

			//cloneをまとめる
			string parentName = emit.name + "s";
			GameObject parent = GameObject.Find(parentName);
			if(parent == null)
			{
				parent = new GameObject(parentName);
			}
			emit.transform.parent = parent.transform;

		}
        
        //  オブジェクト破棄
		Destroy( this.gameObject );
	}
}

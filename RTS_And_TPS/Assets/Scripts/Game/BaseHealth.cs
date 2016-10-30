
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


	// Use this for initialization
	void Start()
	{
		hp = maxHp;

	}

	// Update is called once per frame
	void Update()
	{

	}

	public bool IsDeath()
	{
		return isDeath;
	}

	public void GiveDamage(float damage)
	{
		hp -= damage;
		if (hp <= .0f)
		{
			isDeath = true;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		PlayerDamageSource source = collision.gameObject.GetComponentInParent<PlayerDamageSource>();
		if (source != null)
		{
			GiveDamage(source.damage);
		}
	}
    void    OnTriggerEnter( Collider _rCollider )
    {
        //  サーバーでのみ処理を行う
        if( !isServer ) return;

        //  突っ込んできたのがエネミーかどうかチェック
        TPS_Enemy       rEnemy  =   _rCollider.GetComponent< TPS_Enemy >();
        if( !rEnemy )   rEnemy  =   _rCollider.transform.GetComponentInParent< TPS_Enemy >();
        if( !rEnemy )   return;

        //  ダメージを受ける
        hp  =   Mathf.Max( --hp, 0 );

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
}

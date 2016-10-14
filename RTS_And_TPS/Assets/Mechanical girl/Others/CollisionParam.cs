using UnityEngine;
using System.Collections;

public class CollisionParam : MonoBehaviour
{
	public int	m_level			= 1;
	public int	m_attack		= 0;
	public int	m_defense		= 0;
	public int	m_hp			= 0;

	public int	m_attackUp		= 0;
	public int	m_defenseUp		= 0;
	public int	m_hpUp			= 0;

	private int m_initAttack	= 0;
	private int m_initDefense	= 0;
	private int m_initHp		= 0;

	// Use this for initialization
	void Awake ()
	{
		m_initAttack	= m_attack;
		m_initDefense	= m_defense;
		m_initHp		= m_hp;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void LevelUp()
	{
		m_level++;

		m_attack	+= m_attackUp;
		m_defense	+= m_defenseUp;
		m_hp		= m_initHp + ( m_level-1 ) * m_hpUp;	
	}

	public void SetInitParam( int attack, int defense, int hp )
	{
		m_initAttack	= attack;
		m_initDefense	= defense;
		m_initHp		= hp;

		m_attack		= attack;
		m_defense		= defense;
		m_hp			= hp;
	}

	public void Copy( CollisionParam param )
	{
		//	全コピやりかたわからん
		m_level			= param.m_level;
		m_attack		= param.m_attack;
		m_defense		= param.m_defense;
		m_hp			= param.m_hp;

		m_attackUp		= param.m_attackUp;
		m_defenseUp		= param.m_defenseUp;
		m_hpUp			= param.m_hpUp;

		m_initHp		= param.m_initHp;
		m_initAttack	= param.m_initAttack;
		m_initDefense	= param.m_initDefense;
	}

    public float GetRate()
	{
		return (float)m_hp / (float)m_initHp;
	}

	static public void ComputeDamage( CollisionParam attack, ref CollisionParam defense, bool effectEnable )
	{
		int power = attack.m_attack - defense.m_defense;

		if( power < 0 )
			power = 0;
	
		defense.m_hp -= power;

		if( effectEnable )
			GameObject.Find("NumberEffectFactory").GetComponent<NumberEffectFactory>().Create( defense.gameObject.transform.position, power, Color.yellow );
	}
}

using UnityEngine;
using System.Collections;

public class CollisionParam : MonoBehaviour
{
	public int m_attack		= 1;
	public int m_defense	= 1;
	public int m_maxhp		= 10;
	public int m_hp			= 10;

	// Use this for initialization
	void Start ()
	{
		m_maxhp = m_hp;
	}
	
	// Update is called once per frame
	void Update ()
	{	
	}

	public float GetRate()
	{
		return (float)m_hp / (float)m_maxhp;
	}

	static public void ComputeDamage( CollisionParam attack, ref CollisionParam defense )
	{
		int power = attack.m_attack - defense.m_defense;

		defense.m_hp -= power;
		GameObject.Find("NumberEffectFactory").GetComponent<NumberEffectFactory>().Create( defense.gameObject.transform.position, power, Color.yellow );
	}
}

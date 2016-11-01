using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class LevelParam
{
	public int			hp			= 0;
	public int			power		= 0;
	public float		range		= 1.0f;
	public int			upCost		= 0;
}


[System.Serializable]
public class LevelUpParamReorderableList : ReorderableList<LevelParam>
{
}


public class ResourceParameter : MonoBehaviour
{
	public int			m_createCost		= 0;
	public int			m_breakCost			= 0;

	private int			m_level				= 0;
	private int			m_curHp				= 0;

	[ ReorderableList( new int[] { 100, 100 }), HeaderAttribute ("体力ー火力ー範囲ーレベルアップ費用")]
	public LevelUpParamReorderableList m_levelInformations;


	void Awake()
	{
		m_curHp = GetCurLevel().hp;
	}


	//-------------------------------------------------------------
	//	get
	//-------------------------------------------------------------
	public LevelParam GetCurLevel()
	{
		return m_levelInformations[ m_level ];
	}
	public float GetRate()
	{
		return (float)m_curHp / (float)GetCurLevel().hp;
	}
	
	
	//-------------------------------------------------------------
	//	set
	//-------------------------------------------------------------
	public void LevelUp()
	{
		m_level++;
		m_curHp = GetCurLevel().hp;
	}
	public void GiveDamage( int damage )
	{
		m_curHp -= damage;
	}
}
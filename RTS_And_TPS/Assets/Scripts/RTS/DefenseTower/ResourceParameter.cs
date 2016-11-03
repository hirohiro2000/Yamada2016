using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class LevelParam
{
	public int			hp			= 0;
	public int			power		= 0;
	public float		range		= 1.0f;
	public float		interval	= 0;
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

	[HideInInspector]
	public int			m_level				= 0;

	[HideInInspector]
	public int			m_curHp				= 0;

	[ ReorderableList( new int[]{ 50, 50, 50, 50, 50 }), HeaderAttribute ("体力ー火力ー射程[radius]ー発射間隔[sec]ーレベルアップ費用")]
	public LevelUpParamReorderableList m_levelInformations = null;


	void Start()
	{
		m_curHp = GetCurLevelParam().hp;
	}


	//-------------------------------------------------------------
	//	get
	//-------------------------------------------------------------
	public LevelParam GetCurLevelParam()
	{
		return m_levelInformations[ m_level ];
	}
	public float GetRate()
	{
		return (float)m_curHp / (float)GetCurLevelParam().hp;
	}
	public bool CheckWhetherCanUpALevel()
	{
		return m_levelInformations.Length-1 > m_level;
	}
	
	
	//-------------------------------------------------------------
	//	set
	//-------------------------------------------------------------
	public void LevelUp()
	{
		if( m_levelInformations.Length <= m_level+1 )
			return;

		m_level++;
		m_curHp = GetCurLevelParam().hp;
	}
	public void GiveDamage( int damage )
	{
		m_curHp -= damage;
	}
	public void Copy( ResourceParameter param )
	{
		m_curHp = param.m_curHp;
		m_level = param.m_level;

		m_levelInformations			= new LevelUpParamReorderableList();
		m_levelInformations.list	= new List<LevelParam>();

		for( int i=0; i<param.m_levelInformations.Length; ++i )
		{
			m_levelInformations.list.Add( param.m_levelInformations[i] );
		}
	}
}
﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class EnemyDifficulter : MonoBehaviour {

	int  Level = 0;

	float cntNextLevelCooldownTime;


	[SerializeField]
	AnimationCurve enemyHealthMultiple = null;

	[SerializeField]
	AnimationCurve enemyAttackMultiple = null;

	[SerializeField]
	AnimationCurve enemyEmitNum = null;

	[SerializeField]
	AnimationCurve enemySpeedMultiple = null;

	[SerializeField]
	Transform emitEnterPoints = null;

	[SerializeField]
	float nextLevelCooldownTime = .0f;

	[SerializeField]
	Text textCurLevel = null;

	[SerializeField]
	Text textCntNextLevelCooldownTime = null;

	int cntEmitEnterPointNum;
	EmitEnterPoint cntEmitEnterPoint;

	int cntEnemyEmitNum = 0;

	void Start_Level()
	{
		cntEnemyEmitNum = (int)(enemyEmitNum.Evaluate((float)Level));

		SearchNextEmitPoint();

	}

	void SearchNextEmitPoint()
	{
		if (cntEmitEnterPointNum >= emitEnterPoints.childCount)
			return;

		if(cntEmitEnterPoint == null)
		{
			cntEmitEnterPoint = emitEnterPoints.GetChild(0).GetComponent<EmitEnterPoint>();
		}

		EmitEnterPoint next = emitEnterPoints.GetChild(cntEmitEnterPointNum + 1).GetComponent<EmitEnterPoint>();

		if(next.IsActiveEmit(Level))
		{
			cntEmitEnterPointNum++;
			cntEmitEnterPoint = next;
        }
	}

	public int GetEmitIndex()
	{
		return cntEmitEnterPoint.getEmitIndex();
    }

	public void SubEmitEnemyNum()
	{
		cntEnemyEmitNum--;
    }

	public bool IsCanEmit()
	{
		return cntEnemyEmitNum > 0;
    }


	void NextLevel()
	{
		Level++;
		Start_Level();
    } 
	void Start()
	{
		cntNextLevelCooldownTime = 20.0f;
    }

	void Update()
	{
		if (IsCanEmit() == false)
        {
			cntNextLevelCooldownTime -= Time.deltaTime;
			if(cntNextLevelCooldownTime < 0)
			{
				NextLevel();
                cntNextLevelCooldownTime = nextLevelCooldownTime;

			}
		}
		textCurLevel.text= Level.ToString();

		if(IsCanEmit() == true)
		{
			textCntNextLevelCooldownTime.transform.parent.gameObject.SetActive(false);
        }
		else
		{
			textCntNextLevelCooldownTime.transform.parent.gameObject.SetActive(true);
			textCntNextLevelCooldownTime.text = cntNextLevelCooldownTime.ToString("f1") + "秒";

		}
	}
}

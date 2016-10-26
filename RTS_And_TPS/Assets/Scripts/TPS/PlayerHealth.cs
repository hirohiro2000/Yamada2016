﻿using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField]
	float maxHp = 0;

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
			//ゲームオーバー
			GameSystemManager systemManager = FindObjectOfType<GameSystemManager>();
			if(systemManager != null)
			{
				systemManager.BeginGameOver();

			}

			Time.timeScale = .0f;
		}
	}
    public bool flg = false;
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log(gameObject.name);
		PlayerDamageSource source = collision.gameObject.GetComponentInParent<PlayerDamageSource>();
		if (source != null)
		{
			GiveDamage(source.damage);

            // 
            if(flg)
            GuideHitEffect.Add(source.user);

        }
	}

	public void OnGUI()
	{
		if(IsDeath())
		{
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.fontSize = (int)(Screen.height * 0.1f);
			style.alignment = TextAnchor.MiddleCenter;

			GUI.Label(new Rect(
				.0f, .0f, Screen.width, Screen.height),"GAME OVER", style);
		}
		GUIStyle style2 = new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = (int)(Screen.height * 0.03f);

		GUI.Label(new Rect(
			Screen.width * rect.x, Screen.height * rect.y,
			Screen.width * rect.width, Screen.height * rect.height),text + hp, style2);



	}
}

using UnityEngine;
using System.Collections;

public class EnemyKillCounter : MonoBehaviour {

	static  public int killCount;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = (int)(Screen.height * 0.05f);
        GUI.Label(new Rect(
			Screen.width * 0.7f, Screen.height * 0.8f,
			Screen.width * 0.4f, Screen.height * 0.4f), "倒した数: " + killCount, style);
	}
}

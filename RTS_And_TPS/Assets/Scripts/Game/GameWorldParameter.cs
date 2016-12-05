using UnityEngine;
using System.Collections;

[System.Serializable]
public class TPSPlayerParam
{
	public float Health = 20.0f;
	public float WalkSpeed = 7.0f;
	public float JumpPower = 0.45f;
	public float HoverPower = 0.5f;
	public float HoverSpeed = 0.15f;
	public float HoverTime = 1.02f;

	public float FallDamageHeight = 10.0f;

	public float StepPower = 7.0f;



}

public class GameWorldParameter : MonoBehaviour {

	private static GameWorldParameter _instance = null;

	public static GameWorldParameter instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<GameWorldParameter>();

				if(_instance == null)
				{
					GameObject obj = new GameObject("GameWorldParameter");
					obj.AddComponent<GameWorldParameter>();
					//Config_Dataがあればその親を代入する
					EditData_Config Config_Data = FindObjectOfType<EditData_Config>();
					if(Config_Data != null)
						obj.transform.parent = Config_Data.transform.parent;
				}
			}
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	public TPSPlayerParam TPSPlayer = new TPSPlayerParam();

	// Use this for initialization
	void Awake () {
		instance = this;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

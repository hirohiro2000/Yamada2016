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

	public float StepPower = 18.0f;


}

[System.Serializable]
public class RTSPlayerParam
{
	public float Health = 20.0f;
	public float WalkSpeed = 5.0f;

	//public float FallDamageHeight = 10.0f;

	public float ResourceCreateCostMultiple = 1.0f;
	public float ResourceLevelUpCostMultiple = 1.0f;
	public float ResourceBreakCostMultiple = 1.0f;

}

[System.Serializable]
public class EnemyParam
{
	public float EmitMultiple = 1.0f;
	public float HealthMultiple = 1.5f;
	public float WalkSpeedMultiple = 1.0f;
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
	public RTSPlayerParam RTSPlayer = new RTSPlayerParam();
	public EnemyParam Enemy = new EnemyParam();

	// Use this for initialization
	void Awake () {
		instance = this;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

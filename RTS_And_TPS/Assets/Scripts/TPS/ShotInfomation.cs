using UnityEngine;
using System.Collections;

public enum ShotInfoFlag //リストに含まれていたらtrue
{
	FullAutoShot, //おしっぱで自動発射
	ShotDirectionChangeFromScreenCenter, //画面中央に発射方向を補正する
	FLAG_MAX,
}

public enum ShotInfoValue //リストに含むと値をセットする
{
	CoolDownTime, //クールダウンタイム
	VisibleSimurateLineLength, //発射時の経路をシミュレート表示し、値は長さ
	Ammo,//一度に発射できる弾薬数(0なら無限)
	HavingAmmo,//他に所有できる弾薬数 
	ReloadTime,//リロード時間
	SingleReloadTime,//一発を込める時間(0なら未設定)
	VALUE_MAX,
}

[System.Serializable]
public class ShotInfoValueParam
{
	public ShotInfoValue type;
	public float value = .0f;
}

[System.Serializable]
public class ShotInfoFlagParam
{
	public ShotInfoFlag type;
}

[System.Serializable]
public class ShotInfoValueParamReorderableList : ReorderableList<ShotInfoValueParam> { }

[System.Serializable]
public class ShotInfoFlagReorderableList : ReorderableList<ShotInfoFlagParam> { }



public class ShotInfomation : MonoBehaviour {

	[ReorderableList(new int[] { 300 })]
	public ShotInfoFlagReorderableList shot_info_flags;

	[ReorderableList(new int[] { 300, 50 })]
	public ShotInfoValueParamReorderableList shot_info_values;

    public string m_seFileName = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

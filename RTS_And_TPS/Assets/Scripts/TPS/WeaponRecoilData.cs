using UnityEngine;
using System.Collections;

public class WeaponRecoilData : MonoBehaviour {

	public float armCriticalRateIncrement; //一発ごとの腕力限界度増加量
	public float[] baseReticle; //姿勢ごとのレティクル基本値
	public float baseRecoil; //リコイル基本値
	public float recoveryTimeWithRecoil; //リコイルの回復時間
	public float maxValueWithUpperRecoil; //上方向リコイルの最大値
	public float maxValueWithHorizontalRecoil; //横方向リコイルの最大値
	// Update is called once per frame
	void Update () {
	
	}
}

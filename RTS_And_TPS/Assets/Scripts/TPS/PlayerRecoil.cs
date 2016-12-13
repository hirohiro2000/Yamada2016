using UnityEngine;
using System.Collections;


public class PlayerRecoil : MonoBehaviour {

	public float recoveryTimeWithArmAndReticle = 0.5f; //腕力限界度とレティクルが戻る時間
	float cntMaxArmCriticalRate = .0f; //現在の最大腕力限界度
	public float cntArmCriticalRate = .0f; //現在の腕力限界度

	float cntMaxReticleMultiple = .0f; //現在の最大レティクルブレ倍率
	public float cntReticleMultiple = .0f; //現在のレティクルブレ倍率
	float recoveryRateWithArmAndReticle = 1.0f; //腕力限界度とレティクルの回復率
	[SerializeField]
	AnimationCurve reticleIncrementCurve = null; //レティクル増加量カーブ
	[SerializeField]
	AnimationCurve recoilIncrementCurve = null; //リコイル増加量カーブ
	Vector2 beforeRecoil = Vector2.zero; //発射前のリコイル量
	Vector2 cntMaxRecoil = Vector2.zero; //現在の最大リコイル量
	public Vector2 cntRecoil = Vector2.zero; //現在のリコイル量
	float recoveryRateWithRecoil = 1.0f; //リコイルの回復率
	public Vector2 cntDisplayRecoiil = Vector2.zero; //現在の表示用リコイル量
	public float recoilChangeTime = 0.05f;//リコイルが変化する時間
	float changeRateWithRecoil = 1.0f; //リコイルの変化率

	public WeaponRecoilData holdingWeapon;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//DeltaTimeで回復

		//腕力限界度・レティクル回復
		{
			float recovery = Time.deltaTime / recoveryTimeWithArmAndReticle;
			recoveryRateWithArmAndReticle += recovery;

			if (recoveryRateWithArmAndReticle > 1.0f)
				recoveryRateWithArmAndReticle = 1.0f;

			//回復率を反映
			cntArmCriticalRate = cntMaxArmCriticalRate * (1.0f - recoveryRateWithArmAndReticle);
			cntReticleMultiple = cntMaxReticleMultiple * (1.0f - recoveryRateWithArmAndReticle);

		}


		//リコイル回復
		{
			float recoveryTime;
			if (holdingWeapon != null)
				recoveryTime = holdingWeapon.recoveryTimeWithRecoil;
			else
				recoveryTime = 5.0f;

			float recovery = Time.deltaTime / recoveryTime;
			recoveryRateWithRecoil += recovery;

			if (recoveryRateWithRecoil > 1.0f)
				recoveryRateWithRecoil = 1.0f;

			//回復率を反映
			cntRecoil = cntMaxRecoil * (1.0f - recoveryRateWithRecoil) * (1.0f - recoveryRateWithRecoil);

		}
		//表示用リコイルの更新
		{
			float recovery = Time.deltaTime / recoilChangeTime;
			changeRateWithRecoil += recovery;

			if (changeRateWithRecoil > 1.0f)
				changeRateWithRecoil = 1.0f;

			//変化率を反映
			cntDisplayRecoiil = beforeRecoil +  (cntRecoil - beforeRecoil )* changeRateWithRecoil;
		}

	}

	public void Shot()
	{
		if(holdingWeapon == null)
		{
			//無ければ無視
			return;
		}
		//回復率をリセット
		recoveryRateWithArmAndReticle = .0f;
		recoveryRateWithRecoil = .0f;
		changeRateWithRecoil = .0f;

		//リコイル上昇
		{
			Vector2 work = cntRecoil;

			//縦上昇
			work.y += holdingWeapon.baseRecoil * recoilIncrementCurve.Evaluate(cntArmCriticalRate);

			//横方向のリコイルを取得
			float horizontalRecoil;
			{
				horizontalRecoil = work.y - holdingWeapon.maxValueWithUpperRecoil;

				//上方向の上限を超えていなければ0
				if (horizontalRecoil < .0f)
					horizontalRecoil = .0f;
				else//上方向の上限を超えていなければ縦のリコイル量を制限する
				{
					work.y = holdingWeapon.maxValueWithUpperRecoil;
				}
			}
			//左右は50:50の割合で
			if(Random.Range(0,2) == 0)
			{
				work.x += horizontalRecoil;
			}
			else
			{
				work.x -= horizontalRecoil;
			}
			//横リコイル量が余ったら折り返す
			float surPlusRecoil = Mathf.Abs(work.x) - holdingWeapon.maxValueWithHorizontalRecoil;
			if(surPlusRecoil > .0f)
			{
				//surPlusRecoil *= work.x / Mathf.Abs(work.x);
				
				//左端を基準とする(右端ならば左端から+1されてるとする)
				float surPlusMultiple = surPlusRecoil / (holdingWeapon.maxValueWithHorizontalRecoil * 2.0f);
				if (work.x > .0f)
					surPlusMultiple += 1;
				surPlusMultiple -= (int)surPlusMultiple / 2 * 2;

				if(surPlusMultiple > 1.0f)
				{
					surPlusMultiple -= 1.0f;
					work.x = holdingWeapon.maxValueWithHorizontalRecoil - (holdingWeapon.maxValueWithHorizontalRecoil * 2.0f * surPlusMultiple);
				}
				else
				{
					work.x = -holdingWeapon.maxValueWithHorizontalRecoil + (holdingWeapon.maxValueWithHorizontalRecoil * 2.0f * surPlusMultiple);
				}
			}

			//最終結果を代入
			beforeRecoil = cntRecoil;
			cntRecoil = work;
			cntDisplayRecoiil = beforeRecoil;
            cntMaxRecoil = cntRecoil; 
		}
		//レティクル上昇
		{
			cntReticleMultiple += reticleIncrementCurve.Evaluate(cntArmCriticalRate);
			cntMaxReticleMultiple = cntReticleMultiple;
		}
		//腕力限界度上昇
		{
			cntArmCriticalRate += holdingWeapon.armCriticalRateIncrement;
			cntMaxArmCriticalRate = cntArmCriticalRate;
		}
	}

	public Vector3 GetReticleVector(Vector3 forward)
	{
		float reticle = GetReticle();

		//レティクルをベクトルに変換
		int loop = 3;
		Vector2 reticleVector2 = Vector2.zero;
		for (int i = 0; i < loop; i++)
		{
			reticleVector2.x += Random.Range(-reticle, reticle);
			reticleVector2.y += Random.Range(-reticle, reticle);
		}
		reticleVector2 /= (float)loop;

		Vector3 reticleVector =  Quaternion.Euler(reticleVector2.x, reticleVector2.y, .0f) * Vector3.forward;

		return Quaternion.LookRotation(forward) * reticleVector;
	}

	public float GetReticle()
	{
		if (holdingWeapon == null)
			return .0f;
		//発射前にレティクルを取得
		float reticleMultiple = cntReticleMultiple;
		float reticle = holdingWeapon.baseReticle[0];

		return reticle + reticle * 0.3f * reticleMultiple + reticleMultiple;

	}

	void OnGUI()
	{

	}

}

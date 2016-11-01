using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*弱点システムの仕様

	攻撃スクリプト(複数所有可能)
	基本ダメージを持っている
	消すオブジェクトを指定することで攻撃後に削除処理ができる
	リストを持っている
	enumで属性を設定
	攻撃スクリプトはトリガーコライダーにアタッチ
	攻撃スクリプトをアタッチされているオブジェクトのコライダーは
	他のスクリプトで利用してはいけない
	(特殊な処理(毎フレームenabledを入れなおしている)を入れているため)

	弱点スクリプト(複数所有可能)
	アタッチするオブジェクトは自由(コライダーから探して無ければ親から再帰的に探す)
	リストを持っている
	enumで属性を設定
	リストの中で同じ属性同士の値をかけ合わせて、
	最終的に攻撃のダメージ量をかける
	リストの中の属性と対応しなかった場合は無衝突と判定される

	ダメージバンク(rigidbodyと同じオブジェクトにアタッチ)
	ダメージをため込みます

	DamageResultに含まれたAttackPointListはnullが返ってくる可能性があります
 */


public class DamageParam
{
	public WeakPointType type;
	public float damage;

	public DamageParam(float damage,WeakPointType type)
	{
		this.damage = damage;
		this.type = type;
	}
}					   

public class DamageResult
{
	float baseDamage = .0f;
	AttackPointList attackedObject;
	List<DamageParam> damageParamList = new List<DamageParam>();

	public DamageResult(AttackPointList attackedObject)
	{
		this.baseDamage = attackedObject.baseAttackPoint;
		this.attackedObject = attackedObject;
	}

	public List<DamageParam> GetDamageParamList()
	{
		return damageParamList;
	}

	public float GetTotalDamage()
	{
		float damage = baseDamage;
		foreach (DamageParam param in damageParamList)
		{
			damage *= param.damage;
		}
		return damage;
	}

	public float GetBaseDamage()
	{
		return baseDamage;
	}

	public void PushBack_Damage(float damage, WeakPointType type)
	{
		damageParamList.Add(new DamageParam(damage, type));
	}

	//攻撃者をDestroyします(設定されていれば)
	public void AttackedObject_SendDestroy()
	{
		if(attackedObject != null)
		{
			attackedObject.CallDestroy();
		}
	}

	public AttackPointList GetAttackedObject()
	{
		return attackedObject;
	}


}







public class DamageBank : MonoBehaviour {

	public List<DamageResult> damageList = new List<DamageResult>();
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update()
	{
		//foreach (DamageResult result in damageList)
		//{
		//	GetElementDamageResult getElementDamageResult = new GetElementDamageResult;
		//	getElementDamageResult(result);
		//}
		//現在自動消去
		damageList.Clear();
	}

	public delegate void AdvancedDamaged(DamageResult damageResult,Vector3 contactPoint);

	public event AdvancedDamaged AdvancedDamagedCallback = null;

	public delegate void Damaged(float damage);

	public event Damaged DamagedCallback = null;

	public void RecieveDamage(AttackPointList atk ,Collider damagedCollider)
	{
		WeakPointList[] weaks;
		Transform searchTransform =  damagedCollider.transform;

		//弱点スクリプトを検索
        while(true)
		{
			weaks = searchTransform.GetComponents<WeakPointList>();
			if (weaks != null)
				break;
			if (searchTransform.parent == null)
				break;

			searchTransform = searchTransform.parent;
		}

		if(weaks != null)
		{
			//Debug.Log("damaged:" + searchTransform.gameObject.name + "   attacked:" + atk.gameObject.name);

			//衝突を取得しなかった場合、非衝突とみなす
			bool isHit = false;

			//ダメージ計算前にデリゲートします
			AttackPointList copyAtk;
			if(atk.BeforeCalcDamegeCallBack != null)
			{
				//設定していたら複製
				copyAtk = new AttackPointList(atk);
				atk.BeforeCalcDamegeCallBack(ref copyAtk, damagedCollider.transform.position);
            }
			else
			{
				copyAtk = atk;
            }

			foreach(WeakPointList weak  in weaks)
			{
				//ダメージ計算前にデリゲートします
				WeakPointList copyWeak;
				if (weak.BeforeCalcDamegeCallBack != null)
				{
					//設定していたら複製
					copyWeak = new WeakPointList(weak);
					weak.BeforeCalcDamegeCallBack(ref copyWeak, damagedCollider.transform.position);
				}
				else
				{
					copyWeak = weak;
				}
				//ダメージを取得(衝突扱いでなければ取得できない)
				DamageResult damageResult = GetDamageResult(copyAtk, copyWeak);
				if(damageResult != null)
				{
					isHit = true;
					damageList.Add(damageResult);
					if(AdvancedDamagedCallback != null)
						AdvancedDamagedCallback(damageResult,damagedCollider.ClosestPointOnBounds(atk.transform.position));
					if (DamagedCallback != null)
						DamagedCallback(damageResult.GetTotalDamage());
					if (atk.GiveDamagedCallBack != null)
					{
						//エラー回避
						DamageBank i = this;
                        atk.GiveDamagedCallBack(ref i, damageResult);
					}

					if (weak.HitedCallBack != null)
					{
						//エラー回避
						WeakPointList temp = weak;
						temp.HitedCallBack(ref temp, damagedCollider.transform.position);
                    }
				}
			}


			//衝突していたら攻撃スクリプトに破壊命令を出す
			if (isHit == true)
			{
				if(atk.HitedCallBack != null)
					atk.HitedCallBack(ref atk, damagedCollider.transform.position);



				atk.CallDestroy();
			}
		}


	}

	DamageResult GetDamageResult(AttackPointList atk, WeakPointList weak)
	{
		bool isHit = false;
		DamageResult damageResult = new DamageResult(atk);
		foreach (WeakPointParam weakParam in weak.weak_lists.list)
		{
			//検索するタイプを決定
			WeakPointType searchType = weakParam.type;

			//同じタイプを検索
			foreach (WeakPointParam attackParam in atk.attack_list.list)
			{
				if(searchType == attackParam.type)
				{
					//衝突した
					isHit = true;
					float damage = attackParam.multiple * weakParam.multiple;
					damageResult.PushBack_Damage(damage, searchType);
				}

			}
		}
		//Typeが一つも適合しなかったら非接触とする
		if(isHit == false)
		{
			return null;
		}

		return damageResult;


	}
}


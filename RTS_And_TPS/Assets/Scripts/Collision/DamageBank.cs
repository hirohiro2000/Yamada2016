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

 */


public struct DamageParam
{
	public WeakPointType type;
	public float damage;

	public DamageParam(float damage,WeakPointType type)
	{
		this.damage = damage;
		this.type = type;
	}
}		

//衝突時の情報
public class CollisionInfo
{
	Transform _attackedObject;  //攻撃スクリプトを持つオブジェクト
	Transform _damagedObject;	//DamageBankを持つオブジェクト

	Transform _weakObject;		//弱点スクリプトを持つオブジェクト
	Collider _damagedCollider;	//ダメージを受けたコライダー
	Vector3 _contactPoint;      //衝突点

	public Transform attackedObject
	{
		get
		{
			return _attackedObject;
		}
	}

	public Transform damagedObject
	{
		get
		{
			return _damagedObject;
		}
	}

	public Transform weakObject
	{
		get
		{
			return _weakObject;
		}
	}

	public Collider damagedCollider
	{
		get
		{
			return _damagedCollider;
		}
	}

	public Vector3 contactPoint
	{
		get
		{
			return _contactPoint;
		}
	}


	public CollisionInfo(WorkDamageResults workDamageResults, Transform damagedObject)
	{
		_attackedObject = workDamageResults.atk.transform;
		_damagedObject = damagedObject;
		_weakObject = workDamageResults.weaksObject;     
		_damagedCollider = workDamageResults.damagedCollider;
		_contactPoint = workDamageResults.damagedCollider.ClosestPointOnBounds(_attackedObject.position);
	}
}
//外部公開用クラス
public class DamageResult
{
	float baseDamage = .0f;
	//AttackPointList attackedObject;
	//WeakPointList weak;
	List<DamageParam> damageParamList = new List<DamageParam>();

	public DamageResult(AttackPointList attackedObject)
	{
		this.baseDamage = attackedObject.baseAttackPoint;
		//this.attackedObject = attackedObject;
	}

	//public DamageResult(AttackPointList copyedAtk,WeakPointList damagedWeak)
	//{
	//	this.baseDamage = copyedAtk.baseAttackPoint;
	//	this.weak = damagedWeak;
	//}

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
	//public void AttackedObject_SendDestroy()
	//{
	//	if(attackedObject != null)
	//	{
	//		attackedObject.CallDestroy();
	//	}
	//}

	//public AttackPointList GetAttackedObject()
	//{
	//	return attackedObject;
	//}


}
//内部作業用クラス
public class WorkDamageResult : DamageResult
{
	public WorkDamageResult(AttackPointList attackedObject) :base(attackedObject)
	{

	}
	public WeakPointList weak;
}



public class WorkDamageResults
{
	public WorkDamageResults(AttackPointList atk, Transform weaksObject,Collider damagedCollider,Transform damagedObject)
	{
		this.atk = atk;
		this.weaksObject = weaksObject;
		this.damagedCollider = damagedCollider;
		info = new CollisionInfo(this, damagedObject);
	}

	public bool IsSame(WorkDamageResults target)
	{
		return (atk == target.atk) && (weaksObject == target.weaksObject);
	}
	public float GetAllTotalDamage()
	{
		float damage = .0f;
		foreach (WorkDamageResult element in results)
		{
			damage += element.GetTotalDamage();
		}

		return damage;
	}

	public AttackPointList atk = null;
	public Transform weaksObject = null; //weakを持つオブジェクト
	public List<WorkDamageResult> results = new List<WorkDamageResult>();
	public Collider damagedCollider = null;
	public bool	isDelete = false; //削除フラグ
	public CollisionInfo info = null;
}







public class DamageBank : MonoBehaviour {

	public enum RecieveDamageType//採用するダメージの基準
	{
		Default,		//デフォルト
		HighestTotalDamage,  //最も大きい合計ダメージ
		LowestTotalDamage,   //最も低い合計ダメージ
	};

	List<WorkDamageResults> workDamageList = new List<WorkDamageResults>();

	public RecieveDamageType recieveDamageType = RecieveDamageType.Default;

	bool ListClearRequest = false; //リストの削除を要求

	//FixedUpdateで検査されたリストの数
	int CheckedListNum = 0;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update()
	{
		//現在自動消去
		ListClearRequest = true;
	}

	public delegate void AdvancedDamaged(DamageResult damageResult,CollisionInfo info);

	public event AdvancedDamaged AdvancedDamagedCallback = null;

	public delegate void Damaged(float damage);

	public event Damaged DamagedCallback = null;

	delegate bool LeaveDamageCheck(WorkDamageResults leave, WorkDamageResults compare);


	public void RecieveDamage(AttackPointList atk ,Collider damagedCollider)
	{
		WeakPointList[] weaks;
		Transform searchTransform =  damagedCollider.transform;

		//弱点スクリプトを検索
        while(true)
		{
			weaks = searchTransform.GetComponents<WeakPointList>();
			if (weaks.Length > 0)
				break;
			if (searchTransform.parent == null)
				break;

			searchTransform = searchTransform.parent;
		}

		if(weaks.Length > 0)
		{
			//弱点スクリプトが見つかった時点でWorkDamageResultsを作成
			WorkDamageResults workDamageResults = new WorkDamageResults(atk, searchTransform,damagedCollider,transform);


			//かぶっていれば無効
			if(IsAttackAvaliable(workDamageResults) == false)
			{
				return;
			}

			//衝突を取得しなかった場合、非衝突とみなす
			bool isHit = false;

			//ダメージ計算前にデリゲートします
			AttackPointList copyAtk;
			if(atk.BeforeCalcDamegeCallBack != null)
			{
				//設定していたら複製
				copyAtk = new AttackPointList(atk);
				atk.BeforeCalcDamegeCallBack(ref copyAtk, workDamageResults.info);
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
					weak.BeforeCalcDamegeCallBack(ref copyWeak, workDamageResults.info);
				}
				else
				{
					copyWeak = weak;
				}
				//ダメージを取得(衝突扱いでなければ取得できない)
				WorkDamageResult damageResult = GetDamageResult(copyAtk, copyWeak);
				if(damageResult != null)
				{
					isHit = true;
					//作業用としてweakを代入
					damageResult.weak = weak;
					workDamageResults.results.Add(damageResult);
				}
			}


			//衝突していたらリストに追加する
			if (isHit == true)
			{
				workDamageList.Add(workDamageResults);
			}
		}


	}

	//その攻撃は有効か(ここで攻撃が確定されるわけではない)
	bool IsAttackAvaliable(WorkDamageResults createResults)
	{
		//検査された物の中でatkが一致すれば無効とする(すでに衝突したものとするため)
		for (int i = 0; i < CheckedListNum; i++)
		{
			if (workDamageList[i].atk == createResults.atk)
				return false;
		}

		//検査される前の物の中でatkとweaksObjectが一致すれば無効とする(取得できるダメージ結果が同じため)
		for (int i = CheckedListNum; i < workDamageList.Count; i++)
		{
			if (workDamageList[i].IsSame(createResults))
				return false;
		}

		return true;
	}

	//検査
	void FixedUpdate()
	{
		DamageListCheckAndExecute();

		if(ListClearRequest == true) //要求を受け入れる
		{
			ListClearRequest = false;
			CheckedListNum = 0;
			workDamageList.Clear();
		}
	}

	//目的
	//一つの攻撃スクリプトから受けるダメージを一つに絞れるように
	//絞る基準はダメージ量
	void DamageListCheckAndExecute()
	{
		//検査されていないダメージがなかったら処理しない
		if (CheckedListNum == workDamageList.Count)
			return;

		//同じAttackPointListを持つダメージをまとめるインデックスリストのリストを作成
		List<List<int>> checkList = new List<List<int>>();


		//残す基準を設定
		LeaveDamageCheck[] leaveDamageChecks =
		{
			CheckHighestTotalDamage,	//Default
			CheckHighestTotalDamage,	//HighestTotalDamage
			CheckLowestTotalDamage,		//LowestTotalDamage
		};
		LeaveDamageCheck leaveDamageCheck = leaveDamageChecks[(int)recieveDamageType];

		//検査されていないダメージのみ検査
		for (int i = CheckedListNum; i < workDamageList.Count; i++)
		{
			//AttackPointListで振り分けてインデックスをリストに追加する
			WorkDamageResults result = workDamageList[i];
			bool Added = false;
			for (int listIndex = 0; listIndex < checkList.Count; listIndex++)
			{
				int checkNum = checkList[listIndex][0];

				//一致すれば追加
				if (workDamageList[checkNum].atk == result.atk)
				{
					checkList[listIndex].Add(i);
					Added = true;
					break;
				}

			} 
			//一致するリストが無ければ新しく作ってそのリストに追加する
			if(Added == false)
			{
				List<int> list = new List<int>();
				list.Add(i);
				checkList.Add(list);
			}
		}//リスト追加完了

		//一つの攻撃スクリプトから受けるダメージを一つになるように絞る
		for (int i = 0; i < checkList.Count; i++)
		{
			//もしかぶっているAttackPointListが無ければ無条件で残す
			if (checkList[i].Count == 1)
				continue;

			//残すダメージの初期設定
			int index = checkList[i][0];
			WorkDamageResults leaveDamage = workDamageList[index];
			for (int k = 1; k < checkList[i].Count; k++)
			{
				index = checkList[i][k];
				WorkDamageResults CompareDamage = workDamageList[index];
				WorkDamageResults DeleteDamage = null;
				//比較開始(基準で変化)
				if(leaveDamageCheck(leaveDamage,CompareDamage))
				{
					DeleteDamage = leaveDamage;
					leaveDamage = CompareDamage;
					
				}
				else
				{
					DeleteDamage = CompareDamage;
				}
				//削除するダメージは印としてisDeleteをtrueにする
				DeleteDamage.isDelete = true;
			}
		}//比較終了

		//印のついたダメージを消す(めんどくさかったので検査済みの物も見ている)
		workDamageList.RemoveAll(d => d.isDelete == true);

		//すべての検査が終了したのでここからコールバック関数を実行する
		for (int i = CheckedListNum; i < workDamageList.Count; i++)
		{//階層atk

			WorkDamageResults workDamageResults = workDamageList[i];

			foreach (WorkDamageResult result in workDamageResults.results)
			{//階層weak

				//その場しのぎ
				if (workDamageResults.atk == null)
					continue;

				if ( DamagedCallback != null )
					DamagedCallback(result.GetTotalDamage());
				if (AdvancedDamagedCallback != null)
					AdvancedDamagedCallback(result, workDamageResults.info);

				if (result.weak.HitedCallBack != null)
				{
					result.weak.HitedCallBack(ref result.weak, result,workDamageResults.info);
				}

			}
			if (workDamageResults.atk.HitedCallBack != null)
				workDamageResults.atk.HitedCallBack(ref workDamageResults.atk,  workDamageResults.info);
			//その場しのぎ
			if(workDamageResults.atk != null)
			{
				workDamageResults.atk.CallEmitObjectOnHitPoint(workDamageResults.info.contactPoint);
				workDamageResults.atk.Hited();
			}
		}
		//検査済みとして値を更新
		CheckedListNum = workDamageList.Count;
	}

	LeaveDamageCheck CheckLowestTotalDamage = (WorkDamageResults leave, WorkDamageResults compare) =>
	{
		return (leave.GetAllTotalDamage() > compare.GetAllTotalDamage());
	};

	LeaveDamageCheck CheckHighestTotalDamage = (WorkDamageResults leave, WorkDamageResults compare) =>
	{
		return (leave.GetAllTotalDamage() < compare.GetAllTotalDamage());
	};


	WorkDamageResult GetDamageResult(AttackPointList atk, WeakPointList weak)
	{
		bool isHit = false;
		WorkDamageResult damageResult = new WorkDamageResult(atk);
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


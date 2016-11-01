using UnityEngine;
using System.Collections;


//シンプルに乗算します
//無い属性が入っていれば追加します
public class UpgradeModule : MonoBehaviour {

	public float BaseAttackPoint = 1.0f;

	[SerializeField, ReorderableList(new int[] { 100, 100 })]
	public WeakPointParamReorderableList powerUp_list = null;

	AttackPointList attackPointList = null;



	// Use this for initialization
	void Start () {
		attackPointList = GetComponent<AttackPointList>();

		attackPointList.BeforeCalcDamegeCallBack = (ref AttackPointList copy, Vector3 point) =>
		{
			//BaseDamageを乗算
			copy.baseAttackPoint *= BaseAttackPoint;
			foreach(WeakPointParam upgrade in powerUp_list.list)
			{
				bool isFound = false;
				foreach (WeakPointParam param in copy.attack_list.list)
				{
					//同じタイプがあれば乗算します
					if(upgrade.type == param.type)
					{
						isFound = true;
                        param.multiple *= upgrade.multiple;
						break;
                    }
				}

				//見つからなければ追加します
				if(isFound == false)
				{
					copy.attack_list.list.Add(upgrade);
                }
				
            }

        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent((typeof(CapsuleCollider)))]
// 前回との位置の差分をコライダーの当たり判定に反映する(前方向のみ)
public class TriggerCollisionDetection : MonoBehaviour
{

	Vector3 beforePos;
	Vector3 defCenter;
	CapsuleCollider col;

	void Awake ()
	{
		col = GetComponent<CapsuleCollider>();
		beforePos = transform.position;
		defCenter = col.center;
	}
	
	void FixedUpdate ()
	{
		Vector3 sub = transform.position - beforePos;
		float len = Vector3.Dot(sub, transform.forward) / transform.localScale.z;

		col.height = Mathf.Abs(len);
		col.center = new Vector3(defCenter.x, defCenter.y, defCenter.z - len / 2.0f);
		beforePos = transform.position;
	}
}

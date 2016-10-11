using UnityEngine;
using System.Collections;

public class TPS_EnemyAttacker : MonoBehaviour {
	public float coolDown;
	float cntCooldown;

	public float searchRadius;

	bool isAttack;

	[SerializeField]
	GameObject bullet;

	[SerializeField]
	Transform attackPoint;

	[SerializeField]
	Transform rotatePoint;

	EnemyAtkTargetManager _enemyAtkTargetManager;
	EnemyAtkTargetManager enemyAtkTargetManager
	{
		get
		{
			if(_enemyAtkTargetManager == null)
			{
				_enemyAtkTargetManager = FindObjectOfType<EnemyAtkTargetManager>();
			}
			return _enemyAtkTargetManager;
		}

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		isAttack = false;
		cntCooldown -= Time.deltaTime;
		Transform target = enemyAtkTargetManager.getNearestTarget(transform.position, searchRadius, false);
		if(target != null)
		{
			isAttack = true;

			if(rotatePoint != null)
			{
				Vector3 direction = target.position - rotatePoint.position;
				direction.y = .0f;
				direction.Normalize();

				Vector3 forward = rotatePoint.forward;
				forward.y = .0f;
				forward.Normalize();

				//角度差を計算
				float dot = Vector3.Dot(forward, direction);
				float radian = Mathf.Acos(dot);
				if (Vector3.Cross(forward, direction).y < 0)
					radian = -radian;

				radian *= 180.0f / Mathf.PI;

				rotatePoint.rotation *= Quaternion.AngleAxis(radian, Vector3.up);


			}

			if (cntCooldown < .0f)
			{
				cntCooldown = coolDown;
			}
		}

	}
}

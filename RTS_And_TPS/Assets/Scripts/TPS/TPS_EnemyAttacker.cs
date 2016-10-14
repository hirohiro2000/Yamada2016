using UnityEngine;
using System.Collections;

public class TPS_EnemyAttacker : MonoBehaviour {
	public float coolDown;
	float cntCooldown;

	public float searchRadius;

	public float ShotSpeed;

	public bool isAttack;

	[SerializeField]
	GameObject bullet;

	[SerializeField]
	Transform[] attackPoint;

	[SerializeField]
	Transform rotatePoint;

	[SerializeField]
	Transform rotatePointY;

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
				Vector3 direction2D = direction;
                direction2D.y = .0f;
				direction2D.Normalize();

				Vector3 forward = rotatePoint.forward;
				forward.y = .0f;
				forward.Normalize();

				//角度差を計算
				float dot = Vector3.Dot(forward, direction2D);
				float radian = Mathf.Acos(dot);
				if (Vector3.Cross(forward, direction2D).y < 0)
					radian = -radian;

				radian *= 180.0f / Mathf.PI;

				Quaternion q = Quaternion.AngleAxis(radian, Vector3.up);

				if (!float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w))
					rotatePoint.rotation *= q;

				if (rotatePointY != null)
				{
					direction.Normalize();
					rotatePointY.rotation = Quaternion.LookRotation(direction, Vector3.up);
                }

            }

			//発射
			if (cntCooldown < .0f)
			{
				cntCooldown = coolDown;

				foreach(Transform firePoint in attackPoint)
				{
					GameObject emit = Instantiate(bullet, firePoint.position, Quaternion.LookRotation(firePoint.forward)) as GameObject;
					emit.GetComponent<Rigidbody>().velocity = firePoint.forward * ShotSpeed;
					//自動消去設定
					Destroy(emit, 10.0f);

				}
			}
		}

	}
}

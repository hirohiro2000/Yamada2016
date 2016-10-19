using UnityEngine;
using System.Collections;

public class TPSShotController : MonoBehaviour {

	[SerializeField]
	Transform[] firePoints;

	[SerializeField]
	GameObject emitter;

	[SerializeField]
	float shotCooldown;

	int fireCnt;
	float cntCoolDown;

	[SerializeField]
	float shotDistance;

	//bool Targeted;

	float AimDistance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		cntCoolDown -= Time.deltaTime;
		//そのままの前方向
		Transform firePoint = firePoints[fireCnt];
		Vector3 targetPoint = firePoints[fireCnt].position + firePoints[fireCnt].forward;
		//画面中央に飛ばす
		{
			//画面中央を取得(Near,Far)
			Vector3 shotPointNear, shotPointFar;

			//発射点の画面震度を取得
            float worldnearZ = Camera.main.WorldToViewportPoint(firePoint.position).z;

			//画面右上 = (1,1)
			Vector2 shotViewportPos = new Vector2(0.5f, 0.5f);
			shotPointNear = Camera.main.ViewportToWorldPoint(new Vector3(shotViewportPos.x, shotViewportPos.y, worldnearZ));
			shotPointFar = Camera.main.ViewportToWorldPoint(new Vector3(shotViewportPos.x, shotViewportPos.y, Camera.main.farClipPlane));
			Vector3 direction = (shotPointFar - shotPointNear).normalized;
			Ray ray = new Ray(shotPointNear, direction);

			RaycastHit hit;
			if(Physics.Raycast(ray,out hit, 1000.0f))
			{
				AimDistance = hit.distance;
				if (AimDistance <= shotDistance)
				{
					targetPoint = hit.point;
				}

            }
        }

        if (Input.GetMouseButton(0))
		{
			if(cntCoolDown < 0)
			{
				cntCoolDown = shotCooldown;
				
				Shot(firePoint, targetPoint);
				fireCnt++;
				if(firePoints.Length <= fireCnt)
				{
					fireCnt = 0;
                }

            }
		}
	
	}

	void Shot(Transform firePoint,Vector3 target)
	{
		Vector3 forward;
		forward = (target - firePoint.position).normalized;
		GameObject emit = Instantiate(emitter, firePoint.position, Quaternion.LookRotation(forward)) as GameObject;
		float speed = 200.0f;
		emit.GetComponent<Rigidbody>().velocity = forward * speed;

		//距離に合わせて寿命を設定
		emit.GetComponent<TPSNormalGun>().Shot_Start(shotDistance / speed);

	}

	void OnGUI()
	{
		//照準の描画
		GUIStyle style = new GUIStyle();
		if(AimDistance <= shotDistance)
		{
			style.normal.textColor = new Color(1.0f,0.5f,.0f);
		}
		else
		{
			style.normal.textColor = Color.white;
		}
		style.fontSize =(int)( Screen.height * 0.05f);
		style.alignment = TextAnchor.MiddleCenter;

		GUI.Label(new Rect(.0f, .0f, Screen.width, Screen.height), "+", style);
		style.fontSize = (int)(Screen.height * 0.02f);
		style.alignment = TextAnchor.MiddleLeft;
		GUI.Label(new Rect(Screen.height * 0.02f + Screen.width  * 0.5f  , Screen.height * 0.02f, Screen.width, Screen.height), AimDistance.ToString(), style);


	}

}

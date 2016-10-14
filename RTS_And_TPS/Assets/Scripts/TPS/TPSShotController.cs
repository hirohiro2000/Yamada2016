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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		cntCoolDown -= Time.deltaTime;
        if (Input.GetMouseButton(0))
		{
			if(cntCoolDown < 0)
			{
				cntCoolDown = shotCooldown;
				//そのままの前方向
				Transform firePoint = firePoints[fireCnt];
				Vector3 targetPoint = firePoints[fireCnt].position + firePoints[fireCnt].forward;
				//画面中央に飛ばす
				{
					//画面中央を取得(Near,Far)
					Vector3 shotPointNear, shotPointFar;
					//画面右上 = (1,1)
					Vector2 shotViewportPos = new Vector2(0.5f, 0.5f);
					shotPointNear = Camera.main.ViewportToWorldPoint(new Vector3(shotViewportPos.x, shotViewportPos.y, .0f));
					shotPointFar = Camera.main.ViewportToWorldPoint(new Vector3(shotViewportPos.x, shotViewportPos.y, 1.0f));
					Vector3 direction = (shotPointFar - shotPointNear).normalized;
					Ray ray = new Ray(shotPointNear, direction);

					RaycastHit hit;
					if(Physics.Raycast(ray,out hit, 10000.0f))
						targetPoint = hit.point;


                }

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
		emit.GetComponent<Rigidbody>().velocity = forward * 60.0f;
		emit.GetComponent<TPSNormalGun>().Shot_Start(2.0f);

	}

	void OnGUI()
	{
		//照準の描画
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize =(int)( Screen.height * 0.05f);
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(.0f, .0f, Screen.width, Screen.height), "+", style);


	}

}

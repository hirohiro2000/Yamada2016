
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TPSRotationController : MonoBehaviour
{
	[SerializeField]
	Transform YAxisRotater = null;

	[SerializeField]
	Transform XAxisRotater = null;

	[SerializeField, Range(1.0f, 100.0f)]
	float YAxisRotSpeed = 50.0f;

	[SerializeField, Range(1.0f, 100.0f)]
	float XAxisRotSpeed = 50.0f;

	[SerializeField]
	float XAxisMaxangle = 70.0f;

	[SerializeField, Range(1.0f, 100.0f)]
	float XaxisMinAngle = -70.0f;

	//[SerializeField, TooltipAttribute("一秒間で戻る角度")]
	//float RecoilDampRate = 5.0f;

	//[SerializeField, TooltipAttribute("リコイルの最大角度")]
	//float MaxRecoil = 10.0f;

	//float cntRecoil = .0f;

	float xAxisAngle = .0f;
	float yAxisAngle = .0f;

	private PlayerRecoil playerRecoil = null ;

    private NetworkIdentity m_rIdentity =   null;

	// Use this for initialization
	void Start()
	{
		yAxisAngle = YAxisRotater.localRotation.eulerAngles.y;
		m_rIdentity =   GetComponent< NetworkIdentity >();
		playerRecoil = GetComponent<PlayerRecoil>();
	}

	void XAxisRotate(float radius)
	{
		//XAxisRotater.rotation *= Quaternion.AngleAxis(radius, Vector3.right);
		xAxisAngle += radius;

		if (xAxisAngle > XAxisMaxangle)
			xAxisAngle = XAxisMaxangle;

		if (xAxisAngle < XaxisMinAngle)
			xAxisAngle = XaxisMinAngle;
	}

	void YAxisRotate(float radius)
	{
		yAxisAngle += radius;

		if (yAxisAngle > 720.0f)
			yAxisAngle -= 360.0f;

		if (yAxisAngle < -720.0f)
			yAxisAngle += 360.0f;

		//YAxisRotater.rotation *= Quaternion.AngleAxis(radius, Vector3.up);
	}

	// Update is called once per frame
	void Update()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rIdentity.isLocalPlayer )    return;

		if(Time.timeScale != .0f)
		{
			XAxisRotate(Input.GetAxis("Mouse Y") * XAxisRotSpeed);

			YAxisRotate(Input.GetAxis("Mouse X") * YAxisRotSpeed);

			//cntRecoil -= Time.deltaTime * RecoilDampRate;
			//if (cntRecoil < .0f)
			//	cntRecoil = .0f;

			XAxisRotater.localRotation = Quaternion.AngleAxis (xAxisAngle + playerRecoil.cntRecoil.y, Vector3.left);
			YAxisRotater.localRotation = Quaternion.AngleAxis (yAxisAngle + playerRecoil.cntRecoil.x, Vector3.up);

		}

	}

	public void Recoil(float val)
	{
		playerRecoil.Shot();
		//cntRecoil += val;

		//if (MaxRecoil < cntRecoil)
		//	cntRecoil = MaxRecoil;
    }
}
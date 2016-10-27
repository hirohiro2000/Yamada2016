
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TPSRotationController : MonoBehaviour
{
	[SerializeField]
	Transform YAxisRotater;

	[SerializeField]
	Transform XAxisRotater;

	[SerializeField, Range(1.0f, 100.0f)]
	float YAxisRotSpeed;

	[SerializeField, Range(1.0f, 100.0f)]
	float XAxisRotSpeed;

	[SerializeField]
	float XAxisMaxangle = 70.0f;

	[SerializeField, Range(1.0f, 100.0f)]
	float XaxisMinAngle = -70.0f;

	[SerializeField, TooltipAttribute("一秒間で戻る角度")]
	float RecoilDampRate = 5.0f;

	[SerializeField, TooltipAttribute("リコイルの最大角度")]
	float MaxRecoil = 10.0f;

	float cntRecoil = .0f;

	float xAxisAngle = .0f;

    private NetworkIdentity m_rIdentity =   null;

	// Use this for initialization
	void Start()
	{
        m_rIdentity =   GetComponent< NetworkIdentity >();
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

		YAxisRotater.rotation *= Quaternion.AngleAxis(radius, Vector3.up);
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

			cntRecoil -= Time.deltaTime * RecoilDampRate;
			if (cntRecoil < .0f)
				cntRecoil = .0f;

			XAxisRotater.localRotation = Quaternion.AngleAxis (xAxisAngle + cntRecoil,Vector3.left); 


        }

	}

	public void Recoil(float val)
	{
		cntRecoil += val;

		if (MaxRecoil < cntRecoil)
			cntRecoil = MaxRecoil;
    }
}
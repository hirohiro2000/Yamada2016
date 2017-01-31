using UnityEngine;
using System.Collections;

public class TPS_RideGirlController : MonoBehaviour {

	bool m_IsRide;

	[SerializeField]
	Transform m_RideGirl;

	[SerializeField]
	Transform m_Cover;

	float m_Angle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//女の子を検索
		GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
		GirlController Girl = null;
        for (int i = 0; i < playerList.Length; i++)
		{
			if (this.gameObject == playerList[i]) continue;
			if (playerList[i].GetComponent<GirlController>() == null) continue;

			Girl = playerList[i].GetComponent<GirlController>();
			break;
        }
		if (Girl != null)
			m_IsRide = Girl.IsRide();
		else
		{
			m_IsRide = false;
        }

		m_RideGirl.gameObject.SetActive(m_IsRide);

		float RequestAngle;
		if(m_IsRide == true)
		{
			RequestAngle = 37.0f;
		}
		else
		{
			RequestAngle = 270.0f;
		}

		m_Angle = m_Angle + (RequestAngle - m_Angle) * 0.7f;

		m_Cover.localRotation = Quaternion.Euler(90, m_Angle, .0f);
	}
}

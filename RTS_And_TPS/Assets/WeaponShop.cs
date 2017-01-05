using UnityEngine;
using System.Collections;

public class WeaponShop : MonoBehaviour {

	// Use this for initialization
	TextMesh m_rCostText = null;
	GameManager m_rGameManager = null;
	LinkManager m_rLinkManager = null;

	public Transform ShopCanvas = null;

	//  Use this for initialization
	void Start()
	{
		m_rGameManager = FunctionManager.GetAccessComponent<GameManager>("GameManager");
		m_rLinkManager = FunctionManager.GetAccessComponent<LinkManager>("LinkManager");

		m_rCostText = transform.FindChild("_TextCost").GetComponent<TextMesh>();

	}
	// Update is called once per frame
	void Update () {
		//  アクセスを取得
		if (!m_rGameManager) m_rGameManager = FunctionManager.GetAccessComponent<GameManager>("GameManager");
		if (!m_rLinkManager) m_rLinkManager = FunctionManager.GetAccessComponent<LinkManager>("LinkManager");
	}

	void OnTriggerEnter(Collider collider)
	{
		//  アクセスチェック
		if (!m_rGameManager) return;
		if (!m_rLinkManager) return;

		//  プレイヤーかどうかチェック
		if (collider.tag != "Player") return;

		//  ローカルプレイヤーかどうかチェック
		NetPlayer_Control rNetControl = collider.GetComponentInParent<NetPlayer_Control>();
		if (!rNetControl) return;
		if (rNetControl.c_ClientID != m_rLinkManager.m_LocalPlayerID) return;


		ShopCanvas.gameObject.SetActive(true);
		m_rLinkManager.m_rLocalNPControl.GetComponent<TPSPlayer_Control>(). m_IsLock = false;
    }

	void OnTriggerExit(Collider collider)
	{

		ShopCanvas.gameObject.SetActive(false);
		m_rLinkManager.m_rLocalNPControl.GetComponent<TPSPlayer_Control>().m_IsLock = true;


	}

}

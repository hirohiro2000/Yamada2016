using UnityEngine;
using System.Collections;

public class UIForcusSelect : MonoBehaviour
{
    UIGirlTaskSelect m_girlTask                     { get; set; }
    ItemController   m_itemController               { get; set; }
    GameObject       m_towerInfoPanel               { get; set; }
    GameObject       m_towerInfoPanelCrystalFarm    { get; set; }

    public  void Initialize( UIGirlTaskSelect girlTask, ItemController itemController )
    {
        m_girlTask                  = girlTask;
        m_itemController            = itemController;
        m_towerInfoPanel            = girlTask.m_towerInfoPanel;
        m_towerInfoPanelCrystalFarm = girlTask.m_towerInfoPanelCrystalFarm;
    }
	private void OnEnable ()
    {
        if (m_itemController)
        {
            m_itemController.SetActive(true);
            if (m_itemController.GetForcus() != -1)
            {
                //	リソースのUI設定
                var param = m_itemController.GetResourceParam(m_itemController.GetForcus());
                if (param.gameObject.GetComponent<CrystalFarm_Control>() == null)
                {
                    m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(true);
                }
                else
                {
                    m_towerInfoPanelCrystalFarm.GetComponent<AppearInfopanel>().SetActive(true);
                }
            }
        }
	}       
	private void OnDisable ()
    {
        if ( m_itemController )
        {
            m_itemController.SetActive( false );
            m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(false);
            m_towerInfoPanelCrystalFarm.GetComponent<AppearInfopanel>().SetActive(false);
        }
    }	
	private void Update ()
    {
        if ( m_girlTask         == null )   return;
        if ( m_itemController   == null )   return;

	    if ( Input.GetKeyDown(KeyCode.E) )
        {
            int forcus = m_itemController.GetForcus();
            forcus = ( forcus+1 ) % m_itemController.GetNumKind();
            m_girlTask.SetForcus( forcus );
        }
	    if ( Input.GetKeyDown(KeyCode.Q) )
        {
            int forcus = m_itemController.GetForcus();
            forcus = ( forcus+m_itemController.GetNumKind()-1 ) % m_itemController.GetNumKind();
            m_girlTask.SetForcus( forcus );
        }

	}
}

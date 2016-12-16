using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 女の子のタワーの作成や破壊の選択をクリックでも行えるようにするクラス
public class UIGirlTaskSelect : MonoBehaviour
{
//  public  GameObject          m_rGirl                     = null;
	public	GameObject			m_buttonOk                  = null;
    public	GameObject			m_buttonCancel				= null;
	public	GameObject			m_buttonLevel				= null;
	public	GameObject			m_buttonBreak				= null;
	public	GameObject			m_towerInfoPanel			= null;

	public ItemController		m_itemCntroller             = null;
    public ResourceInformation	m_rResourceInformation		= null;

    public enum RESULT
    {
        eNone,
        eOK,
        eCancel,
        eLevel,
        eBreak,
        eErr = -1,
    }
    public RESULT result = RESULT.eNone;

    public void Clear()
    {
        result = RESULT.eNone;
		m_buttonOk.SetActive(false);
		m_buttonLevel.SetActive(false);
		m_buttonBreak.SetActive(false);
		m_buttonCancel.SetActive(false);
		m_towerInfoPanel.SetActive(false);
        m_itemCntroller.SetActive( false );
    }
    public void SetForcus( int forcusID )
    {
        if (forcusID == -1)
        {
            m_towerInfoPanel.SetActive(false);
        }
        else
        {
		    //	リソースのUI設定
            m_towerInfoPanel.SetActive(true);
		    var forcusParam = m_itemCntroller.GetForcusResourceParam();
		    m_buttonOk.transform.FindChild("Point").GetComponent<Text>().text = "-" + forcusParam.GetCreateCost().ToString();
		    m_towerInfoPanel.transform.FindChild("Kind").GetComponent<Text>().text     = forcusParam.m_name;
		    m_towerInfoPanel.transform.FindChild("Summary").GetComponent<Text>().text  = "概要:　　　" + forcusParam.m_summary;
		    m_towerInfoPanel.transform.FindChild("Power").GetComponent<Text>().text    = "攻撃力:　　" + forcusParam.GetLevelParam(0).power;
		    m_towerInfoPanel.transform.FindChild("Interval").GetComponent<Text>().text = "発射間隔:　" + forcusParam.GetLevelParam(0).interval + "秒/発";
        }

    }

    public RESULT ToSelectTheCreateResource()
    {
        m_itemCntroller.SetActive( true );
        return result;
   }
    public RESULT ToSelectTheConvertAction()
    {
		m_buttonLevel.SetActive(true);
		m_buttonBreak.SetActive(true);
		m_buttonCancel.SetActive(true);
        return result;
    }
    
    public void SelectOK()
    {
        result = RESULT.eOK;
    }
    public void SelectCancel()
    {
        result = RESULT.eCancel;
    }
    public void SelectLevel()
    {
        result = RESULT.eLevel;
    }
    public void SelectBreak()
    {
        result = RESULT.eBreak;
    }

}

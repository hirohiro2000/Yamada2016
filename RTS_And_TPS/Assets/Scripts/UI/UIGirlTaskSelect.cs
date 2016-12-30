using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// UIと女の子のやり取りを補助するクラス
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
		    var param = m_itemCntroller.GetResourceParam( forcusID );
		    m_buttonOk.transform.FindChild("Point").GetComponent<Text>().text          = "-" + param.GetCreateCost().ToString();
		    m_towerInfoPanel.transform.FindChild("Kind").GetComponent<Text>().text     = param.m_name;
		    m_towerInfoPanel.transform.FindChild("Summary").GetComponent<Text>().text  = "概要:　　　" + param.m_summary;
		    m_towerInfoPanel.transform.FindChild("Power").GetComponent<Text>().text    = "攻撃力:　　" + param.GetLevelParam(0).power;
		    m_towerInfoPanel.transform.FindChild("Interval").GetComponent<Text>().text = "発射間隔:　" + param.GetLevelParam(0).interval + "秒/発";
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

        //  効果音再生  
        SoundController.PlayNow( "UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f );
        SoundController.PlayNow( "UI_Click", 0.0f, 0.1f, 0.84f, 1.0f );
    }
    public void SelectLevel()
    {
        result = RESULT.eLevel;

        //  効果音再生  
        SoundController.PlayNow( "UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f );
        SoundController.PlayNow( "UI_Click", 0.0f, 0.1f, 0.84f, 1.0f );
    }
    public void SelectBreak()
    {
        result = RESULT.eBreak;

        //  効果音再生  
        SoundController.PlayNow( "UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f );
        SoundController.PlayNow( "UI_Click", 0.0f, 0.1f, 0.84f, 1.0f );
    }

}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// UIと女の子のやり取りを補助するクラス
public class UIGirlTaskSelect : MonoBehaviour
{
	public	GameObject			m_buttonOk                  = null;
    public	GameObject			m_buttonCancel				= null;
	public	GameObject			m_buttonLevel				= null;
	public	GameObject			m_buttonBreak				= null;
	public	GameObject			m_towerInfoPanel			= null;

    public ResourceInformation	m_resourceInformation		= null;
    public ResourceCreator      m_resourceCreator           = null;
	public ItemController		m_itemController            = null;

	private const KeyCode		m_okKey							= KeyCode.Z;
	private const KeyCode		m_cancelKey						= KeyCode.X;
	private const KeyCode		m_breakKey						= KeyCode.C;

    private enum MODE
    {
        eCommon,
        eCreate,
        eConvert,
    }
    private MODE   m_mode       = MODE.eCommon;

    public enum RESULT
    {
        eNone,
        eOK,
        eCancel,
        eLevel,
        eBreak,
        eErr = -1,
    }
    private RESULT result = RESULT.eNone;
    
    // ＵＩ情報を更新をして行うべきタスクを戻り値として返す
    public RESULT Select( Vector3 girlPosition )
    {
        switch (m_mode)
        {
            case MODE.eCommon:  UpdateCommon( girlPosition );  break;
            case MODE.eCreate:  UpdateCreate( girlPosition );  break;
            case MODE.eConvert: UpdateConvert( girlPosition ); break;
            default: break;
        }
        return result;
    }

    // 初期状態に戻す
    public void Reset()
    {
        m_mode = MODE.eCommon;
        result = RESULT.eNone;
		m_buttonOk.SetActive(false);
		m_buttonLevel.SetActive(false);
		m_buttonBreak.SetActive(false);
		m_buttonCancel.SetActive(false);
		m_towerInfoPanel.SetActive(false);
        m_itemController.SetActive( false );
        m_towerInfoPanel.SetActive(false);
        m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
        m_resourceCreator.SetGuideVisibleDisable();
    }

    //---------------------------------------------------------------------
    //      ステート
    //---------------------------------------------------------------------   	
    void UpdateCommon( Vector3 computePosition )
    {
		m_buttonOk.SetActive(false);
		m_buttonLevel.SetActive(false);
		m_buttonBreak.SetActive(false);
		m_buttonCancel.SetActive(false);
		m_resourceCreator.SetGuideVisibleDisable();

		//	change state
		if( !Input.GetKeyDown( m_cancelKey ))
			return;

		if( m_resourceInformation.CheckExistResourceFromPosition( computePosition ) )
		{
			m_buttonLevel.SetActive(true);
			m_buttonBreak.SetActive(true);
			m_buttonCancel.SetActive(true);
			m_mode = MODE.eConvert;
		}
		else
		{
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_itemController.SetActive(true);
			m_mode = MODE.eCreate;
		}
    }
    void UpdateCreate( Vector3 computePosition )
    {
        if ( m_resourceInformation.CheckExistResourceFromPosition( computePosition ) )
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
            m_resourceCreator.SetGuideVisibleDisable();
            m_itemController.SetActive(false);
    		m_towerInfoPanel.SetActive(false);

			m_buttonLevel.SetActive(true);
			m_buttonBreak.SetActive(true);
			m_buttonCancel.SetActive(true);

            m_mode = MODE.eConvert;
            return;
        }

        if ( Input.GetKeyDown( m_cancelKey ))
        {
            Reset();
            return;
        }

		//	リソースのUI設定
        m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
        m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);
        
		//	リソースの範囲表示更新
        int forcus = m_itemController.GetForcus();
        if ( forcus != -1 )
        {
		    m_resourceCreator.UpdateGuideResource( forcus, computePosition );
		    m_resourceCreator.UpdateGuideRange( forcus, computePosition );
        }
        else
        {
            m_resourceCreator.SetGuideVisibleDisable();
        }

        
        if ( Input.GetKeyDown( m_okKey ) )
        {
            // 生成
            result = RESULT.eOK;
        }

        // ショートカット
        for (int i = 0; i < m_itemController.GetNumKind(); i++)
        {
            if ( Input.GetKeyDown(KeyCode.Alpha1+ i) )
            {
                result = RESULT.eOK;
                m_itemController.SetForcus(i);
            }
        }

    }
    void UpdateConvert( Vector3 computePosition )
    {
		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        if( !param )
        {

			m_buttonLevel.SetActive(false);
			m_buttonBreak.SetActive(false);
			m_buttonCancel.SetActive(false);

            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_itemController.SetActive(true);
    		m_towerInfoPanel.SetActive(true);
            m_mode = MODE.eCreate;

            return;
        }

        if ( Input.GetKeyDown( m_cancelKey ))
        {
            Reset();
            return;
        }

		//	リソースのUI設定
		m_buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + param.GetCurLevelParam().GetUpCost().ToString();
		m_buttonBreak.transform.FindChild("Point").GetComponent<Text>().text = "+" + param.GetBreakCost().ToString();

		//	リソースの範囲表示更新
		m_resourceCreator.UpdateGuideRange( computePosition );

        if (Input.GetKeyDown(m_okKey) && param != null && param.CheckWhetherCanUpALevel())
        {
            // 強化
            result = RESULT.eLevel;
        }
        else if (Input.GetKeyDown(m_breakKey) && param != null)
        {
            // 破壊
            result = RESULT.eBreak;
        }
               
    }
    
    // ボタン専用設定
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
		    var param = m_itemController.GetResourceParam( forcusID );
		    m_buttonOk.transform.FindChild("Point").GetComponent<Text>().text          = "-" + param.GetCreateCost().ToString();
		    m_towerInfoPanel.transform.FindChild("Kind").GetComponent<Text>().text     = param.m_name;
		    m_towerInfoPanel.transform.FindChild("Summary").GetComponent<Text>().text  = "概要:　　　" + param.m_summary;
		    m_towerInfoPanel.transform.FindChild("Power").GetComponent<Text>().text    = "攻撃力:　　" + param.GetLevelParam(0).power;
		    m_towerInfoPanel.transform.FindChild("Interval").GetComponent<Text>().text = "発射間隔:　" + param.GetLevelParam(0).interval + "秒/発";
        }

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

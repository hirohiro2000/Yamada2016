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
        eSelect,
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
            case MODE.eCommon:  UpdateCommon( girlPosition );           break;
            case MODE.eSelect:  UpdateSelectResource( girlPosition );   break;
            case MODE.eCreate:  UpdateCreate( girlPosition );           break;
//            case MODE.eConvert: UpdateConvert( girlPosition ); break;
            default: break;
        }

        UpdateConvert( girlPosition );

        return result;
    }

    // 初期状態に戻す
    public void Reset()
    {
        result = RESULT.eNone;
//        m_mode = MODE.eCommon;
//		m_buttonOk.SetActive(false);
//		m_buttonLevel.SetActive(false);
//		m_buttonBreak.SetActive(false);
//		m_buttonCancel.SetActive(false);
//		m_towerInfoPanel.SetActive(false);
//        m_itemController.SetActive( false );
//        m_towerInfoPanel.SetActive(false);
//        m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
//        m_resourceCreator.SetGuideVisibleDisable();
    }

    //---------------------------------------------------------------------
    //      ステート
    //---------------------------------------------------------------------   	
#if false

    void UpdateCommon( Vector3 computePosition )
    {
		m_buttonOk.SetActive(false);
		m_buttonLevel.SetActive(false);
		m_buttonBreak.SetActive(false);
		m_buttonCancel.SetActive(false);
		m_resourceCreator.SetGuideVisibleDisable();

		//	change state
        bool isAnyKey = ( Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1) );

		if( !isAnyKey )			return;


        //  効果音再生（パネルを出す）
        SoundController.PlayNow( "UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f );

		{
            m_mode = MODE.eCreate;
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_itemController.SetActive(true);
            m_itemController.SetForcus( -1 );
		}
    }
    void UpdateCreate( Vector3 computePosition )
    {
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

		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        if( param )
        {
            m_itemController.SetActive( false );
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
            m_resourceCreator.SetGuideVisibleDisable();
        }
        else
        {
            m_itemController.SetActive( true );
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
        } 
        
        //
        if ( Input.GetKeyDown( KeyCode.F ) || Input.GetMouseButtonDown(1) )
        {  
            m_mode = MODE.eCommon;
            m_itemController.SetActive( false );
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
            m_resourceCreator.SetGuideVisibleDisable();
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
                        
            return;
        }

    	m_buttonLevel.SetActive( param.CheckWhetherCanUpALevel() && param.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost() );
        m_buttonBreak.SetActive(true);
        m_buttonCancel.SetActive(true);

		//	リソースのUI設定
		m_buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + param.GetCurLevelParam().GetUpCost().ToString();
		m_buttonBreak.transform.FindChild("Point").GetComponent<Text>().text = "+" + param.GetBreakCost().ToString();

		//	リソースの範囲表示更新
		m_resourceCreator.UpdateGuideRange( computePosition );

        if (Input.GetKeyDown(KeyCode.V) && param.CheckWhetherCanUpALevel() && param.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost() )
        {
            // 強化
            result = RESULT.eLevel;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            // 破壊
            result = RESULT.eBreak;
        }
    }


#else

    void UpdateCommon( Vector3 computePosition )
    {
		m_buttonOk.SetActive(false);
		m_buttonLevel.SetActive(false);
		m_buttonBreak.SetActive(false);
		m_buttonCancel.SetActive(false);
		m_resourceCreator.SetGuideVisibleDisable();

		//	change state
        bool isAnyKey = ( Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E ) || Input.GetMouseButtonDown(1) );

		if( !isAnyKey )			return;


        //  効果音再生（パネルを出す）
        SoundController.PlayNow( "UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f );

//        var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
//        if( param )
//		{
//			m_mode = MODE.eConvert;
//			m_buttonLevel.SetActive( param.CheckWhetherCanUpALevel() && param.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost() );
//			m_buttonBreak.SetActive(true);
//			m_buttonCancel.SetActive(true);
//		}
//		else
		{
            m_mode = MODE.eSelect;
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_itemController.SetActive(true);
            m_itemController.SetForcus( 0 );
		}
    }
    void UpdateCreate( Vector3 computePosition )
    {
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

        //
        if ( Input.GetKeyDown( KeyCode.X ) || Input.GetMouseButtonDown(1) )
        {
            m_mode = MODE.eSelect;
            m_itemController.SetActive( true );
            m_resourceCreator.SetGuideVisibleDisable();
        }
		if( Input.GetKeyDown( KeyCode.Q ))
        {
            m_mode = MODE.eSelect;
            m_itemController.SetActive( true );
            m_resourceCreator.SetGuideVisibleDisable();
            forcus = Mathf.Clamp( m_itemController.GetForcus(), 0, m_itemController.GetNumKind() );
            for (int i = 0; i < m_itemController.GetNumKind(); i++)
            {
                forcus--;
                if (forcus == -1)
                {
                    forcus = m_itemController.GetNumKind()-1;
                }
                if ( m_itemController.CheckWhetherTheCostIsEnough( forcus ) )
                {
                    SetForcus( forcus );
                    m_itemController.SetForcus( forcus );
                    break;
                }
            }
        }
        if ( Input.GetKeyDown( KeyCode.E ))
        {
            m_mode = MODE.eSelect;
            m_itemController.SetActive( true );
            m_resourceCreator.SetGuideVisibleDisable();
            forcus = Mathf.Clamp( m_itemController.GetForcus(), 0, m_itemController.GetNumKind() );
            for (int i = 0; i < m_itemController.GetNumKind(); i++)
            {
                forcus++;
                if (forcus == m_itemController.GetNumKind())
                {
                    forcus = 0;
                }
                if ( m_itemController.CheckWhetherTheCostIsEnough( forcus ) )
                {
                    SetForcus( forcus );
                    m_itemController.SetForcus( forcus );
                    break;
                }
            }
        }

		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        if( param )
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
            m_resourceCreator.SetGuideVisibleDisable();
        }
        else
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            if ( Input.GetKeyDown( KeyCode.F ))
            {
                result = RESULT.eOK;
            }  
        }

        // 残高不足
        int balance = ( m_itemController.GetHaveCost() - m_itemController.GetForcusResourceParam().GetCreateCost() );
        if ( balance < 0 )
        {
            m_mode = MODE.eSelect;
            m_itemController.SetActive(true);
            m_towerInfoPanel.SetActive(true);
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_resourceCreator.SetGuideVisibleDisable();
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
                        
            return;
        }

		//	リソースの範囲表示更新
	    m_resourceCreator.UpdateGuideRange( computePosition, param.GetCurLevelParam().range );


    	m_buttonLevel.SetActive( param.CheckWhetherCanUpALevel() && param.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost() );
        m_buttonBreak.SetActive(true);
        m_buttonCancel.SetActive(true);

		//	リソースのUI設定
		m_buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + param.GetCurLevelParam().GetUpCost().ToString();
		m_buttonBreak.transform.FindChild("Point").GetComponent<Text>().text = "+" + param.GetBreakCost().ToString();

		//	リソースの範囲表示更新
		m_resourceCreator.UpdateGuideRange( computePosition );

        if (Input.GetKeyDown(m_okKey) && param.CheckWhetherCanUpALevel() && param.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost() )
        {
            // 強化
            result = RESULT.eLevel;
        }
        else if (Input.GetKeyDown(m_breakKey))
        {
            // 破壊
            result = RESULT.eBreak;
        }
    }
    void UpdateSelectResource( Vector3 computePosition )
    {
		if( Input.GetKeyDown( KeyCode.Q ))
        {
            int forcus = Mathf.Clamp( m_itemController.GetForcus(), 0, m_itemController.GetNumKind() );
            {
                forcus--;
                if (forcus == -1)
                {
                    forcus = m_itemController.GetNumKind()-1;
                }
                {
                    SetForcus( forcus );
                    m_itemController.SetForcus( forcus );
                }
            }
        }
        if ( Input.GetKeyDown( KeyCode.E ))
        {
            int forcus = Mathf.Clamp( m_itemController.GetForcus(), 0, m_itemController.GetNumKind() );
            {
                forcus++;
                if (forcus == m_itemController.GetNumKind())
                {
                    forcus = 0;
                }
                {
                    SetForcus( forcus );
                    m_itemController.SetForcus( forcus );
                }
            }
        }

		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = ( param == null );
        m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
        m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);

        if ( Input.GetKeyDown( KeyCode.F ) && ( m_itemController.CheckWhetherTheCostIsEnough( m_itemController.GetForcus() ) ) )
        {
            m_mode = MODE.eCreate;
     		m_towerInfoPanel.SetActive(false);
            for (int i = 0; i < m_itemController.GetNumKind(); i++)
            {
                m_itemController.SetActive( i == m_itemController.GetForcus(), i );
            }
        }
        if ( Input.GetKeyDown( KeyCode.X ) || Input.GetMouseButtonDown(1))
        {
            m_mode = MODE.eCommon;
            m_itemController.SetActive( false );
    		m_towerInfoPanel.SetActive( false );
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
            m_resourceCreator.SetGuideVisibleDisable();
        }

    }
 
#endif


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
    public void SetForcusByButton( int forcusID )
    {
        if ( m_mode == MODE.eCreate )   return;
        SetForcus( forcusID );
        m_itemController.SetForcus( forcusID );
    }
    public void SelectOKByButton( int forcusID )
    {    
        Vector3 computePosition = m_resourceInformation.m_gridSplitSpacePlane.transform.position;
		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        if ( m_mode == MODE.eCreate && param == null )
        {
            result = RESULT.eOK;
            //  効果音再生  
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
            return;
        }

        if (m_itemController.CheckWhetherTheCostIsEnough(forcusID))
        {
            m_mode = MODE.eCreate;
     		m_towerInfoPanel.SetActive(false);
            m_itemController.SetForcus( forcusID );
            for (int i = 0; i < m_itemController.GetNumKind(); i++)
            {
                m_itemController.SetActive( i == m_itemController.GetForcus(), i );
            }
            //  効果音再生  
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
        }
        else
        {
            //  効果音再生
            SoundController.PlayNow("UI_NG", 0.0f, 0.1f, 0.64f, 1.0f);
        }
    













//        Vector3 computePosition = m_resourceInformation.m_gridSplitSpacePlane.transform.position;
//		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
//        if ( param != null )  return;
//
//        if (m_itemController.CheckWhetherTheCostIsEnough(forcusID))
//        {
//            result = RESULT.eOK;
//     		m_towerInfoPanel.SetActive(false);
//            m_itemController.SetForcus( forcusID );
//            //  効果音再生  
//            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
//            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
//        }
//        else
//        {
//            //  効果音再生
//            SoundController.PlayNow("UI_NG", 0.0f, 0.1f, 0.64f, 1.0f);
//        }
    }
    public void SelectOK()
    {
//        result = RESULT.eOK;
    }
    public void SelectCancel()
    {
//        result = RESULT.eCancel;
//
//        //  効果音再生  
//        SoundController.PlayNow( "UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f );
//        SoundController.PlayNow( "UI_Click", 0.0f, 0.1f, 0.84f, 1.0f );
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

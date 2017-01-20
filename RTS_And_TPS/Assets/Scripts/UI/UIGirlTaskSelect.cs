using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// UIと女の子のやり取りを補助するクラス
public class UIGirlTaskSelect : MonoBehaviour
{
    public	GameObject			m_uiResourceBG   			= null;
    public	GameObject			m_uiForcusFrame 			= null;
    public	GameObject			m_uiConvert 				= null;
	public	GameObject			m_towerInfoPanel			= null;
	public	GameObject			m_towerInfoPanelEx			= null;

    private ResourceInformation m_resourceInformation		{ get; set; }
    private ResourceCreator     m_resourceCreator           { get; set; }
	private ItemController		m_itemController            { get; set; }
    public  GameObject          m_curConvertTarget          { get; set; }

    private bool isInit { get; set; }

    public  enum RESULT
    {
        eNone,
        eOK,
        eCancel,
        eLevel,
        eBreak,
        eErr = -1,
    }
    private RESULT result               { get; set; }

    // [GirlController]専用の初期化関数
    public void Initialize( GirlController girl )
    {
        result                   = RESULT.eNone;
        m_itemController         = girl.GetComponent<ItemController>();
        m_itemController.SetForcus( 0 );
        m_resourceInformation    = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
        m_resourceCreator        = GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();

        m_uiResourceBG.GetComponent<UIForcusSelect>().Initialize( this, m_itemController );
        m_uiForcusFrame.GetComponent<UIForcusFrame>().Initialize( this, m_itemController );

    }

    // ＵＩ情報を更新をして行うべきタスクを戻り値として返す
    public RESULT Select( Vector3 girlPosition )
    {
        if ( isInit == false )
        {
            isInit = true;
            return result;
        }

        UpdateGuide( girlPosition );
        UpdateEvent( girlPosition );

        return result;
    }
             
    // 初期状態に戻す
    public void Reset()
    {
        result = RESULT.eNone;
    }

    //---------------------------------------------------------------------
    //      入力情報の更新
    //---------------------------------------------------------------------   	
    void UpdateEvent(Vector3 computePosition)
    {
        if ( !m_uiConvert.activeInHierarchy )
        {
            if ( Input.GetKeyDown(KeyCode.X ) || Input.GetMouseButtonDown(1) )
            {
                bool active0 = !m_uiForcusFrame.activeInHierarchy;
                m_uiForcusFrame.SetActive( active0 );

                bool active1 = !m_uiResourceBG.activeInHierarchy;
                m_uiResourceBG.SetActive( active1 );
            }
        }
        else
        {
            if ( Input.GetKeyDown(KeyCode.X ) )
            {
                m_uiConvert.SetActive( false );
            }
        }

    }
    void UpdateActiveUI()
    {

    }

    //---------------------------------------------------------------------
    //      ガイド情報の更新
    //---------------------------------------------------------------------   	
    void UpdateGuide(Vector3 computePosition)
    {
        var param = m_resourceInformation.GetResourceParamFromPosition(computePosition);
        if ( param )
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);
        }
    }

    //---------------------------------------------------------------------
    //      
    //---------------------------------------------------------------------   	
    public void SetForcus( int forcusID )
    {
        // フォーカス変更
        m_itemController.SetForcus( forcusID );

        if (forcusID == -1)
        {
            m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(false);
        }
        else    
        {                   
		    //	リソースのUI設定
            m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(true);
		    var param = m_itemController.GetResourceParam( forcusID );
		    m_towerInfoPanel.transform.FindChild("Kind").GetComponent<Text>().text     = param.m_name;
		    m_towerInfoPanel.transform.FindChild("Summary").GetComponent<Text>().text  = "概要:　　　" + param.m_summary;
		    m_towerInfoPanel.transform.FindChild("Power").GetComponent<Text>().text    = "攻撃力:　　" + param.GetLevelParam(0).power;
		    m_towerInfoPanel.transform.FindChild("Interval").GetComponent<Text>().text = "発射間隔:　" + param.GetLevelParam(0).interval + "秒/発";

            // 効果音再生
            SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.1f, Random.Range( 0.95f, 1.05f ), 1.0f );
        }
    }
    
    // ボタン専用設定
    public void SelectOK( int forcusID )
    {    
        // Go to [Create]
        if (m_itemController.CheckWhetherTheCostIsEnough(forcusID))
        {
            m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(false);
     		m_itemController.SetActive(false);
            m_uiResourceBG.SetActive( false );
            m_itemController.SetForcus( forcusID );
            m_uiForcusFrame.SetActive( true );   
			Text text = m_uiForcusFrame.transform.GetChild(2).GetComponent<Text>();
			ResourceParameter resource = m_resourceCreator.m_resources[ m_itemController.GetForcus() ].GetComponent<ResourceParameter>();
            text.text = resource.GetCreateCost().ToString();
            m_uiForcusFrame.transform.GetChild(0).GetComponent<RawImage>().texture = m_resourceCreator.m_textures[ m_itemController.GetForcus() ].GetComponent<Image>().mainTexture;

            //  効果音再生  
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
        }
        else
        {
            //  効果音再生
            SoundController.PlayNow("UI_NG", 0.0f, 0.1f, 0.64f, 1.0f);
        }
    }
    public void SelectLevel()
    {
        var target = m_curConvertTarget.GetComponent<ResourceParameter>();

        // レベルアップ処理はできますか？
        if ( target.CheckWhetherCanUpALevel() && target.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost())
        {
            // レベルアップ処理を行います
            result = RESULT.eLevel;

            //  効果音再生  
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
            
            // タワーのレベルは上限に達しましたか？
            if (target.m_levelInformations.Length-1 == target.m_level + 1)
            {
                // [上限に達しました]
                Transform buttonLevel = m_uiConvert.transform.GetChild(0);
                buttonLevel.gameObject.SetActive(false);
                m_towerInfoPanelEx.SetActive(false);

                // [処理が中断されました]
                return;
            }
            else
            {
                // 上限でないのでLvUP情報を更新します
                m_towerInfoPanelEx.transform.FindChild("Power").GetComponent<Text>().text           = "攻撃力:　　" + target.m_levelInformations[target.m_level + 1].power;
                m_towerInfoPanelEx.transform.FindChild("Interval").GetComponent<Text>().text        = "発射間隔:　" + target.m_levelInformations[target.m_level + 1].interval;
                m_towerInfoPanelEx.transform.FindChild("PowerLv").GetComponent<Text>().text         = target.m_levelInformations[target.m_level + 2].power.ToString();
                m_towerInfoPanelEx.transform.FindChild("IntervalLv").GetComponent<Text>().text      = target.m_levelInformations[target.m_level + 2].interval.ToString();
            }


            // ボタンの情報の更新
            {
                // 消費コスト
                float nextCost = target.m_levelInformations[target.m_level + 1].GetUpCost();                                    

                // 使用後の残高
                float balance = m_itemController.GetHaveCost() - target.m_levelInformations[target.m_level].GetUpCost();        

                Transform buttonLevel = m_uiConvert.transform.GetChild(0);
                buttonLevel.transform.GetChild(0).gameObject.SetActive( (balance - nextCost < 0) );                         
                buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + nextCost.ToString();
            }

        }
        else
        {
            //  効果音再生
            SoundController.PlayNow("UI_NG", 0.0f, 0.1f, 0.64f, 1.0f);
        }


    }
    public void SelectBreak()
    {
        result = RESULT.eBreak;

        // 効果音再生
        SoundController.PlayNow( "UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f );
        SoundController.PlayNow( "UI_Click", 0.0f, 0.1f, 0.84f, 1.0f );
    }
    public void ClikedForcusFrame()
    {
        if ( m_itemController.GetForcus() == -1 )               return;
        if ( !m_itemController.CheckWhetherTheCostIsEnough(m_itemController.GetForcus()) ) return;

        Vector3 computePosition = m_resourceInformation.m_gridSplitSpacePlane.transform.position;
		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        if ( param != null )    return;
               
        // 建築
        if (m_uiConvert.activeInHierarchy)
        {
            result = RESULT.eOK;
        }
        else
        {
            m_uiConvert.SetActive( true );
        }

        //  効果音再生  
        SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
        SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
    }

}


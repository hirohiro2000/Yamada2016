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


    private RectTransform       m_parentTrans               { get; set; }
    private Camera              m_uiCamera                  { get; set; }
    private Canvas              m_uiCanvas                  { get; set; }

    public  enum CREATE_STEP
    {
        eCommon,
        eSelect,
        eCreate,
    }
    public  CREATE_STEP         m_createStep        { get; private set; }

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
        m_createStep             = CREATE_STEP.eCommon;
        result                   = RESULT.eNone;
        m_itemController         = girl.GetComponent<ItemController>();
        m_resourceInformation    = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
        m_resourceCreator        = GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();

        RectTransform rt = GetComponent<RectTransform>();
        m_parentTrans = rt.parent.GetComponent<RectTransform>();

        Canvas[] canvasArr = m_uiConvert.GetComponentsInParent<Canvas>();
        for (int i = 0; i < canvasArr.Length; i++)
        {
            if (canvasArr[i].isRootCanvas)
            {
                m_uiCamera = canvasArr[i].worldCamera;
                m_uiCanvas = canvasArr[i];
            }
        }

    }

    // ＵＩ情報を更新をして行うべきタスクを戻り値として返す
    public RESULT Select( Vector3 girlPosition )
    {
        //  建築用ＵＩの更新
        switch (m_createStep)
        {
            case CREATE_STEP.eCommon:  UpdateCommon( girlPosition );           break;
            case CREATE_STEP.eSelect:  UpdateSelectResource( girlPosition );   break;
            case CREATE_STEP.eCreate:  UpdateCreate( girlPosition );           break;
            default: break;
        }
                   
        // コンバート用ＵＩの更新
        UpdateConvert( girlPosition );

        return result;
    }

    // 初期状態に戻す
    public void Reset()
    {
        result = RESULT.eNone;
    }

    //---------------------------------------------------------------------
    //      建築用ＵＩの更新
    //---------------------------------------------------------------------   	
    void UpdateCommon( Vector3 computePosition )
    {
		//	Can you move to selection phase ?
        bool canToSelectState = ( Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1) );
		if( !canToSelectState )			return;

        // リソースが存在する時は'Q'と'E'が使用できません
        if (( Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E) ) && m_resourceInformation.GetResourceParamFromPosition( computePosition ) )
        {
            return;
        }

        int forcus = m_itemController.GetForcus();

        // 選択されていない状態から始める
        if( Input.GetMouseButtonDown(1) )
        {
            forcus = -1;
        }
        // そのまま使用
        if( Input.GetKeyDown( KeyCode.X ))
        {
            forcus = Mathf.Clamp( m_itemController.GetForcus(), 0, m_itemController.GetNumKind() );
        }
        // 左にスライド
        if( Input.GetKeyDown( KeyCode.Q ))
        {
            forcus = ( ( forcus+m_itemController.GetNumKind()-1 ) % m_itemController.GetNumKind() );
        }
        // 右にスライド
        if ( Input.GetKeyDown( KeyCode.E ))
        {
            forcus = ( (forcus+1) % m_itemController.GetNumKind() );
        }


        // Go to [Select]
        m_createStep = CREATE_STEP.eSelect;
        m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
        m_resourceCreator.SetGuideVisibleDisable();
        m_itemController.SetActive(true);
        m_uiResourceBG.SetActive(true);
        SetForcus( forcus );

        
        //  効果音再生（パネルを出す）
        SoundController.PlayNow( "UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f );

    }
    void UpdateSelectResource( Vector3 computePosition )
    { 
        //       
		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        if (param)
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
        }
        else
        {    
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);
        }
        
        // 左にスライド
		if( Input.GetKeyDown( KeyCode.Q ))
        {
            int forcus = ( ( m_itemController.GetForcus()+m_itemController.GetNumKind()-1 ) % m_itemController.GetNumKind() );
            SetForcus( forcus );
        }
        // 右にスライド
        if ( Input.GetKeyDown( KeyCode.E ))
        {
            int forcus = ( (m_itemController.GetForcus()+1) % m_itemController.GetNumKind() );
            SetForcus( forcus );
        }
        
        // Go to [Create]
        if ( Input.GetKeyDown( KeyCode.F ) && ( m_itemController.CheckWhetherTheCostIsEnough( m_itemController.GetForcus() ) ) )
        {
            m_createStep = CREATE_STEP.eCreate;
     		m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive( false );
            m_itemController.SetActive( false );
            m_uiResourceBG.SetActive( false );
            m_uiForcusFrame.SetActive( true );

			Text text = m_uiForcusFrame.transform.GetChild(2).GetComponent<Text>();
			ResourceParameter resource = m_resourceCreator.m_resources[ m_itemController.GetForcus() ].GetComponent<ResourceParameter>();
            text.text = resource.GetCreateCost().ToString();
            m_uiForcusFrame.transform.GetChild(0).GetComponent<RawImage>().texture = m_resourceCreator.m_textures[ m_itemController.GetForcus() ].GetComponent<Image>().mainTexture;
    
            //  効果音再生（パネルを出す）
            SoundController.PlayNow( "UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f );

        }
        // Back to [Common]
        if ( Input.GetKeyDown( KeyCode.X ) || Input.GetMouseButtonDown(1))
        {
            m_createStep = CREATE_STEP.eCommon;
            m_itemController.SetActive( false );
    		m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive( false );
            m_uiResourceBG.SetActive( false );
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
            m_resourceCreator.SetGuideVisibleDisable();

            //  効果音再生（パネルを出す）
            SoundController.PlayNow( "UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f );

        }

    }
    void UpdateCreate( Vector3 computePosition )
    {           
        // 自分がいるマスの情報による処理の分岐（タワーがある場合ガイドが非表示になり建築できません）
        if( m_resourceInformation.GetResourceParamFromPosition( computePosition ) )
        {
            m_resourceCreator.SetGuideVisibleDisable();
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            // 建築予定のタワーとガイドの更新
            m_resourceCreator.UpdateGuideResource( m_itemController.GetForcus(), computePosition );
            m_resourceCreator.UpdateGuideRange( m_itemController.GetForcus(), computePosition );

            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);

            //　建築イベントの発生
            if ( Input.GetKeyDown( KeyCode.F ))
            {
                result = RESULT.eOK;
                //  効果音再生  
                SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
                SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
            }
              
        }


        
		//	Can you move to selection phase ?
        bool canToSelectState = Input.GetKeyDown( KeyCode.X ) || Input.GetMouseButtonDown(1)
                             || Input.GetKeyDown( KeyCode.Q ) || Input.GetKeyDown( KeyCode.E );

        // 残高不足の判定
        int balance = ( m_itemController.GetHaveCost() - m_itemController.GetForcusResourceParam().GetCreateCost() );
        if ( balance < 0 )
        {
            canToSelectState = true;
        }

        if ( !canToSelectState )    return;
        
        int forcus = m_itemController.GetForcus();

        // 左にスライド
        if( Input.GetKeyDown( KeyCode.Q ))
        {
            forcus = ( ( forcus+m_itemController.GetNumKind()-1 ) % m_itemController.GetNumKind() );
        }
        // 右にスライド
        if ( Input.GetKeyDown( KeyCode.E ))
        {
            forcus = ( (forcus+1) % m_itemController.GetNumKind() );
        }
        
        // Back to [Select]
        m_createStep = CREATE_STEP.eSelect;
        SetForcus( forcus );
        m_itemController.SetActive( true );
        m_uiResourceBG.SetActive( true );
        m_uiForcusFrame.SetActive( false );
        m_resourceCreator.SetGuideVisibleDisable();

    }

    //---------------------------------------------------------------------
    //      コンバート用ＵＩの更新
    //---------------------------------------------------------------------   	
    void UpdateConvert(Vector3 computePosition)
    {
        var param = m_resourceInformation.GetResourceParamFromPosition(computePosition);

        // コンバートできる状態ですか？
        if (param == null || m_createStep != CREATE_STEP.eCommon)
        {
            // コンバート処理ができないのでUIを閉じます
            if ( m_uiConvert.activeInHierarchy )
            {
                m_towerInfoPanelEx.GetComponent<AppearInfopanel>().SetActive(false);
                m_uiConvert.SetActive(false);
                m_resourceCreator.SetGuideVisibleDisable();
            }

            // コンバートを行うターゲットをリセットします
            if (m_curConvertTarget != null)
            {
                m_curConvertTarget.GetComponent<MaterialSwitchToConvert>().Deactivate();
                m_curConvertTarget.GetComponent<MaterialSwitchToConvert>().enabled = false;
                m_curConvertTarget = null;
            }

            // [処理が中断されました]
            return;
        }

        // [--以下からコンバート情報の更新を行います--]

        
        // ＵＩの情報とターゲットの情報は正しいですか？
        if ( m_curConvertTarget != param.gameObject )
        {
            // ＵＩに誤りがあるので初期化処理を行います

            //　ターゲットの変更の場合
            if ( m_curConvertTarget != null )
            {
                m_curConvertTarget.GetComponent<MaterialSwitchToConvert>().Deactivate();
                m_curConvertTarget.GetComponent<MaterialSwitchToConvert>().enabled = false;
                m_curConvertTarget = null;
            }


            //
            m_uiConvert.SetActive(true);
            
            // ボタンに必要なコストを設定します
            Transform buttonBreak = m_uiConvert.transform.GetChild(1);
            buttonBreak.transform.FindChild("Point").GetComponent<Text>().text = "+" + param.GetBreakCost().ToString();

            // タワーのレベルは上限ですか？
            if ( param.CheckWhetherCanUpALevel() == false )
            {
                // [上限]
                Transform buttonLevel = m_uiConvert.transform.GetChild(0);
                buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "";

                // ターゲットの情報を設定します
                m_towerInfoPanelEx.GetComponent<AppearInfopanel>().SetActive(true);
                m_towerInfoPanelEx.transform.FindChild("Kind").GetComponent<Text>().text            = param.m_name;
                m_towerInfoPanelEx.transform.FindChild("Summary").GetComponent<Text>().text         = "概要:　　　" + param.m_summary;
                m_towerInfoPanelEx.transform.FindChild("Power").GetComponent<Text>().text           = "攻撃力:　　" + param.GetLevelParam(param.m_level).power;
                m_towerInfoPanelEx.transform.FindChild("Interval").GetComponent<Text>().text        = "発射間隔:　" + param.GetLevelParam(param.m_level).interval;
                m_towerInfoPanelEx.transform.FindChild("PowerLv").GetComponent<Text>().text         = param.GetLevelParam(param.m_level).power.ToString();
                m_towerInfoPanelEx.transform.FindChild("IntervalLv").GetComponent<Text>().text      = param.GetLevelParam(param.m_level).interval.ToString();

            }
            else
            {
                // [上限ではありません]
                Transform buttonLevel = m_uiConvert.transform.GetChild(0);
                buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + param.GetCurLevelParam().GetUpCost().ToString();

                // ターゲットの情報を設定します
                m_towerInfoPanelEx.GetComponent<AppearInfopanel>().SetActive(true);
                m_towerInfoPanelEx.transform.FindChild("Kind").GetComponent<Text>().text            = param.m_name;
                m_towerInfoPanelEx.transform.FindChild("Summary").GetComponent<Text>().text         = "概要:　　　" + param.m_summary;
                m_towerInfoPanelEx.transform.FindChild("Power").GetComponent<Text>().text           = "攻撃力:　　" + param.GetLevelParam(param.m_level).power;
                m_towerInfoPanelEx.transform.FindChild("Interval").GetComponent<Text>().text        = "発射間隔:　" + param.GetLevelParam(param.m_level).interval;
                m_towerInfoPanelEx.transform.FindChild("PowerLv").GetComponent<Text>().text         = param.GetLevelParam(param.m_level + 1).power.ToString();
                m_towerInfoPanelEx.transform.FindChild("IntervalLv").GetComponent<Text>().text      = param.GetLevelParam(param.m_level + 1).interval.ToString();

            }           

            // ターゲットのマテリアルを変更します
            param.gameObject.GetComponent<MaterialSwitchToConvert>().Activate();
            param.gameObject.GetComponent<MaterialSwitchToConvert>().enabled = true;

            //
            m_curConvertTarget = param.gameObject;

        }

        //  [--更新処理--]
        {
            //	射程範囲のガイドを更新します
            m_resourceCreator.UpdateGuideRange(computePosition, param.GetCurLevelParam().range);
        
            // ＵＩの位置ターゲットの位置によって更新します
            Vector3 screenPos = Camera.main.WorldToScreenPoint(m_curConvertTarget.transform.position);
            Vector2 localPos  = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_parentTrans, screenPos, m_uiCamera, out localPos);
            m_uiConvert.GetComponent<RectTransform>().localPosition = localPos;

            // LvUP用
            Transform buttonLevel = m_uiConvert.transform.GetChild(0);
            buttonLevel.transform.GetChild(0).gameObject.SetActive( !(param.CheckWhetherCanUpALevel() && param.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost() ) );
        }
        


        //  [--イベント処理--]

        
        // LvUPを行いますか？
        if (Input.GetKeyDown(KeyCode.Q) && param.CheckWhetherCanUpALevel() && param.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost())
        {

            // LvUP処理
            result = RESULT.eLevel;

            // タワーのレベルは上限に達しましたか？
            if (param.m_levelInformations.Length-1 == param.m_level+1)
            {
                // [上限に達しました]
                Transform buttonLevel = m_uiConvert.transform.GetChild(0);
                buttonLevel.transform.GetChild(0).gameObject.SetActive(true);
                buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "";

                LevelParam nextParam = param.m_levelInformations[param.m_level + 1];
                m_towerInfoPanelEx.transform.FindChild("Power").GetComponent<Text>().text           = "攻撃力:　　" + nextParam.power;
                m_towerInfoPanelEx.transform.FindChild("Interval").GetComponent<Text>().text        = "発射間隔:　" + nextParam.interval;
                m_towerInfoPanelEx.transform.FindChild("PowerLv").GetComponent<Text>().text         = nextParam.power.ToString();
                m_towerInfoPanelEx.transform.FindChild("IntervalLv").GetComponent<Text>().text      = nextParam.interval.ToString();

                // [処理が中断されました]
                return;
            }
            else
            {
                // 上限でないのでLvUP情報を更新します
                m_towerInfoPanelEx.transform.FindChild("Power").GetComponent<Text>().text           = "攻撃力:　　" + param.m_levelInformations[param.m_level + 1].power;
                m_towerInfoPanelEx.transform.FindChild("Interval").GetComponent<Text>().text        = "発射間隔:　" + param.m_levelInformations[param.m_level + 1].interval;
                m_towerInfoPanelEx.transform.FindChild("PowerLv").GetComponent<Text>().text         = param.m_levelInformations[param.m_level + 2].power.ToString();
                m_towerInfoPanelEx.transform.FindChild("IntervalLv").GetComponent<Text>().text      = param.m_levelInformations[param.m_level + 2].interval.ToString();
            }

            // ボタンの情報の更新
            {
                // 消費コスト
                float nextCost = param.m_levelInformations[param.m_level + 1].GetUpCost();                                    

                // 使用後の残高
                float balance = m_itemController.GetHaveCost() - param.m_levelInformations[param.m_level].GetUpCost();        

                Transform buttonLevel = m_uiConvert.transform.GetChild(0);
                buttonLevel.transform.GetChild(0).gameObject.SetActive( (balance - nextCost < 0) );                         
                buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + nextCost.ToString();
            }


        }

        // 破壊しますか？
        if (Input.GetKeyDown(KeyCode.E))
        {
            result = RESULT.eBreak;
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
            m_createStep = CREATE_STEP.eCreate;
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
                buttonLevel.transform.GetChild(0).gameObject.SetActive(true);
                buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "";

                LevelParam nextParam = target.m_levelInformations[target.m_level + 1];
                m_towerInfoPanelEx.transform.FindChild("Power").GetComponent<Text>().text           = "攻撃力:　　" + nextParam.power;
                m_towerInfoPanelEx.transform.FindChild("Interval").GetComponent<Text>().text        = "発射間隔:　" + nextParam.interval;
                m_towerInfoPanelEx.transform.FindChild("PowerLv").GetComponent<Text>().text         = nextParam.power.ToString();
                m_towerInfoPanelEx.transform.FindChild("IntervalLv").GetComponent<Text>().text      = nextParam.interval.ToString();

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
        if ( m_itemController.GetForcus() == -1 )   return;
        if ( m_createStep != CREATE_STEP.eCreate)   return;

        Vector3 computePosition = m_resourceInformation.m_gridSplitSpacePlane.transform.position;
		var param = m_resourceInformation.GetResourceParamFromPosition( computePosition );
        if ( m_createStep == CREATE_STEP.eCreate && param == null )
        {
            // 建築
            result = RESULT.eOK;
            //  効果音再生  
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
        }
    }

}

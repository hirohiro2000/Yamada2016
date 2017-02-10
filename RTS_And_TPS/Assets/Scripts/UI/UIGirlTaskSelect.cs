using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// UIと女の子のやり取りを補助するクラス
public class UIGirlTaskSelect : MonoBehaviour
{
    //  ダブルクリックチェック
    class DoublePushChecker
    {
        public enum State
        {
            Neutral,
            Push,
            DoublePush,
        }

        private int   m_mouseIndex  = 0;
        private float m_threshold   = 0.0f;
        private float m_pushTimer   = 0.0f;
        private State m_state       = State.Neutral;
        private State m_keyState    = State.Neutral;

        public DoublePushChecker(int mouseIndex, float threshold)
        {
            m_mouseIndex = mouseIndex;
            m_threshold  = threshold;
        }

        public  void Update()
        {
            //  状態に応じた処理を行う
            switch (m_state)
            {
                case State.Neutral:    Update_Neutral();    break;
                case State.Push:       Update_Push();       break;
            }
        }
        private void Update_Neutral()
        {
            //  入力があれば一回押された状態へ
            if (Input.GetMouseButtonDown(m_mouseIndex))
            {
                m_pushTimer     = 0.0f;
                m_state         = State.Push;
                m_keyState      = State.Neutral;
            }
            else
            {
                m_keyState      = State.Neutral;
            }
        }
        private void Update_Push()
        {
            //  一定時間以内に入力があれば次の状態へ
            if (Input.GetMouseButtonDown(m_mouseIndex))
            {
                m_state    = State.Neutral;
                m_keyState = State.DoublePush;
                return;
            }

            //  一回目の入力から一定時間経過でニュートラルに戻る
            m_pushTimer += Time.deltaTime;
            m_pushTimer = Mathf.Min(m_pushTimer, m_threshold);
            if (m_pushTimer >= m_threshold)
            {
                m_state    = State.Neutral;
                m_keyState = State.Push;
            }
        }
        
        public State GetDown()
        {
            return m_keyState;
        }
        public void  Reset()
        {
            m_pushTimer     = 0.0f;
            m_state         = State.Neutral;
            m_keyState      = State.Neutral;
        }
    }


    public	GameObject			m_uiResourceBG   			        = null;
    public	GameObject			m_uiForcusFrame 			        = null;
    public	GameObject			m_uiConvert 				        = null;
	public	GameObject			m_towerInfoPanel			        = null;
	public	GameObject			m_towerInfoPanelEx			        = null;
	public	GameObject			m_towerInfoPanelCrystalFarm			= null;
	public	GameObject			m_towerInfoPanelCrystalFarmEx       = null;
	public	GameObject			m_workingAreaAsset                  = null;

    public  Vector3             m_editTargetPosition        { get; set; }
    public  Vector3             m_computePosition           { get; set; }

    private ResourceInformation m_resourceInformation		{ get; set; }
    private ResourceCreator     m_resourceCreator           { get; set; }
	private ItemController		m_itemController            { get; set; }
    private GameObject          m_convertObject             { get; set; }
    private GameObject          m_workingArea               { get; set; }

    private bool                m_isInit                    { get; set; }
    private DoublePushChecker   m_rightDoublePushChecker    { get; set; }


    public  enum RESULT
    {
        eNone,
        eOK,
        eCancel,
        eLevel,
        eBreak,
        eConfirming,
        eErr = -1,
    }
    private RESULT result               { get; set; }

    // [GirlController]専用の初期化関数
    public void Initialize( GirlController girl )
    {
        result                      = RESULT.eNone;
        m_rightDoublePushChecker    =   new DoublePushChecker( 2, 0.2f );

        m_itemController         = girl.GetComponent<ItemController>();
        m_itemController.SetForcus( 0 );
        m_resourceInformation    = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
        m_resourceCreator        = GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();

        m_uiResourceBG.GetComponent<UIForcusSelect>().Initialize( this, m_itemController );
        m_uiForcusFrame.GetComponent<UIForcusFrame>().Initialize( this, m_itemController );
        m_uiConvert.GetComponent<UIConfirm>().Initialize( this, m_itemController );

        if ( m_workingArea != null )    Destroy(m_workingArea);
        m_workingArea = Instantiate(m_workingAreaAsset);
        m_workingArea.SetActive(false);

    }

    // ＵＩ情報を更新をして行うべきタスクを戻り値として返す
    public RESULT Select( Vector3 girlPosition, bool isJump )
    {
        if ( m_isInit == false )
        {
            m_isInit = true;
            return result;
        }
        
        m_computePosition = girlPosition;
        m_rightDoublePushChecker.Update();
        UpdateGuide( girlPosition );
        UpdateEvent( girlPosition, isJump );                    
        UpdateEveryTime( girlPosition );

        if (m_uiConvert.activeInHierarchy)
        {
            return RESULT.eConfirming;
        }

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
    void UpdateEvent(Vector3 computePosition, bool isJump )
    {
        if ( !m_uiConvert.activeInHierarchy )
        {
            if ( Input.GetKeyDown(KeyCode.X ) || m_rightDoublePushChecker.GetDown() == DoublePushChecker.State.Push || ( m_uiResourceBG.activeInHierarchy && Input.GetKey(KeyCode.F) ) )
            {
                bool active0 = !m_uiForcusFrame.activeInHierarchy;
                m_uiForcusFrame.SetActive( active0 );

                bool active1 = !m_uiResourceBG.activeInHierarchy;
                m_uiResourceBG.SetActive( active1 );

                m_rightDoublePushChecker.Reset();

                // 効果音再生
                SoundController.PlayNow( "UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f );

            }        
            else if ( !m_uiResourceBG.activeInHierarchy && !isJump && ( Input.GetKeyDown( KeyCode.F ) ) )
            {
                ClikedForcusFrame();
                m_rightDoublePushChecker.Reset();
            }

        }
        else
        {
            if ( Input.GetKeyDown( KeyCode.F ) )
            {
                SelectLevel();
            }
            if ( Input.GetKeyDown( KeyCode.C ) )
            {
                SelectBreak();                 
            }
            if ( Input.GetKeyDown(KeyCode.X ) || m_rightDoublePushChecker.GetDown() == DoublePushChecker.State.Push || isJump || !InRangeWorkArea(computePosition) )
            {
                m_uiConvert.SetActive( false );

                m_rightDoublePushChecker.Reset();

                // 効果音再生
                SoundController.PlayNow( "UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f );
            }
        }

    }
    void UpdateEveryTime(Vector3 computePosition)
    {
        m_uiForcusFrame.transform.GetChild(1).gameObject.SetActive( m_itemController.GetHaveCost() < m_itemController.GetForcusResourceParam().GetCreateCost() );
        m_uiConvert.GetComponent<UIConfirm>().m_target = m_editTargetPosition;

        SetInfopanelEx( computePosition );

    }

    //---------------------------------------------------------------------
    //      ガイド情報の更新
    //---------------------------------------------------------------------   	
    void UpdateGuide(Vector3 computePosition)
    {
        if ( m_uiConvert.activeInHierarchy )
        {
            // ガイドの位置補正
            RaycastHit  rHit        = new RaycastHit();
            Ray         rRay        = new Ray( computePosition + Vector3.up*2.0f, Vector3.down );
            int         layerMask   = LayerMask.GetMask( "Field" );
            //  レイ判定
            if (Physics.Raycast(rRay, out rHit, float.MaxValue, layerMask))
            {
                computePosition = rHit.point;
            }

            var param = m_resourceInformation.GetResourceParamFromPosition(m_editTargetPosition);
            if ( param )
            {
                m_resourceCreator.UpdateGuideRange( m_editTargetPosition, param.GetCurLevelParam().range );
                m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
                m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);
            }
            else
            {
                m_resourceCreator.UpdateGuideResource(m_itemController.GetForcus(), m_editTargetPosition);
                m_resourceCreator.UpdateGuideRange( m_itemController.GetForcus(), m_editTargetPosition );
            }
            
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_workingArea.SetActive(true);
            m_workingArea.transform.position = m_editTargetPosition;           

        }
        else if ( m_uiResourceBG.activeInHierarchy )
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);
            m_resourceCreator.UpdateGuideResource(m_itemController.GetForcus(), computePosition);
            m_resourceCreator.UpdateGuideRange( m_itemController.GetForcus(), computePosition );
        }
        else
        {
            m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position  = computePosition;
            m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);
            m_resourceCreator.SetGuideVisibleDisable();
            m_workingArea.SetActive(false);
        }


        if ( m_uiConvert.activeInHierarchy )
        {
            var param = m_resourceInformation.GetResourceParamFromPosition(m_editTargetPosition);
            if ( param != null && m_convertObject != param.gameObject )
            {
                if ( m_convertObject != null )
                {
                    m_convertObject.GetComponent<MaterialSwitchToConvert>().Deactivate();
                    m_convertObject.GetComponent<MaterialSwitchToConvert>().enabled = false;
                }

                m_convertObject = param.gameObject;
                m_convertObject.GetComponent<MaterialSwitchToConvert>().enabled = true;
                m_convertObject.GetComponent<MaterialSwitchToConvert>().Activate();
            }
        }
        else
        {
            if ( m_convertObject != null )
            {
                m_convertObject.GetComponent<MaterialSwitchToConvert>().Deactivate();
                m_convertObject.GetComponent<MaterialSwitchToConvert>().enabled = false;
                m_convertObject = null;
            }
        }


    }

    //---------------------------------------------------------------------
    //      判定
    //---------------------------------------------------------------------   	
    public bool canCreate( Vector3 pos )
    {
        var param   = m_resourceInformation.GetResourceParamFromPosition(pos);
        if( param != null )     return false;

        int resorceID = m_itemController.GetForcus();
        if ( resorceID == -1 )   return false;
        if ( m_itemController.GetHaveCost() < m_itemController.GetForcusResourceParam().GetCreateCost() )  return false;
        
        return true;
    }
    public bool canLevelUP( Vector3 pos )
    {
        var param   = m_resourceInformation.GetResourceParamFromPosition(pos);
        if ( param == null )     return false;

        if ( m_itemController.GetHaveCost() < param.GetCurLevelParam().GetUpCost() )    return false;
        if ( param.CheckWhetherCanUpALevel() == false )                                 return false;
                
        return true;

    }
    public bool InRangeWorkArea( Vector3 pos )
    {
        Vector3 local = pos - m_editTargetPosition;

        float halfScale = m_resourceInformation.m_gridSplitSpaceScale*0.5f;

        if ( local.x > halfScale )      return false;
        if ( local.x < -halfScale )     return false;
        if ( local.z > halfScale )      return false;
        if ( local.z < -halfScale )     return false;
                   
        return true;
    }

    //---------------------------------------------------------------------
    //      
    //---------------------------------------------------------------------   	
    public void SetForcus( int forcusID )
    {
        string[]    paramStr    =   new string[]{
            "15/30/45",
            "30/60/90",
            "45/90/135",
            "45/90/135",
        };

        // フォーカス変更
        m_itemController.SetForcus( forcusID );

        if (forcusID != -1)
        {                   
		    //	リソースのUI設定
		    var param = m_itemController.GetResourceParam( forcusID );
            if ( param.gameObject.GetComponent<CrystalFarm_Control>() == null )
            {
                m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(true);
    		    m_towerInfoPanel.transform.FindChild("Kind").GetComponent<Text>().text      = param.m_name;
    		    m_towerInfoPanel.transform.FindChild("Summary").GetComponent<Text>().text   = "概要:　　　" + param.m_summary;
                m_towerInfoPanel.transform.FindChild("HP").GetComponent<Text>().text        = "体力:  　　 " + param.GetLevelParam(0).hp;
                m_towerInfoPanel.transform.FindChild("Power").GetComponent<Text>().text     = "攻撃力:　　" + param.GetLevelParam(0).power;
                m_towerInfoPanel.transform.FindChild("Interval").GetComponent<Text>().text  = "発射間隔:　" + param.GetLevelParam(0).interval;
                m_towerInfoPanel.transform.FindChild("Range").GetComponent<Text>().text     = "射程距離:　" + param.GetLevelParam(0).range;

                m_towerInfoPanelCrystalFarm.GetComponent<AppearInfopanel>().SetActive(false);

            }
            else
            {
                m_towerInfoPanelCrystalFarm.GetComponent<AppearInfopanel>().SetActive(true);
                m_towerInfoPanelCrystalFarm.transform.FindChild("HP").GetComponent<Text>().text         = "体力:  　　" + param.GetLevelParam(param.m_level).hp;
                m_towerInfoPanelCrystalFarm.transform.FindChild("Interval").GetComponent<Text>().text   = "生成量:　    " + paramStr[ param.m_level ] + "      秒/発";

                m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(false);
            }

            // 効果音再生
            SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.1f, Random.Range( 0.95f, 1.05f ), 1.0f );
        }
        else
        {
            m_towerInfoPanel.GetComponent<AppearInfopanel>().SetActive(false);
            m_towerInfoPanelCrystalFarm.GetComponent<AppearInfopanel>().SetActive(false);
        }

    }
    public void SetInfopanelEx( Vector3 pos )
    {
        string[]    paramStr    =   new string[]{
            "15/30/45",
            "30/60/90",
            "45/90/135",
            "45/90/135",
        };


        var param = m_resourceInformation.GetResourceParamFromPosition(pos);
        if ( param && !TowerInfoActiveInHierarchy() )
        {          
            if ( param.gameObject.GetComponent<CrystalFarm_Control>() == null )
            {
                m_towerInfoPanelEx.GetComponent<AppearInfopanel>().SetActive(true);
                m_towerInfoPanelEx.transform.FindChild("Kind").GetComponent<Text>().text       = param.m_name;
                m_towerInfoPanelEx.transform.FindChild("HP").GetComponent<Text>().text         = "体力:  　　" + param.GetLevelParam(param.m_level).hp;
                m_towerInfoPanelEx.transform.FindChild("Power").GetComponent<Text>().text      = "攻撃力:　　" + param.GetLevelParam(param.m_level).power;
                m_towerInfoPanelEx.transform.FindChild("Interval").GetComponent<Text>().text   = "発射間隔:　" + param.GetLevelParam(param.m_level).interval;
                m_towerInfoPanelEx.transform.FindChild("Range").GetComponent<Text>().text      = "射程距離:　" + param.GetLevelParam(param.m_level).range;
    
                int refLv = ( param.CheckWhetherCanUpALevel() ) ? param.m_level+1 : param.m_level;
    
                m_towerInfoPanelEx.transform.FindChild("HPLv").GetComponent<Text>().text         = param.GetLevelParam(refLv).hp.ToString();
                m_towerInfoPanelEx.transform.FindChild("PowerLv").GetComponent<Text>().text      = param.GetLevelParam(refLv).power.ToString();
                m_towerInfoPanelEx.transform.FindChild("IntervalLv").GetComponent<Text>().text   = param.GetLevelParam(refLv).interval.ToString();
                m_towerInfoPanelEx.transform.FindChild("RangeLv").GetComponent<Text>().text      = param.GetLevelParam(refLv).range.ToString();

                m_towerInfoPanelCrystalFarmEx.GetComponent<AppearInfopanel>().SetActive(false);

            }
            else
            { 
                m_towerInfoPanelCrystalFarmEx.GetComponent<AppearInfopanel>().SetActive(true);
                m_towerInfoPanelCrystalFarmEx.transform.FindChild("HP").GetComponent<Text>().text           = "体力:  　　" + param.GetLevelParam(param.m_level).hp;
                m_towerInfoPanelCrystalFarmEx.transform.FindChild("Interval").GetComponent<Text>().text     = "生成量:　    " + paramStr[ param.m_level ];//"＃＃＃＃＃＃";

                int refLv = ( param.CheckWhetherCanUpALevel() ) ? param.m_level+1 : param.m_level;
    
                m_towerInfoPanelCrystalFarmEx.transform.FindChild("HPLv").GetComponent<Text>().text         = param.GetLevelParam(refLv).hp.ToString();
                m_towerInfoPanelCrystalFarmEx.transform.FindChild("IntervalLv").GetComponent<Text>().text   = paramStr[ refLv ];//"＃＃＃＃＃＃";

                m_towerInfoPanelEx.GetComponent<AppearInfopanel>().SetActive(false);
            }

        }
        else
        {
            m_towerInfoPanelEx.GetComponent<AppearInfopanel>().SetActive(false);
            m_towerInfoPanelCrystalFarmEx.GetComponent<AppearInfopanel>().SetActive(false);
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
        Vector3 target  = m_resourceInformation.m_gridSplitSpacePlane.transform.position;

        if (canCreate(target))
        {
            result = RESULT.eOK;
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
        }
        else if (canLevelUP(target))
        {
            result = RESULT.eLevel;
        }
        else
        {
            //  効果音再生
            SoundController.PlayNow("UI_NG", 0.0f, 0.1f, 0.64f, 1.0f);
        }

        m_uiConvert.GetComponent<UIConfirm>().Cliked( result );

    }
    public void SelectBreak()

    {
        var param   = m_resourceInformation.GetResourceParamFromPosition(m_editTargetPosition);

        if (param != null)
        {
            result = RESULT.eBreak;
        }
        else           
        {
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
        }
               
        m_uiConvert.GetComponent<UIConfirm>().Cliked( result );
    }
    public void ClikedForcusFrame()
    {
        if (m_uiConvert.activeInHierarchy)  return;

        m_editTargetPosition = m_resourceInformation.m_gridSplitSpacePlane.transform.position;
        RaycastHit  rHit        = new RaycastHit();
        Ray         rRay        = new Ray(m_editTargetPosition + Vector3.up * 2.0f, Vector3.down );
        int         layerMask   = LayerMask.GetMask( "Field" );
        //  レイ判定
        if (Physics.Raycast(rRay, out rHit, float.MaxValue, layerMask))
        {
            m_editTargetPosition = rHit.point;
        }


        var param = m_resourceInformation.GetResourceParamFromPosition(m_editTargetPosition);
        if (param == null)
        {
            if (canCreate(m_editTargetPosition))
            {         
                m_uiConvert.SetActive( true );
                m_uiConvert.GetComponent<UIConfirm>().SetMode(UIConfirm.MODE.eCreate, m_itemController.GetForcusResourceParam() );
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
        else
        {
            m_uiConvert.SetActive( true );
            m_uiConvert.GetComponent<UIConfirm>().SetMode( UIConfirm.MODE.eConvert, param );
            //  効果音再生  
            SoundController.PlayNow("UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f);
            SoundController.PlayNow("UI_Click", 0.0f, 0.1f, 0.84f, 1.0f);
        }


    }         

    //---------------------------------------------------------------------
    //      
    //---------------------------------------------------------------------   	
    // [m_uiResourceBG][m_uiForcusFrame][m_towerInfoPanel]を上２０に移動
    public void UpPanel()
    {
        m_uiResourceBG.GetComponent<RectTransform>().localPosition                  += new Vector3( 0.0f, 35.0f, 0.0f );
        m_uiForcusFrame.GetComponent<RectTransform>().localPosition                 += new Vector3( 0.0f, 35.0f, 0.0f );
	    m_towerInfoPanel.GetComponent<RectTransform>().localPosition                += new Vector3( 0.0f, 35.0f, 0.0f );
	    m_towerInfoPanelCrystalFarm.GetComponent<RectTransform>().localPosition     += new Vector3( 0.0f, 35.0f, 0.0f );
    }   
 
    
    public bool TowerInfoActiveInHierarchy()
    {
        return m_towerInfoPanel.GetComponent<AppearInfopanel>().IsActive() || m_towerInfoPanelCrystalFarm.GetComponent<AppearInfopanel>().IsActive();
    }
    public bool TowerInfoExActiveInHierarchy()
    {
        return m_towerInfoPanelEx.GetComponent<AppearInfopanel>().IsActive() || m_towerInfoPanelCrystalFarmEx.GetComponent<AppearInfopanel>().IsActive();
    }
    public void ForceClear()
    {              
        m_uiForcusFrame.SetActive(true);
        m_uiResourceBG.SetActive(false);
        m_uiConvert.SetActive(false);

        m_rightDoublePushChecker.Reset();
    }
                      
}


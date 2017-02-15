using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIConfirm : MonoBehaviour
{
    public enum MODE
    {
        eNone,
        eCreate,
        eConvert
    }
    public Vector3                      m_target { get; set; }

    private MODE                        m_mode                      { get; set; }
    private RectTransform               m_parentTrans               { get; set; }
    private Camera                      m_uiCamera                  { get; set; }
    private Canvas                      m_uiCanvas                  { get; set; }
    private UIGirlTaskSelect            m_girlTask                  { get; set; }
    private ItemController              m_itemController            { get; set; }
    private GameObject                  m_clikedUI                  { get; set; }
    private Vector3[]                   m_firstLocalPosition        { get; set; }
    private ResourceParameter           m_targetParam               { get; set; }

    public void Initialize( UIGirlTaskSelect girlTask, ItemController itemController )
    {
        m_girlTask          = girlTask;
        m_itemController    = itemController;

        RectTransform rt = GetComponent<RectTransform>();
        m_parentTrans = rt.parent.GetComponent<RectTransform>();

        Canvas[] canvasArr = transform.GetComponentsInParent<Canvas>();
        for (int i = 0; i < canvasArr.Length; i++)
        {
            if (canvasArr[i].isRootCanvas)
            {
                m_uiCamera = canvasArr[i].worldCamera;
                m_uiCanvas = canvasArr[i];
            }
        }
        
        m_firstLocalPosition = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            m_firstLocalPosition[i] = transform.GetChild(i).GetComponent<RectTransform>().transform.localPosition;
        }

    }
    public void Update ()
    {
        // ＵＩの位置ターゲットの位置によって更新します
        Vector3 screenPos = Camera.main.WorldToScreenPoint(m_target);
        Vector2 localPos  = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_parentTrans, screenPos, m_uiCamera, out localPos);
        transform.GetComponent<RectTransform>().localPosition = localPos;

        if ( m_mode == MODE.eConvert )
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive( !( m_targetParam.CheckWhetherCanUpALevel() && m_targetParam.GetCurLevelParam().GetUpCost() <= m_itemController.GetHaveCost() ) );
        }

        if ( m_clikedUI )
        {
            Close();
        }

    }
    public void SetMode( MODE mode, ResourceParameter targetParam )
    {
        m_clikedUI      = null;
        m_targetParam   = targetParam;
        m_mode          = mode;
        if (mode == MODE.eNone)
        {

        }
        if (mode == MODE.eCreate)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        if (mode == MODE.eConvert)
        {            
            transform.GetChild(0).gameObject.SetActive( targetParam.CheckWhetherCanUpALevel() );
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
        }

        transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "-" + targetParam.GetCurLevelParam().GetUpCost();
        transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "+" + targetParam.GetBreakCost();
        transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "-" + targetParam.GetCreateCost();

    }
    public void Cliked( UIGirlTaskSelect.RESULT type )
    {
        if (type == UIGirlTaskSelect.RESULT.eLevel)
        {
            m_clikedUI = transform.GetChild(0).gameObject;
        }
        if (type == UIGirlTaskSelect.RESULT.eBreak)
        {
            m_clikedUI = transform.GetChild(1).gameObject;
        }
        if (type == UIGirlTaskSelect.RESULT.eOK)
        {            
            m_clikedUI = transform.GetChild(2).gameObject;
        }
    }
    public void Close()
    {
        bool isClose = true;
        for (int i = 0; i < 3; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            RectTransform rt = obj.GetComponent<RectTransform>();
            RectTransform parentRt = obj.transform.parent.GetComponent<RectTransform>();

            if ( obj == m_clikedUI )
            {
                rt.localScale = Vector3.Lerp( rt.localScale, Vector3.zero, 20.0f*Time.deltaTime );
                rt.localPosition += ( parentRt.localPosition - rt.localPosition )*5.0f*Time.deltaTime;
            }                
            else
            {
                rt.localScale = Vector3.Lerp( rt.localScale, Vector3.zero, 50.0f*Time.deltaTime );
                rt.localPosition += ( parentRt.localPosition - rt.localPosition )*15.0f*Time.deltaTime;
            }

            

            if ( rt.localScale.sqrMagnitude > 0.01f )
            {
                isClose = false;
            }
        }

        if ( isClose )
        {
            m_clikedUI = null;
            gameObject.SetActive(false);

            for (int i = 0; i < 3; i++)
            {
                GameObject obj = transform.GetChild(i).gameObject;
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.localScale       = Vector3.one;
                rt.localPosition    = m_firstLocalPosition[i];
            }


        }



    }
    public bool IsClose()
    {
        return m_clikedUI == null || !gameObject.activeInHierarchy;
    }

}

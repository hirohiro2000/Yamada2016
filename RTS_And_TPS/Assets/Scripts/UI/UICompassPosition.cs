using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICompassPosition : MonoBehaviour
{
    [SerializeField]
    private float   m_uiCircle          = 45.0f;

    public float    m_clipDistanceSq    = 300.0f;
    public Vector3  m_rTargetPos        = Vector3.zero;

    private RectTransform   m_parentTrans   = null;
    private Camera          m_uiCamera      = null;
    private Canvas          m_uiCanvas      = null;

    void Start()
    {

        RectTransform rt = GetComponent<RectTransform>();
        m_parentTrans = rt.parent.GetComponent<RectTransform>();

        Canvas[] canvasArr = GetComponentsInParent<Canvas>();
        for (int i = 0; i < canvasArr.Length; i++)
        {
            if (canvasArr[i].isRootCanvas)
            {
                m_uiCamera = canvasArr[i].worldCamera;
                m_uiCanvas = canvasArr[i];
            }
        }

    }
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(m_rTargetPos);

        Vector2 localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_parentTrans, screenPos, m_uiCamera, out localPos);

        Vector2 clampSize = new Vector2( 1100.0f, 576.0f );

        float dz       = Vector3.Dot( Camera.main.transform.forward, ( m_rTargetPos - Camera.main.transform.position ) );
        float distance = ( m_rTargetPos - Camera.main.transform.position ).sqrMagnitude;

        bool isInScreen = (localPos.x  >= -clampSize.x*0.5f && localPos.x <= clampSize.x*0.5f) &&
                          (localPos.y  >= -clampSize.y*0.5f && localPos.y <= clampSize.y*0.5f) && 
                          ( dz         >= 0.0f );
        
        //　※距離制限は適当
        if ( isInScreen && ( distance <= m_clipDistanceSq ) )
        { 
            GetComponent<RawImage>().enabled = false;
            transform.GetChild(0).GetComponent<Image>().enabled = false;
            return;
        }

        GetComponent<RawImage>().enabled = true;

        if (dz < 0.0f)
        {              
            localPos.x = -localPos.x;
            if ( localPos.y < 0.0f )
            { 
                localPos.y = -10000.0f;
            }
            else
            {
                localPos.y = -10000.0f;
            }
        }

        RectTransform rt = GetComponent<RectTransform>();
        localPos.x = Mathf.Clamp( localPos.x, -clampSize.x*0.5f, clampSize.x*0.5f );
        localPos.y = Mathf.Clamp( localPos.y, -clampSize.y*0.5f, clampSize.y*0.5f );



        rt.localPosition = localPos;                      

        RectTransform rtArrow = transform.GetChild(0).GetComponent<RectTransform>();
       
        float angle = Mathf.Atan2(localPos.y, localPos.x);
        rtArrow.eulerAngles = new Vector3(rtArrow.eulerAngles.x, rtArrow.eulerAngles.y, (angle-Mathf.PI*0.5f) * Mathf.Rad2Deg);

        rtArrow.localPosition = new Vector3( Mathf.Cos(angle), Mathf.Sin(angle), 0.0f ) * m_uiCircle;

        transform.GetChild(0).GetComponent<Image>().enabled = !isInScreen;

    }

}

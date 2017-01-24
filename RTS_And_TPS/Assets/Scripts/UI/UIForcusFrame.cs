using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIForcusFrame : MonoBehaviour
{
    int              m_curForcus        { get; set; }
    ResourceCreator  m_resourceCreator  { get; set; }
    UIGirlTaskSelect m_girlTask         { get; set; }
    ItemController   m_itemController   { get; set; }
    
    public  void Initialize( UIGirlTaskSelect girlTask, ItemController itemController )
    {
	    m_curForcus         = -1;
        m_girlTask          = girlTask;
        m_itemController    = itemController;
        m_resourceCreator   = GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
    }
	private void OnEnable ()
    {
        if ( m_itemController   == null )   return;
        if ( m_itemController.GetForcus() != -1 )
        {
            m_curForcus = m_itemController.GetForcus();
            Text text = transform.GetChild(2).GetComponent<Text>();
            ResourceParameter resource = m_resourceCreator.m_resources[ m_itemController.GetForcus() ].GetComponent<ResourceParameter>();
            text.text = resource.GetCreateCost().ToString();
            transform.GetChild(0).GetComponent<RawImage>().texture = m_resourceCreator.m_textures[ m_itemController.GetForcus() ].GetComponent<Image>().mainTexture;
        }
        InputFlushSetActive(true);
	}
	private void OnDisable ()
    {

    }	
	private void Update ()
    {
        if ( m_girlTask         == null )   return;
        if ( m_itemController   == null )   return;
        if (m_girlTask.m_uiConvert.activeInHierarchy)
        {
            InputFlushSetActive(false);
            return;
        }
        
        InputFlushSetActive(true);
        if ( Input.GetKeyDown(KeyCode.E) )
        {
            int forcus = m_itemController.GetForcus();
            forcus = ( forcus+1 ) % m_itemController.GetNumKind();
            m_itemController.SetForcus( forcus );
        }
	    if ( Input.GetKeyDown(KeyCode.Q) )
        {
            int forcus = m_itemController.GetForcus();
            forcus = ( forcus+m_itemController.GetNumKind()-1 ) % m_itemController.GetNumKind();
            m_itemController.SetForcus( forcus );
        }

        if ( m_curForcus != m_itemController.GetForcus() )
        {
            m_curForcus = m_itemController.GetForcus();
            Text text = transform.GetChild(2).GetComponent<Text>();
            ResourceParameter resource = m_resourceCreator.m_resources[ m_itemController.GetForcus() ].GetComponent<ResourceParameter>();
            text.text = resource.GetCreateCost().ToString();
            transform.GetChild(0).GetComponent<RawImage>().texture = m_resourceCreator.m_textures[ m_itemController.GetForcus() ].GetComponent<Image>().mainTexture;

            // 効果音再生
            SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.1f, Random.Range( 0.95f, 1.05f ), 1.0f );

        }


	}
    public  void InputFlushSetActive( bool active )
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i+3).gameObject.SetActive( active );
        }
    }

}

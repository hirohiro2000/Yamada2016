using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TitleUIController : MonoBehaviour {


    [SerializeField]
    private string GameSceneName = "MainGame_T2";

    private GameObject m_menu_button_root = null;
    private GameObject m_staff_credit_root = null;
    private Image            m_title_image = null;

    void Awake()
    {
        m_menu_button_root = transform.FindChild("MemuButtonRoot").gameObject;
        m_title_image = transform.FindChild("TitleImage").GetComponent<Image>();
        m_staff_credit_root = transform.FindChild("StaffCreditRoot").gameObject;
        m_staff_credit_root.GetComponent<StaffCredit>().SetCreditEndNotifyFunction(EndCredit);
    }

    void Start()
    {
     
    }

    public void ChangeSceneSinglePlayer()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneName);
    }
       
    public void ChangeSceneMultiPlayer()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneName);
    }

    public void BeginCredit()
    {
       
        DisableAllButton();
        m_staff_credit_root.SetActive(true);
    }

    private void EndCredit()
    {
        EnableAllButton();
        m_staff_credit_root.SetActive(false);
    }

    void DisableAllButton()
    {
        var text_root = m_menu_button_root.GetComponentsInChildren<Text>();
        foreach (var text in text_root)
            text.color = Color.white;
      
        m_menu_button_root.SetActive(false);
        m_title_image.enabled = false;
    }

    void EnableAllButton()
    {
        m_menu_button_root.SetActive(true);
        m_title_image.enabled = true;
    }

}

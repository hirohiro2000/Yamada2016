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
    private GameObject            m_title_button = null;
    private FadeController m_fade_controller = null;

    void Awake()
    {
        m_menu_button_root = transform.FindChild("MemuButtonRoot").gameObject;
        m_fade_controller = GameObject.Find("FadeObject").GetComponent<FadeController>();
        m_title_button = transform.FindChild("TitleButton").gameObject;
        m_staff_credit_root = transform.FindChild("StaffCreditRoot").gameObject;
        m_staff_credit_root.GetComponent<StaffCredit>().SetCreditEndNotifyFunction(EndCredit);
    }

    void Start()
    {
        m_fade_controller.BeginFade(FadeController.FadeMode.In, Color.black, 1.0f, () =>
           {
               //Debug.Log("done");
           });
    }

    public void ChangeSceneSinglePlayer()
    {

        m_fade_controller.BeginFade(
            FadeController.FadeMode.Out, Color.black, 1.0f,
            () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneName);
            });
    }
       
    public void ChangeSceneMultiPlayer()
    {
        m_fade_controller.BeginFade(
    FadeController.FadeMode.Out, Color.black, 1.0f,
    () =>
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneName);
    });
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
        m_title_button.SetActive(false);
    }

    void EnableAllButton()
    {
        m_menu_button_root.SetActive(true);
        m_title_button.SetActive(true);
    }

}

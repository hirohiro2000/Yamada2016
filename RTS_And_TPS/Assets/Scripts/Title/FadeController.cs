using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeController : MonoBehaviour {

    public enum FadeMode
    {
        In,
        Out,
    }

    [SerializeField,HeaderAttribute("初期のフェードの状態")]
    private FadeMode DefaultMode;

    [SerializeField, HeaderAttribute("初期のフェードポリゴンの色")]
    private Color DefaultColor = Color.black;

    private Image        m_image = null;
    private float           m_fade_time_second = 1.0f;
    private float           m_fade_rate = .0f;
    private FadeMode m_current_mode = FadeMode.In;
    private Color m_image_color = Color.black;

    public delegate void FadeEndCallBack();
    private FadeEndCallBack EndNotifyFunction = null;

    private float m_current_alpha = .0f;
    private float m_target_alpha = .0f;

    // Use this for initialization
    void Awake()
    {
        m_current_mode = DefaultMode;
        m_image = GetComponent<Image>();
        if (DefaultMode == FadeMode.In)
        {
            m_current_alpha = .0f;
            m_image.color = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.g, .0f);
        }
        else
        {
            m_current_alpha = 1.0f;
            m_image.color = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.g, 1.0f);
        }
    }

    private bool IsFinish()
    {
        if (m_current_mode == FadeMode.In)
            if (m_current_alpha <= .0f)
                return true;
            else
                return false;

        if (m_current_alpha >= 1.0f)
            return true;
        return false;
    }

    IEnumerator Execute()
    {
        while(true)
        {
            m_current_alpha += m_fade_rate;
            m_image.color = new Color(m_image_color.r, m_image_color.g, m_image_color.b, m_current_alpha);
            if (IsFinish())
                break;
            
            
            yield return null;
        }
        EndNotifyFunction();
    }

    public void BeginFade(FadeMode mode,
        Color color,
        float end_time_second,
        FadeEndCallBack end_callback = null)
    {
        if (mode == m_current_mode)
            return;
        if (mode == FadeMode.In)
        {
            m_current_alpha = 1.0f;
            m_target_alpha = .0f;
        }
        else
        {
            m_current_alpha = .0f;
            m_target_alpha = 1.0f;
        }
        m_image_color = color;
        EndNotifyFunction = end_callback;
        m_current_mode = mode;
        CalculateFadeRate(end_time_second);
       
        StartCoroutine(Execute());
    }

    private void CalculateFadeRate(float end_time)
    {
        float rate =  1.0f / (end_time * 60.0f);
        if (m_current_mode == FadeMode.In)
            rate *= -1.0f;
        m_fade_rate = rate;
    }
	
}

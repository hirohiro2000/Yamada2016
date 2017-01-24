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

    private Image m_image = null;
    private float m_fade_time_second = 1.0f;

	// Use this for initialization
	void Awake()
    {
        m_image = GetComponent<Image>();
        if (DefaultMode == FadeMode.In)
            m_image.color = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.g, .0f);
        else
            m_image.color = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.g, 255.0f);

    }

    public void BeginFade(FadeMode mode,Color color,float end_time_second)
    {

    }
	
	
}

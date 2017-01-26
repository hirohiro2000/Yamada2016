using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RTSOnConvertFrame : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(4).gameObject.SetActive(true);
        GetComponent<Image>().color = new Color(0.13f, 0.13f, 0.13f, 1.0f);
        // 効果音再生
        SoundController.PlayNow("UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(4).gameObject.SetActive(false);
        GetComponent<Image>().color = new Color( 0.11f, 0.11f, 0.11f, 1.0f );
    }
}

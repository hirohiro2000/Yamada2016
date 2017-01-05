using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponAmmoUI : MonoBehaviour
{

	[SerializeField]
	Text ammoText;
	[SerializeField]
	Slider reloadProgress;
	[SerializeField]
	Text havingAmmoText;

	CanvasGroup group = null;

	// Use this for initialization
	void Awake () {
		group = GetComponent<CanvasGroup>();
	}
	

	public void TextUpdate(WeaponParameter param, bool alpha)
	{
		if (alpha)
			group.alpha = 1.0f;
		else
			group.alpha = 0.5f;

		ammoText.text = param.DispAmmo();
		havingAmmoText.text = param.DispHavingAmmo();
		reloadProgress.value = param.DispReloadProgress();
	}
	public void Alpha()
	{
		group.alpha = 0.0f;
	}
	void Update()
	{
		if (group.alpha != .0f)
		{
			gameObject.SetActive(true);
		}
		if (group.alpha == .0f)
		{
			gameObject.SetActive(false);
		}
	}

}

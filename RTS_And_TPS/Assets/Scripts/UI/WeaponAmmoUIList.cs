using UnityEngine;
using System.Collections;

//テストでcntのみ
public class WeaponAmmoUIList : MonoBehaviour {

	[SerializeField]
	int ID;

	WeaponAmmoUI[] weaponAmmoUIs;

	static WeaponAmmoUIList[] lists = null;
	static public WeaponAmmoUIList Aceess(int ID)
	{
		//if (lists == null)
		//	lists = Object.FindObjectsOfType<WeaponAmmoUIList>();

		foreach(WeaponAmmoUIList list in lists)
		{
			if (list.ID == ID)
				return list;
		}
		return null;

	}
	// Use this for initialization
	void Awake () {
		weaponAmmoUIs = GetComponentsInChildren<WeaponAmmoUI>();
        lists = Object.FindObjectsOfType<WeaponAmmoUIList>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Disp(WeaponList[] weapons, int cnt)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			weaponAmmoUIs[i].Alpha();
		}
		for (int i = 0; i < weapons.Length; i++)
		{
			//weaponAmmoUIs[i].gameObject.SetActive(true);
			weaponAmmoUIs[i].TextUpdate(weapons[i].param, cnt == i);
		}
	}
}

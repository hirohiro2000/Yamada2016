using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//テストでcntのみ
public class WeaponAmmoUIList : MonoBehaviour, IDropHandler
{

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

	public void OnDrop(PointerEventData data)
	{
		if (data.pointerDrag != null)
		{
			DragWeapon weapon = data.pointerDrag.GetComponent<DragWeapon>();
			foreach(GameObject obj in data.hovered)
			{
				if(obj.GetComponent< WeaponAmmoUI > () != null)
				{
					//インデックスを検索
					for (int i = 0; i< transform.childCount; i++)
					{
						if (obj == transform.GetChild(i).gameObject)
						{
							TPSShotController.Aceess(ID).ReplaceWeapon(weapon.weapon, i);
						}
					}


				}

			}
        }
	}

	public void SupplyAmmo()
	{
		TPSShotController.Aceess(ID).SupplyAmmo();
    }
}

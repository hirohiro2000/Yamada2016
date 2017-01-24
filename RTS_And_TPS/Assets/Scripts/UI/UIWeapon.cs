using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIWeapon : MonoBehaviour
{
    [SerializeField]
    private string[]            m_nameList   = null;
                    
    private Text                m_name          { get; set; }
    private Text                m_ammo          { get; set; }
    private Text                m_havingAmmo    { get; set; }
    private RawImage            m_input         { get; set; }
    private GameObject          m_reloadTime    { get; set; }

    public void Initialize()
    {
        m_name          = transform.FindChild("Name").GetComponent<Text>();
        m_ammo          = transform.transform.FindChild("Ammo").GetComponent<Text>();           
        m_havingAmmo    = transform.transform.FindChild("HavingAmmo").GetComponent<Text>();     
        m_input         = transform.transform.GetChild(5).GetChild(0).GetComponent<RawImage>();     
        m_reloadTime    = transform.transform.FindChild("ReloadTime").gameObject;
    }
    public void InternalUpdateByShotcontroller( WeaponList reference )
    {
        m_ammo.text         = reference.param.DispAmmo();
        m_havingAmmo.text   = reference.param.DispHavingAmmo();
        m_reloadTime.transform.localScale = new Vector3( 1.0f, reference.param.DispReloadProgress(), 1.0f );
    }
    public void Change( int index )
    {
        m_name.text = m_nameList[index];
    }



}

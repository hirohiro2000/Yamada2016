using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    [SerializeField, HeaderAttribute("最大HP")]
    private float HP = 10.0f;

    private DamageBank m_damage_bank = null;

    void Awake()
    {
        m_damage_bank = transform.GetComponentInChildren<DamageBank>();
        m_damage_bank.DamagedCallback += RecieveDamage;
    } 
       

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void RecieveDamage(float total_damage)
    {
        HP -= 1.0f;
        if(HP < .0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void CorrectionHP(int level,float correcion_rate)
    {
        HP = HP * level * correcion_rate;
    }
}

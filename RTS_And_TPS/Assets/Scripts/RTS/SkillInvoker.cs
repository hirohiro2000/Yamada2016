using UnityEngine;
using System.Collections;

public class SkillInvoker : MonoBehaviour
{
    int m_curMagicalPower { get; set; }

    //  外部へのアクセス
    SkillAccelWorld m_skillAccelWorld;
    SkillRecovery   m_skillRecovery;
    SkillDrone      m_skillDrone;

    // インスタンス
    private static SkillInvoker instance = null;

    private void Awake()
    {
        instance = this;
        m_curMagicalPower   = 0;

        m_skillAccelWorld   = instance.transform.FindChild("SkillAccelWorld").GetComponent<SkillAccelWorld>();
        m_skillRecovery     = instance.transform.FindChild("SkillRecovery").GetComponent<SkillRecovery>();
        m_skillDrone        = instance.transform.FindChild("SkillDrone").GetComponent<SkillDrone>();

    }
    private void OnGUI()
    {
        GUI.Label(new Rect( 10, 130, 100, 100 ), m_curMagicalPower.ToString() );
    }
 
    // ロボットの場合処理は呼ばれません
    static public void StartWave()
    {
        if ( instance == null ) return;

        instance.m_skillAccelWorld.gameObject.SetActive( true );
        instance.m_skillRecovery.gameObject.SetActive( true );
        instance.m_skillDrone.gameObject.SetActive( true );

        instance.m_skillAccelWorld.SkillDeath();
        instance.m_skillRecovery.SkillDeath();
        instance.m_skillDrone.SkillDeath();

        instance.m_curMagicalPower++;

    }    
    static public void UsedMP( int mp )
    {
        if ( instance == null ) return;
        instance.m_curMagicalPower -= mp;
    }
    static public int  CurrentMagicalPower()
    {
        if ( instance == null ) return 0;

        return instance.m_curMagicalPower;
    }
       
}

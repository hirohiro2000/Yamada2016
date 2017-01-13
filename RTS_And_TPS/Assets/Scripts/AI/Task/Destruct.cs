using UnityEngine;
using System.Collections;

public class Destruct : TaskBase {

    [SerializeField, HeaderAttribute("爆発Object")]
    private GameObject ExplosionObject = null;

    [SerializeField, HeaderAttribute("爆発するときの変色していく色")]
    private Color ExplosionColor = Color.red;

    [SerializeField, HeaderAttribute("何秒後に爆発するか（初期値）")]
    private float DefaultExplosionSecond = 0.8f;

    private float m_explosion_second = .0f;

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        m_explosion_second = DefaultExplosionSecond;
    }

    IEnumerator BeginDestruct()
    {
        yield return new WaitForSeconds(m_explosion_second);

        GameObject effect = Instantiate(ExplosionObject);
        effect.transform.position = m_owner_object.transform.position;
        Destroy(m_owner_object);
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        StartCoroutine(BeginDestruct());
    }

    public override Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        return Status.Active;
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        base.Exit(target_system, task_director);
    }

    public override void SetWaveParametor(EnemyWaveParametor wave_param, EnemyPersonalParametor parsonal_param)
    {
        base.SetWaveParametor(wave_param, parsonal_param);
    }

    public override float EvalutionScore(
        TargetingSystem current_target_info, 
        EnemyTaskDirector director)
    {
        return base.EvalutionScore(current_target_info, director);
    }

}

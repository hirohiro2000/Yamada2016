using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDirectorBase : MonoBehaviour {

    protected ReferenceWrapper m_reference_object = null;
    protected EnemyGenerator m_owner_generator = null;
    void Awake()
    {
        m_owner_generator = GetComponent<EnemyGenerator>();
        m_reference_object = GetComponent<ReferenceWrapper>();
    }

    public virtual EnemyGenerator.EnemyData DirectionGenerateEnemy(List<GameObject> candidate_spawn_enemy_list)
    {
        UserLog.Terauchi("EnemyDirectorBase::DirectionGenerateEnemy call !!");
        return null;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDirectorAuto : EnemyDirectorBase {

    private int m_num_type_enemy = 1;
    private int m_num_spawn_point = 1;
    private List<int> m_route_point_list = new List<int>();
    
    void Start()
    {
        var generator = GetComponent<EnemyGenerator>();
        m_num_type_enemy = generator.GetNumEnemyType();
        m_num_spawn_point = generator.GetNumSpawnPointList();
    }

    public override EnemyGenerator.EnemyData DirectionGenerateEnemy()
    {
        return new EnemyGenerator.EnemyData(
            UnityEngine.Random.Range(0, m_num_type_enemy),
            UnityEngine.Random.Range(0, m_num_spawn_point),
            UnityEngine.Random.Range(0, 2));
    }

}

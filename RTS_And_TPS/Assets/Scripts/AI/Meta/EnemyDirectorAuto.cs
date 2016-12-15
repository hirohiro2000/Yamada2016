using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDirectorAuto : EnemyDirectorBase {

    //private int m_num_type_enemy = 1;
    private int m_num_spawn_point = 1;
    private List<int> m_route_point_list = new List<int>();
    
    void Start()
    {
  
     //   m_num_type_enemy = generator.GetNumEnemyType();
        m_num_spawn_point = m_owner_generator.GetNumSpawnPointList();
    }

    public override EnemyGenerator.EnemyData DirectionGenerateEnemy(
        List<GameObject> candidate_spawn_enemy_list)
    {
        return new EnemyGenerator.EnemyData(
            UnityEngine.Random.Range(0, candidate_spawn_enemy_list.Count),
            UnityEngine.Random.Range(0, m_num_spawn_point),
            UnityEngine.Random.Range(0, 2));
    }

}

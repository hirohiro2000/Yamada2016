using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class EnemyGenerator : NetworkBehaviour
{
    public class EnemyData
    {
        public int type;
        public int respawn_point_index;
        public int route_index;

        public EnemyData(int type,
            int respawn_point_index,
            int route_index)
        {
            this.type = type;
            this.route_index = route_index;
            this.respawn_point_index = respawn_point_index;
        }
    }

    public enum GeneratorType
    {
        Auto,
        ForText,
    }



    [SerializeField, HeaderAttribute("生成する敵Prefab")]
    private GameObject[] GenerateEnemyList = new GameObject[1] { null };

    [SerializeField, HeaderAttribute("デバッグ中　いじるな")]
    private int m_num_spawn_one_frame = 1;  //一度にわく敵の数

    private EnemyDirectorBase m_generate_director = null;
    private List<NavigationRouteData> m_navigation_data_list = null;
    private float m_sparn_interval = 1.0f;
    private static readonly int CanAllRoute = 0;
    private bool m_is_running = false;
    private EnemyWaveParametor m_parametor = null;

    [SerializeField]
    bool OneSpawnMove = false;


    //private List<GameObject> m_current_hierarchy_list = new List<GameObject>();
    private List<GameObject> m_rActiveEnemyList = new List<GameObject>();

    [SerializeField, HeaderAttribute("解禁していくEnemyのリスト")]
    private List<GameObjectList> m_unlock_enemy_block_list = new List<GameObjectList>();

    [SerializeField, HeaderAttribute("unlock_enemy_block_listの各要素を解禁していくwaveレベル")]
    private List<int> m_enemy_unlock_wave_list = new List<int>();

    private List<GameObject> m_current_generate_list = new List<GameObject>();

    public List<GameObject> GetCurrentHierachyList()
    {
        return m_rActiveEnemyList;
    }

    public int GetNumEnemyType()
    {
        return GenerateEnemyList.Length;
    }

    public int GetNumSpawnPointList()
    {
        return m_navigation_data_list.Count;
    }



    /**
    *@brief     敵の生成を開始する
    *@param  waveのレベル
    *@param  生成する敵の総数
    *@param  最初の生成までの遅延時間（秒） 
    */
    public void BeginGenerate(int wave_level, int num_spawn, float delay_second)
    {
        m_is_running = true;
        m_parametor.LevelUp();
        //出現数はWavemanagerが管理してたので
        //num_spawn = Mathf.Clamp(
        //    m_parametor.GetNumStartSpawnEnemy() + (m_parametor.GetNumIncrementSpawnEnemy() * (m_parametor.m_current_level -1)),
        //    1,
        //    m_parametor.GetNumMaxSpawnEnemy());
        UnLockEnemy(wave_level);
        StartCoroutine(Execute(wave_level, num_spawn, delay_second));
    }

    /**
    *@brief 敵生成を行ってる途中かどうかを判定する
    */
    public bool IsGeneratingEnemy()
    {
        return m_is_running;
    }

    /**
    *@breif 現在生存している敵の数を取得する
    */
    public int GetCurrentAliveEnemyCount()
    {
        return m_rActiveEnemyList.Count;
    }


    void Update()
    {
        //GameWorldParameterで強制的に書き換える
        {
            //HPCorrectionRate = GameWorldParameter.instance.Enemy.HealthMultiple;
        }

        //  死亡したエネミーの項目を削除
        for (int i = 0; i < m_rActiveEnemyList.Count; i++) {
            m_rActiveEnemyList.Remove(null);
        }
    }

    void Awake()
    {
        //if(GenerateType == GeneratorType.Auto)
        //{
        gameObject.AddComponent<EnemyDirectorAuto>();
        //}            
        //else
        //{
        //    UserLog.Terauchi("for text generate no support ");
        //}
        m_generate_director = GetComponent<EnemyDirectorBase>();
        m_parametor = GetComponent<EnemyWaveParametor>();
        m_navigation_data_list = new List<NavigationRouteData>();
        InitializeSpawnPoint();
        //UnLockEnemy(0);
    }


    void InitializeSpawnPoint()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var nav_data = transform.GetChild(i).GetComponent<NavigationRouteData>();
            if (nav_data)
                m_navigation_data_list.Add(nav_data);
            else
                UserLog.ErrorTerauchi(transform.GetChild(i).name + "not attach NavigationRouteData");
        }
    }
    public void InitializeSpawnPoint(List<Transform> _rSpawnPointList)
    {
        //  リストをクリア
        m_navigation_data_list.Clear();

        for (int i = 0; i < _rSpawnPointList.Count; i++) {
            Transform rTrans = _rSpawnPointList[i];

            var nav_data = rTrans.GetComponent<NavigationRouteData>();
            if (nav_data)
                m_navigation_data_list.Add(nav_data);
            else
                UserLog.ErrorTerauchi(transform.GetChild(i).name + "not attach NavigationRouteData");
        }
    }

    IEnumerator Execute(int level, int num_spawn, float delay_second)
    {
        yield return new WaitForSeconds(delay_second);

        int respawn_count = 0;

        //test
        if (OneSpawnMove)
            num_spawn = 1;
        while (respawn_count < num_spawn)
        {
            for (int i = 0; i < m_num_spawn_one_frame; i++)
            {
                EnemyData create_enemy_data = m_generate_director.DirectionGenerateEnemy(m_current_generate_list);
                GameObject new_enemy = CreateEnemyInstance(Random.Range(1, level + 1), create_enemy_data);

                if (new_enemy) {
                    NetworkIdentity rIndentity = new_enemy.GetComponent<NetworkIdentity>();
                    if (rIndentity) NetworkServer.Spawn(new_enemy);
                }

                respawn_count++;

                if (respawn_count >= num_spawn)
                    break;
            }
            yield return new WaitForSeconds(m_sparn_interval);
        }
        m_is_running = false;
    }

    /**
    *@brief 敵Instanceを生成する
    *@param 現在のウェーブレベル
    *@param 敵の種類
    *@param 出現する位置Index(子であるSpawnPoint昇順)
    *@param 敵の通ることのできるrouteIndex 
    */
    private GameObject CreateEnemyInstance(
        int level,
        EnemyData data)
    {
        //  出現地点がない場合は発生処理を行わない
        if (m_navigation_data_list.Count == 0) return null;

        //このエラーチェックは最終的に消す
        if (data.type >= GenerateEnemyList.Length)
            UserLog.ErrorTerauchi("Enemygenerator::CreateEnemyInstance type_index error !! max_enemy_type is " + GenerateEnemyList.Length + "type_index is" + data.type);
        if (data.respawn_point_index >= m_navigation_data_list.Count)
            UserLog.ErrorTerauchi("Enemygenerator::CreateEnemyInstance respawan_index error !! max_respawn_point_index is " + GenerateEnemyList.Length + "type_index is" + data.respawn_point_index);

        GameObject ret_object = Instantiate(m_current_generate_list[data.type]);

        //NavMeshのWarpは遅延があるので最初のフレームは遠くに飛ばしておいて隠す
        ret_object.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);

        Vector3 respawn_point = m_navigation_data_list[data.respawn_point_index].transform.position;
        //ret_object.GetComponent<NavMeshAgent>().Warp(respawn_point.position);
        // ret_object.transform.position = respawn_point.position;
        //座標はちょっとだけばらつき入れておく
        respawn_point += new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), .0f, UnityEngine.Random.Range(-0.5f, 0.5f));


        var route_list = m_navigation_data_list[data.respawn_point_index].GetRouteData(data.route_index);
        //サイズが0の場合の時は出現位置から最短経路をたどる敵になる
        if (route_list.data.Count == CanAllRoute)
        {
            route_list = GetComponent<CostNameContainer>().GetLayerNameArray();
        }

        var initializer = ret_object.GetComponent<EnemyInitializerBase>();
        initializer.Execute(respawn_point, m_parametor);
        //ret_object.transform.parent = this.transform;
        return ret_object;
    }

    /**
    *@note そのうちちゃんとする
    */
    private void InitializeCrowdEnemy(GameObject root_enemy_object, StringList route_list, int level)
    {

        for (int i = 0; i < root_enemy_object.transform.childCount; i++)
        {
            var game_object = root_enemy_object.transform.GetChild(i);
            //経路設定
            var controller = game_object.GetComponent<EnemyController>();
            if (route_list.data.Count == CanAllRoute)
            {
                route_list = GetComponent<CostNameContainer>().GetLayerNameArray();
            }
            controller.SetRouteData(route_list);

            //体力設定
            //var hp = game_object.GetComponent<Health>();
        }
    }

    private void CreateCurrentGenerateEnemyList(EnemyWaveParametor param)
    {
        for (int i = 0; i < m_enemy_unlock_wave_list.Count; i++)
        {
            if(param.m_current_level == m_enemy_unlock_wave_list[i])
            {
                UnLockEnemy(i);
            }
        }
    }

    private void UnLockEnemy(int level)
    {
        for(int  count =0;   count<m_enemy_unlock_wave_list.Count; count++)
        {
            if(m_enemy_unlock_wave_list[count] == level)
            {
                foreach (var enemy in m_unlock_enemy_block_list[count].m_list)
                {
                    m_current_generate_list.Add(enemy);
                }
            }
        }
    }
}

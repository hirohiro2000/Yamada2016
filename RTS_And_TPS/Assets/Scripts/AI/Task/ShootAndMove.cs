using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShootAndMove : TaskBase {

    public enum ShootType
    {
         FullAuto,
         Burst 
    }

    [SerializeField,HeaderAttribute("発射するオブジェクト")]
    private GameObject BulletObject = null;

    [SerializeField, HeaderAttribute("弾を発射する間隔(秒)")]
    private float ShotIntarvalSecond = 1.0f;

    [SerializeField, HeaderAttribute("弾の速度")]
    private float ShotPower = 2.0f;

    [SerializeField, HeaderAttribute("弾を撃つ範囲(PlayerのVisibilityRangeより少し近いくらいがおすすめ)")]
    private float ShotRange = 7.0f;

    [SerializeField, HeaderAttribute("trueにするとPQSを使った位置取りを行う(基本触るな)")]
    private bool m_use_pqs = false;

    [SerializeField, HeaderAttribute("Defaultの弾バースト数")]
    private int NumDefaultBurstShot = 3;
    private int m_num_burstshot;
    private GameObject m_home_base = null;
    private Transform m_shoot_object = null;
    private Vector3 m_shoot_point = new Vector3();
    private Vector3 m_move_point = Vector3.zero;       //PQSを使う場合move_point = target.position + m_move_point 
    private NavMeshAgent m_navmesh_accessor = null;
    private bool m_is_active = false;
    private float m_navmesh_agent_stop_dist = 5.0f;

    private GameObject m_attack_object_root = null;

    private float m_shot_pos_update_intarval = 1.0f;
    private float m_agent_speed = .0f;

    private PQSQuery m_pqs_info = null;

    delegate void TargetingPointFunction(TargetingSystem target_system);
    private TargetingPointFunction m_targeting_function;

    void Awake()
    {
      //  m_navmesh_accessor = transform.root.GetComponent<NavMeshAgent>();
        m_shoot_object = transform.FindChild("ShootObject");
        m_num_burstshot = NumDefaultBurstShot;
        
    }

    private void UpdateTargetingPointDefault(TargetingSystem targeting_system)
    {
        //targeting_systemでもcheckしているがなぜかエラーでるのでとりあえず
        if (targeting_system.IsTargetArive())
        {
            m_move_point = targeting_system.m_current_target.transform.position;
            m_navmesh_accessor.SetDestination(m_move_point);
        }
    }

    private void CalculateNewMovePoint(TargetingSystem target_system)
    {
        PointQuerySystem.ResultData result;
        //PQSに失敗した場合は動くMovePointはtargetのpositionにしてreturn
        if (!target_system.m_pqs.CalculateNewPoint(m_pqs_info,
            target_system.m_current_target.transform,
            m_owner_object.transform,
            0.3f,
            0.3f,
           out result))
        {
            m_move_point = Vector3.zero;
            m_navmesh_accessor.SetDestination(target_system.m_current_target.transform.position);
            m_navmesh_accessor.Resume();
            return;
        }
        //成功した場合はoffsetのベクトルを保存してその位置に移動する
        m_move_point = result.target_pos_offset;
        m_navmesh_accessor.SetDestination(target_system.m_current_target.transform.position + m_move_point);
        m_navmesh_accessor.Resume();
        return;
    }

    private void UpdateTargetingPointUsePQS(TargetingSystem target_system)
    {
        //現在のMovePointOffsetがほぼの場合Pointを打ち直す
       if(m_move_point.sqrMagnitude <= 0.0001f)
        {
            CalculateNewMovePoint(target_system);
            return;
        }

       //現在のターゲット位置からのOffsetで移動可能かを評価する
       //だめな場合は新たにPointを打ち直す
       if(!target_system.m_pqs.IsValidCurrentPoint(target_system.m_current_target.transform,
           m_move_point,
           m_pqs_info,
           0.3f))
        {
            CalculateNewMovePoint(target_system);
            return;
        }
        //m_move_point = Vector3.zero;
        m_navmesh_accessor.SetDestination(target_system.m_current_target.transform.position + m_move_point);
    }

    private void InitializeMoveAlg()
    {
        if (m_use_pqs)
        {
            m_pqs_info = GetComponent<PQSQuery>();
            if (m_pqs_info == null)
                UserLog.ErrorTerauchi(m_owner_object.name + "no attach PQSquecy!!");
            m_targeting_function = UpdateTargetingPointUsePQS;
           // m_targeting_function = UpdateTargetingPointDefault;

        }
        else
        {
            m_targeting_function = UpdateTargetingPointDefault;
        }
    }

    void Start()
    {

        var enemy_root = GameObject.Find("EnemySpawnRoot");
        m_attack_object_root = enemy_root.GetComponent<ReferenceWrapper>().m_attack_object_root;
       
    }

    void Update()
    {

    }

    IEnumerator UpdateLookPointAndMovePoint(TargetingSystem target_system)
    {
        m_navmesh_accessor.Resume();
        while(m_is_active)
        {
            //targeting_systemでもcheckしているがなぜかエラーでるのでとりあえず
            if(target_system.IsTargetArive())
                m_shoot_point = target_system.m_current_target.transform.position;

            //   m_owner_object.transform.LookAt(m_shoot_point);
            //射撃位置更新
            m_shoot_object.LookAt(m_shoot_point);
            
            //移動位置更新
            m_targeting_function(target_system);
           
            yield return new WaitForSeconds(m_shot_pos_update_intarval);
        }
    }

    IEnumerator Attack(TargetingSystem target_system)
    {
        while(m_is_active)
        {
            StartCoroutine(BurstShoot(target_system));
            yield return new WaitForSeconds(ShotIntarvalSecond);
        }
    }

    public float GetShotRange()
    {
        return ShotRange;
    }

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        InitializeMoveAlg();
        var enemy_root = GameObject.Find("EnemySpawnRoot");
        m_home_base = enemy_root.GetComponent<ReferenceWrapper>().m_home_base;
        m_navmesh_accessor = owner.GetComponent<NavMeshAgent>();
        m_agent_speed = m_navmesh_accessor.speed;
    }

    public override void SetWaveParametor(EnemyWaveParametor param)
    {
        float temp= param.m_current_level * param.GetBurstIncrementRate();
        m_num_burstshot = NumDefaultBurstShot + (int)temp; 
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        m_is_active = true;
        StartCoroutine(UpdateLookPointAndMovePoint(target_system));
        StartCoroutine(Attack(target_system));


        //ほかのキャラクターのルートをたどる可能性があるから改良する
     //   m_navmesh_accessor.SetDestination(m_home_base.transform.position);
    }

    public override Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        //UpdateIsReachHomeBase(target_system);
        Status current_status = EvaluteStatus(target_system, task_director);
        return current_status;
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        m_is_active = false;
        m_navmesh_accessor.Resume();
        m_navmesh_accessor.speed = m_agent_speed;
    }

    private Status EvaluteStatus(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        if (target_system.m_current_target == null)
            return Status.Completed;

        float dist = (m_owner_object.transform.position - target_system.m_current_target.transform.position).magnitude;
        if (dist >= ShotRange)
            return Status.Failed;

        return Status.Active;
    }

    //private void UpdateIsReachHomeBase(TargetingSystem m_current_target)
    //{
    //    //現在ホームベースがないからターゲットにする
    //    //float dist = (m_owner_object.transform.position - m_home_base.transform.position).magnitude;
    //    float dist = (m_owner_object.transform.position - m_current_target.transform.position).magnitude;
    //    if (dist < m_navmesh_agent_stop_dist)
    //    {
    //        m_navmesh_accessor.Stop();
    //        m_navmesh_accessor.updateRotation = true;
    //        Quaternion look_rotation = Quaternion.LookRotation((m_shoot_point - m_owner_object.transform.position).normalized);
    //        m_owner_object.transform.rotation = Quaternion.Slerp(m_owner_object.transform.rotation, look_rotation, 0.1f);
    //    }
    //    else
    //    {
    //       // m_navmesh_accessor.speed = m_agent_speed;
    //        m_navmesh_accessor.Resume();
    //    }
    //}

    IEnumerator BurstShoot(TargetingSystem target_system)
    {

        for (int i = 0; i < m_num_burstshot; i++)
        {
            GameObject shot_object = Instantiate(BulletObject);
            shot_object.transform.position = m_shoot_object.transform.position;
            Vector3 target_position = target_system.m_current_target.transform.position;
            //とりあえずちょっと上にあげた後に散らばらせる
            //ここの散らばらせ方はそのうち帰るかも
            target_position += new Vector3(
                UnityEngine.Random.Range(-0.3f, 0.3f),
                 0.7f + UnityEngine.Random.Range(-0.3f, 0.3f),
                 UnityEngine.Random.Range(-0.3f, 0.3f));
         
            Vector3 vec = (target_position - shot_object.transform.position).normalized * ShotPower;
            var rigid_body = shot_object.GetComponent<Rigidbody>();
            shot_object.transform.parent = m_attack_object_root.transform;
            if (rigid_body)
            {
                rigid_body.AddForce(vec);
            }
            else
            {
                UserLog.ErrorTerauchi(m_owner_object.name + "ShootAndMove Bullet No attach RigidBody!!");
            }

            //Debug.Log("Fire!!");
            yield return new WaitForSeconds((1.0f / 60.0f) * 3.0f);
        }

    }

}

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

    private GameObject m_home_base = null;
    private Transform m_shoot_object = null;
    private Vector3 m_shoot_point = new Vector3();
    private NavMeshAgent m_navmesh_accessor = null;
    private bool m_is_active = false;
    private float m_navmesh_agent_stop_dist = 5.0f;

    private GameObject m_attack_object_root = null;

    private float m_shot_pos_update_intarval = 1.0f; 

    void Awake()
    {
      //  m_navmesh_accessor = transform.root.GetComponent<NavMeshAgent>();
        m_shoot_object = transform.FindChild("ShootObject");
       
    }

    void Start()
    {
        m_attack_object_root = transform.root.GetComponent<ReferenceWrapper>().m_attack_object_root;
    }

    IEnumerator UpdateLookPoint(TargetingSystem target_system)
    {
        while(m_is_active)
        {
            m_shoot_point = target_system.m_current_target.transform.position;
            m_shoot_object.LookAt(m_shoot_point);
            yield return new WaitForSeconds(m_shot_pos_update_intarval);
        }
    }

    IEnumerator Attack(TargetingSystem target_system)
    {
        while(m_is_active)
        {
            StartCoroutine(BurstShoot(target_system));
            yield return new WaitForSeconds(ShotIntarvalSecond);
            //GameObject shot_object = Instantiate(BulletObject);
            //shot_object.transform.position = m_shoot_object.transform.position;
            //Vector3 target_position = target_system.m_current_target.transform.position;
            ////とりあえずちょっと上にあげる
            //target_position += new Vector3(.0f, 0.7f, .0f);
            //Vector3 vec = (target_position - shot_object.transform.position).normalized * ShotPower;
            //var rigid_body = shot_object.GetComponent<Rigidbody>();
            //shot_object.transform.parent = m_attack_object_root.transform;
            //if(rigid_body)
            //{
            //    rigid_body.AddForce(vec);
            //}
            //else
            //{
            //    UserLog.ErrorTerauchi(m_owner_object.name + "ShootAndMove Bullet No attach RigidBody!!");
            //}
            //NetworkServer.Spawn(shot_object);
        }
    }

    public float GetShotRange()
    {
        return ShotRange;
    }

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        m_home_base = transform.root.GetComponent<ReferenceWrapper>().m_home_base;
        m_navmesh_accessor = owner.GetComponent<NavMeshAgent>();
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        m_is_active = true;
        StartCoroutine(UpdateLookPoint(target_system));
        StartCoroutine(Attack(target_system));
        //ほかのキャラクターのルートをたどる可能性があるから改良する
        m_navmesh_accessor.SetDestination(m_home_base.transform.position);
    }

    public override Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        UpdateIsReachHomeBase();
        Status current_status = EvaluteStatus(target_system, task_director);
        return current_status;
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        m_is_active = false;
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

    private void UpdateIsReachHomeBase()
    {
        float dist = (m_owner_object.transform.position - m_home_base.transform.position).magnitude;
        if(dist < m_navmesh_agent_stop_dist)
        {
            m_navmesh_accessor.Stop();
        }
    }

    IEnumerator BurstShoot(TargetingSystem target_system)
    {

        for (int i = 0; i < 3; i++)
        {
            GameObject shot_object = Instantiate(BulletObject);
            shot_object.transform.position = m_shoot_object.transform.position;
            Vector3 target_position = target_system.m_current_target.transform.position;
            //とりあえずちょっと上にあげる
            target_position += new Vector3(.0f, 0.7f, .0f);
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
            yield return new WaitForSeconds((1.0f / 60.0f) * 3.0f);
        }

    }

}

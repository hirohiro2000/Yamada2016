using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {


    [SerializeField, HeaderAttribute("そのうちインスペクタから隠す")]
    private float m_update_intarval_socond = 0.5f;

    private EnemyTaskDirector  m_task_director;
    private VisibilitySystem       m_visibility_censor;
    private TargetingSystem     m_target_director;
    private bool                        m_coroutine_flg;
    

    void Awake()
    {
        m_target_director = GetComponent<TargetingSystem>();
        m_task_director = GetComponent<EnemyTaskDirector>();
        m_visibility_censor = GetComponent<VisibilitySystem>();

        //処理を分散させるために少しだけ更新間隔をずらす（ちゃんとできてるかは謎）
        float one_frame = 1.0f / 60.0f;
        m_update_intarval_socond += one_frame * UnityEngine.Random.Range(.0f, 4.0f);
        m_coroutine_flg = true;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(UpdateFramework());
	}

    IEnumerator UpdateFramework()
    {
        while(m_coroutine_flg)
        {
            //初期化時にwaitをかけるため
            yield return new WaitForSeconds(m_update_intarval_socond);
         
            UpdateCensor();
            //ターゲットが変更されたらタスク変更の可能性があるので更新をかける
            if (m_target_director.EvalutionTargetCandidate(m_visibility_censor))
            {
                m_task_director.PlanningTask(m_target_director);
            }
        }

    }

    private void UpdateCensor()
    {
        m_visibility_censor.ClearCurrentVisibilityList();
        m_visibility_censor.VisibilityCheck();
    }

    void OnDestroy()
    {
        m_coroutine_flg = false;
    }

    // Update is called once per frame
    void Update ()
    {
         var status =  m_task_director.UpdateTask(m_target_director);
        if(status != TaskBase.Status.Active)
        {
            m_target_director.TargetClear();
            UpdateCensor();
            m_target_director.EvalutionTargetCandidate(m_visibility_censor);
            m_task_director.PlanningTask(m_target_director);
        }
	}
}

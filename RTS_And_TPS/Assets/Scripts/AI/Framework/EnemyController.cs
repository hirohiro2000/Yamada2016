
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyController : NetworkBehaviour {


    [SerializeField, HeaderAttribute("そのうちインスペクタから隠す")]
    private float m_update_intarval_socond = 0.5f;

    private EnemyTaskDirector  m_task_director = null;
    private VisibilitySystem       m_visibility_censor = null;
    private TargetingSystem     m_target_director = null;
    private bool                      m_coroutine_flg ;
    public delegate void          Deadlistener(GameObject dead_enemy);
    private Deadlistener           m_dead_callback;   //死んだときにEnemyGeneratorに通知するためのコールバック
    private EnemyPersonalParametor m_personal_param = null; 


    /**
    *@note　現在未使用
    */
    public void SetRouteData(StringList route_data)
    {
        var move_target = GetComponentInChildren<MovingTarget>();
        if (!move_target)
            return;
        move_target.SetPassingRoute(route_data);
    }

    public void SetDeadListener(Deadlistener callback_function)
    {
        m_dead_callback = callback_function;
    }

    void Awake()
    {
        //個体別のparameter取得
        m_personal_param = GetComponent<EnemyPersonalParametor>();
        m_target_director = GetComponent<TargetingSystem>();
        m_task_director = GetComponent<EnemyTaskDirector>();
        m_visibility_censor = GetComponent<VisibilitySystem>();

        //処理を分散させるために少しだけ更新間隔をずらす（ちゃんとできてるかは謎）
        float one_frame = 1.0f / 60.0f;
        m_update_intarval_socond += one_frame * UnityEngine.Random.Range(.0f, 4.0f);
        m_coroutine_flg = true;

        //  エネミーリストに追加
        EnemyGenerator  rGenerator  =   GameObject.Find( "EnemySpawnRoot" ).GetComponent< EnemyGenerator >();
        rGenerator.GetCurrentHierachyList().Add( gameObject );
    }

	// Use this for initialization
	void Start () {
        //  クライアント側でのみパラメータの初期化処理を行う
        if( !NetworkServer.active ){
            EnemyWaveParametor  rWaveParam  =   FunctionManager.GetAccessComponent< EnemyWaveParametor >( "EnemySpawnRoot" );
            SetWaveParametor_InClient( rWaveParam );
        }

        if (m_personal_param == null)
        {
            UserLog.Terauchi(gameObject.name + "not attach EnemyPersonalParametor!!");
        }

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
            //割り込みtaskを更新する
            m_task_director.EvalutionInterruptTask(m_target_director);
        }

    }

    private void UpdateCensor()
    {
        m_visibility_censor.ClearCurrentVisibilityList();
        m_visibility_censor.VisibilityCheck();
    }

    void    OnDestroy()
    {
        //UIRadar.Remove(gameObject);
        m_coroutine_flg = false;
    }

    // Update is called once per frame
    void Update ()
    {
        //ターゲットが生きているかどうかを確認する
        if(!m_target_director.IsTargetArive())
        {
            m_target_director.TargetClear();
            UpdateCensor();
            m_target_director.EvalutionTargetCandidate(m_visibility_censor);
            m_task_director.PlanningTask(m_target_director);
        }

         var status =  m_task_director.UpdateTask(m_target_director);
        if(status != TaskBase.Status.Active)
        {
            m_target_director.TargetClear();
            UpdateCensor();
            m_target_director.EvalutionTargetCandidate(m_visibility_censor);
            m_task_director.PlanningTask(m_target_director);
        }
	}

    public  void    SetWaveParametor(EnemyWaveParametor wave_param)
    {
        //  体力設定     
        Health rHealth = GetComponent<Health>();
        if (rHealth) rHealth.CorrectionHP(wave_param.m_current_level - m_personal_param.m_emearge_level,
            m_personal_param.GetHPUpMultipleRate());
        else UserLog.ErrorTerauchi(gameObject.name + "no attach Health !!");
        m_task_director.SetWaveparamtor(wave_param, m_personal_param);
    }
    private void    SetWaveParametor_InClient( EnemyWaveParametor _WaveParam )
    {
        //  体力関係のパラメータはHealth内で同期されているので、それ以外のパラメータだけをクライアント側で初期化
        m_task_director.SetWaveparamtor( _WaveParam, m_personal_param );
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/**
*@brief 敵の行動管理を管理しているクラス
*@network メンバ関数はおそらくサーバーだけうごかせばOK
*/
public class EnemyTaskDirector : MonoBehaviour {
   

   
    private TaskBase                m_current_task;
    private List<TaskBase>       m_task_array;    
    GameObject                      m_task_folder;
    public GameObject            m_owner { get; private set; }
    public AnimationController m_anime_controller { get; private set; }

    void Awake()
    {
        m_task_folder = transform.FindChild("TaskHolder").gameObject;
        m_anime_controller = GetComponent<AnimationController>();
        m_owner = gameObject;
        InitializeTaskArray();
    }

    void InitializeTaskArray()
    {
        m_task_array = new List<TaskBase>();
        for(int i = 0; i < m_task_folder.transform.childCount; i++)
        {
            GameObject temp = m_task_folder.transform.GetChild(i).gameObject;
            //temp.transform.parent = m_task_folder.transform;
            var insert_task = temp.GetComponent<TaskBase>();
            insert_task.Initialize(gameObject);
            m_task_array.Add(insert_task);
            
        }
    }

	// なぜかEnemyController::Updateが先に呼ばれている現象が発生
	void Start ()
    {
        //m_task_folder = transform.FindChild("TaskHolder").gameObject;
        //m_anime_controller = GetComponent<AnimationController>();
        //m_owner = gameObject;
        //InitializeTaskArray();
       
    }

    /**
    *@brief 保持している全タスクを評価してタスクを決定する
    *@network おそらくここはサーバーだけ動かしておけばOK
    */
    public void PlanningTask(TargetingSystem target_director)
    {
        //保持している全タスクを評価
        float max_score = .0f;
        TaskBase candidate_task = null;
        
        if(target_director.m_current_target == null)
        {
            UserLog.Terauchi("target is null");
        }

        foreach(var task in m_task_array)
        {
            float score = task.EvalutionScore(target_director, this);
            if(score > max_score)
            {
                max_score = score;
                candidate_task = task;
            }
        }
        if(!candidate_task)
        {
            Debug.Log("PlanningTaskでタスクが見つかりませんでした。");
            return;
        }
        ChangeTask(candidate_task, target_director);
    }

    private void ChangeTask(TaskBase new_task, TargetingSystem target_director)
    {
        if (m_current_task)
        {
            
            //Debug.Log("exit_task is " + m_current_task.name);
            m_current_task.Exit(target_director, this);
        }
        m_current_task = new_task;
        m_current_task.Enter(target_director, this);
        //Debug.Log("new_task is " + m_current_task.name);
    }

    /**
    *@breif      現在のタスクを実行する
    *@network 
    */
    public TaskBase.Status UpdateTask(TargetingSystem target_director)
    {
        TaskBase.Status current_status = TaskBase.Status.Active;
        if (m_current_task)
            current_status = m_current_task.Execute(target_director, this);
        //else
           // Debug.Log(gameObject.name +  " current_task is null");

        return current_status;
    }
        
}

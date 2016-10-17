using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyTaskDirector : MonoBehaviour {
   

    //MovingTarget                   m_move_state;  //とりあえず
    TaskBase                           m_current_task;
    [SerializeField, HeaderAttribute("このキャラクターが使用できるタスクの一覧")]
    private GameObject[]       available_task_array;

    private List<TaskBase>     m_task_array;    
    GameObject                      m_task_folder;
    public GameObject           m_owner { get; private set; }

    void Awake()
    {
        //m_targeting_system = GetComponent<TargetingSystem>();
    }

    void InitializeTaskArray()
    {
        m_task_array = new List<TaskBase>();
        foreach(var task in available_task_array)
        {
            GameObject temp = Instantiate(task);
            temp.transform.parent = m_task_folder.transform;
            var insert_task = task.GetComponent<TaskBase>();
            insert_task.Initialize(gameObject);
            m_task_array.Add(insert_task);
            
        }
    }

	// Use this for initialization
	void Start ()
    {
        //test
        m_task_folder = transform.FindChild("TaskHolder").gameObject;
       // var temp = GameObject.Instantiate((GameObject)Resources.Load("AI\\MoveTargetDefault"));
        // m_move_state = temp.GetComponent<MovingTarget>();
        //temp.transform.parent = m_task_folder.transform;
        m_owner = gameObject;
        InitializeTaskArray();
       
    }

    /**
    *@brief 保持している全タスクを評価してタスクを決定する
    */
    public void PlanningTask(TargetingSystem target_director)
    {
        //保持している全タスクを評価
        float max_score = .0f;
        TaskBase candidate_task = null;
        
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
            Debug.Log("exit_task is " + m_current_task.name);
            m_current_task.Exit(target_director, this);
        }
        m_current_task = new_task;
        m_current_task.Enter(target_director, this);
        //Debug.Log("new_task is " + m_current_task.name);
    }

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

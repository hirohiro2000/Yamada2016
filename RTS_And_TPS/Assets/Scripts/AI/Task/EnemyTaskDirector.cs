using UnityEngine;
using System.Collections;

public class EnemyTaskDirector : MonoBehaviour {

    public enum MessageType
    {
        InVisibilityRange,       //視界判定内に入った
        InNearRange,            //近距離まで来た
        OutVisibilityRange,   //視界判定外から出た
        OutNearRange,         //近距離から外れた
    }

    TargetingSystem m_targeting_system;

    MovingTarget             m_move_state;  //とりあえず
    EnemyParam              m_param;
    TaskEvaluationBase     m_task_evalution_object;
    TaskBase                    m_current_task;

    [SerializeField, HeaderAttribute("攻撃系タスクの配列")]

    private float m_update_interval = 0.5f;

    GameObject m_task_folder;

    void Awake()
    {
        m_targeting_system = GetComponent<TargetingSystem>();
        m_task_evalution_object = GetComponent<TaskEvaluationBase>();
    }

	// Use this for initialization
	void Start ()
    {
        //test
        m_task_folder = transform.FindChild("TaskHolder").gameObject;
        var temp = GameObject.Instantiate((GameObject)Resources.Load("AI\\MoveTarget"));
        m_move_state = temp.GetComponent<MovingTarget>();
        temp.transform.parent = m_task_folder.transform;
        //test
        m_move_state.SetTarget(GameObject.Find("MechanicalGirl"));
        m_move_state.SetNavmesh(GetComponent<NavMeshAgent>());
        m_current_task = m_move_state;
    }
	
	// Update is called once per frame
	void Update () {
        m_current_task.Execute(m_targeting_system,this);
	}

    public void SendMessage(TaskMessageWrapper message)
    {

    }
}

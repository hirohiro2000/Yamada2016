using UnityEngine;
using System.Collections;

public class EnemyTaskDirector : MonoBehaviour {
    


    TargetingSystem         m_targeting_system;
    MovingTarget             m_move_state;  //とりあえず
    TaskBase                    m_current_task;
    [SerializeField, HeaderAttribute("このキャラクターが使用できるタスクの一覧")]
    private TaskBase[]       m_task_array;

    private float m_update_interval = 0.5f;

    GameObject m_task_folder;

    void Awake()
    {
        m_targeting_system = GetComponent<TargetingSystem>();
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
}

using UnityEngine;
using System.Collections;

public class TargetingSystem : MonoBehaviour {

    [SerializeField, HeaderAttribute("攻撃するものの優先順位、サイズはターゲットにしたい対象の数だけにする,ターゲットのTag名を記述")]

    private string[]         m_priority_array;

    public GameObject m_current_target { get; private set; }
    public  float             m_score { get; private set; }


    // Use this for initialization
    void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

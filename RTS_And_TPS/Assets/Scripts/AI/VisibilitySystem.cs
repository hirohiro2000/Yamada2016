using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VisibilitySystem : MonoBehaviour {

    public class ViewCandidateData
    {
        ViewMessageWrapper.MessageType message_type;
        public string sender_tag;
        public GameObject sender_object;
    }

    public class VisibilityData
    {
        public ViewMessageWrapper.MessageType message_type;
        public string sender_tag;
        public GameObject sender_object;
        public float dist;
        
    }
        
    [SerializeField, HeaderAttribute("そのうちインスペクタから隠す")]
    private float m_update_intarval_socond = 0.5f;

    //gameobjectのenableで行けるかもしれんけど
    private bool is_active = true;

    private List<ViewCandidateData> m_candicate_list;
    private List<VisibilityData>          m_current_visibility_list;
        void Awake()
    {
        //処理を分散させるために少しだけ更新間隔をずらす（ちゃんとできてるかは謎）
        float one_frame = 1.0f / 60.0f;
        m_update_intarval_socond += one_frame * UnityEngine.Random.Range(.0f, 4.0f);
        m_candicate_list = new List<ViewCandidateData>();
        m_current_visibility_list = new List<VisibilityData>();
    }

    void OnDestroy()
    {
        is_active = false;
    }
        

    // Use this for initialization
    void Start () {

        StartCoroutine(UpdateVisibilityData());

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator UpdateVisibilityData()
    {
        while(is_active)
        {
            Debug.Log("Update VisibilityData called");
            yield return new WaitForSeconds(m_update_intarval_socond);
        }
    }


    public void InVisibilityRange(ViewMessageWrapper message)
    {
        
    }

    public void OutVisibilityRange(GameObject out_sight_object)
    {

    }
}

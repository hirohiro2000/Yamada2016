using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

public class VisibilitySystem : MonoBehaviour {

    public class ViewCandidateData
    {
        public ViewMessageWrapper.MessageType message_type;
        public string sender_tag;
        public GameObject sender_object;
        public VisibilityChecker visibility_checker;
    }

    public class VisibilityData
    {
        public ViewMessageWrapper.MessageType message_type;
        public string sender_tag;
        public GameObject sender_object;
        public float dist;

        public VisibilityData(ViewCandidateData init_data)
        {
            this.message_type = init_data.message_type;
            this.sender_object = init_data.sender_object;
            this.sender_tag = init_data.sender_tag;

        }
        
    }

    //[SerializeField, HeaderAttribute("目の位置")]
    private Transform m_eye_position;

    [SerializeField, HeaderAttribute("無条件に視認させるオブジェクト(防衛拠点など)必ず1つ以上設定する")]
    private string[] m_always_perception_tag = new string[1] { "DefenseBase" };

    [SerializeField, Range(.0f, 90.0f),HeaderAttribute("視野角")]
    private float m_fav = 45.0f;

    private List<ViewCandidateData> m_candicate_list;
    public List<VisibilityData>             m_current_visibility_list { get; private set; }

    

    //debug
    Text visibility_range_text;
    Text near_range_text;
    Text can_visibility_text;
    private string visibility_text;
    private int can_visibility = 0;

    void Awake()
    {
        m_eye_position = gameObject.transform.FindChild("Eye");
        if (m_eye_position == null)
            Debug.Log(gameObject.name + "Eye transform not found!!");


        m_candicate_list = new List<ViewCandidateData>();
        m_current_visibility_list = new List<VisibilityData>();
    }

    void OnDestroy()
    {
        //is_active = false;
    }

    // Use this for initialization
    void Start () {

     //   StartCoroutine(UpdateVisibilityData());

        //tst
        near_range_text = GameObject.Find("InNearText").GetComponent<Text>();
        visibility_range_text = GameObject.Find("InVisibilityRange").GetComponent<Text>();
        can_visibility_text = GameObject.Find("CanVisibility").GetComponent<Text>();

    }
	
    void DebugLog()
    {
        int visibility_count = 0;
        int near_count = 0;
        foreach (var temp in m_candicate_list)
        {
            switch (temp.message_type)
            {
                case ViewMessageWrapper.MessageType.InNearRange:
                    near_count++;
                    break;

                case ViewMessageWrapper.MessageType.InVisibilityRange:
                    visibility_count++;
                    break;

                default:
                    Debug.Log("error");
                    break;
            }
        }

        near_range_text.text = "near_range is" + near_count;
        visibility_range_text.text = "visibilityrange is" + visibility_count;
        can_visibility_text.text = "can visibility is " + can_visibility.ToString() + " : " +  visibility_text;
    }

    // Update is called once per frame
    void Update () {
        DebugLog();
    }

    private bool IsUnconditionalObject(ViewCandidateData check_object)
    {
        var is_unconditional_tag = Array.IndexOf(m_always_perception_tag, check_object.sender_tag);
        if (is_unconditional_tag != -1)
            return true;

        //とりあえずNearRangeのオブジェクトは無条件で視認させる
        if (check_object.message_type == ViewMessageWrapper.MessageType.InNearRange)
            return true;

        return false;
    }
        

    private void InsertVisibility(ViewCandidateData insert_data)
    {
        var insert = new VisibilityData(insert_data);
        m_current_visibility_list.Add(insert);
    }
      
       
    public void VisibilityCheck()
    {
        for(int object_i = 0; object_i < m_candicate_list.Count; object_i++)
        {
            //まずobjectが消えてないかチェックしておく
            var candicate = m_candicate_list[object_i];
            if(IsUnconditionalObject(candicate))
            {
                InsertVisibility(candicate);
                continue;
            }

            //視野角の判定を行う
            var eye = m_eye_position.position;
            var front = transform.forward;
            if (!candicate.visibility_checker.IsInFov(ref eye,ref front, m_fav))
            {
                continue;
            }

            //rayを飛ばして最終判定を行う
            if (!candicate.visibility_checker.IsInsight(ref eye))
            {
                continue;
            }
            InsertVisibility(candicate);

        }//object_i

        visibility_text = "";
        foreach(var temp in m_current_visibility_list)
        {
            visibility_text += temp.sender_object.name;
            visibility_text += " + ";
        }
        can_visibility = m_current_visibility_list.Count;
    }

    public void PerceptionMessage(ViewMessageWrapper message)
    {
        //そのうち良い方法考える
        switch(message.message_type)
        {
            case ViewMessageWrapper.MessageType.InVisibilityRange:
                InVisibility(message);
                break;

            case ViewMessageWrapper.MessageType.InNearRange:
                InNearRange(message);
                break;

            case ViewMessageWrapper.MessageType.OutNearRange:
                OutNearRange(message);
                break;

            case ViewMessageWrapper.MessageType.OutVisibilityRange:
                OutVisibility(message);
                break;

            default :
                Debug.Log("VisibilitySystem::PerceptionMessageに想定外のメッセージ :" + message.message_type);
                break;
        }    

    }

    private void InVisibility(ViewMessageWrapper msg)
    {
        ViewCandidateData insert = new ViewCandidateData();
        insert.message_type = ViewMessageWrapper.MessageType.InVisibilityRange;
        insert.sender_object = msg.sender_object;
        insert.sender_tag = msg.sender_tag;
        insert.visibility_checker = msg.visibility_checker;
        m_candicate_list.Add(insert);
    }

    private void OutVisibility(ViewMessageWrapper msg)
    {
        m_candicate_list.RemoveAll((x) => x.sender_object == msg.sender_object);
    }

    private void InNearRange(ViewMessageWrapper msg)
    {
        //VisibilityRangeとして視認しているオブジェクトを抽出
        var msg_object = m_candicate_list.Find((x) => x.sender_object == msg.sender_object && x.message_type == ViewMessageWrapper.MessageType.InVisibilityRange);
        if (msg_object == null)
            Debug.Log("InNearRangeでオブジェクトが見つかりません");

        msg_object.message_type = ViewMessageWrapper.MessageType.InNearRange;

    }

    private void OutNearRange(ViewMessageWrapper msg)
    {
        //VisibilityRangeとして視認しているオブジェクトを抽出
        var msg_object = m_candicate_list.Find((x) => x.sender_object == msg.sender_object && x.message_type == ViewMessageWrapper.MessageType.InNearRange);
        if (msg_object == null)
            Debug.Log("OutNearRange");

        msg_object.message_type = ViewMessageWrapper.MessageType.InVisibilityRange;
    }

    public void ClearCurrentVisibilityList()
    {
        m_current_visibility_list.Clear();
    }

}

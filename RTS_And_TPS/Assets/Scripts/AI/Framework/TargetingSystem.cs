using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TargetingSystem : MonoBehaviour {

    public GameObject                                          m_current_target { get; private set; }
    public  float                                                     m_score { get; private set; }
    public ViewMessageWrapper.MessageType   m_message_type { get; private set; }
    private TargetingParam                                   m_param;
    private readonly float                                      m_base_score = 100.0f;

    private VisibilitySystem m_visibility;

    void Awake()
    {
        m_param = GetComponent<TargetingParam>();
        m_visibility = GetComponent<VisibilitySystem>();
    }

    // Use this for initialization
    void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /**
    *@breif　 現在のターゲット情報と候補のターゲット情報を比較する
    * @return ターゲットを変更する必要があるならtrue、なければfalse
    */
    private bool ComparisonCurrentTarget(
        VisibilitySystem.VisibilityData candidata_target,
        float score)
    {
        if (!m_current_target)
            return true;

        //現在のターゲットとMessageRange情報も込みで同じなら変更はしない
        if (m_current_target == candidata_target.sender_object &&
            m_message_type == candidata_target.message_type)
            return false;

        //そのほかはスコアを比較して決める
        if (score > m_score)
            return true;

        return false;
    }
        
    void ChangeTarget(
        GameObject next_target,
        ViewMessageWrapper.MessageType message_type,
        float score)
    {
        if (m_current_target)
        {
            var temp = m_current_target.GetComponent<RendererSwitch>();
            temp.Disable();
        }
        m_current_target = next_target;
        m_message_type = message_type;
        m_score = score;

        var render_switch = m_current_target.GetComponent<RendererSwitch>();
        render_switch.Activate();
    }
        


    /**
    *@breif    視界情報からターゲット情報を評価する
    * @return ターゲットが変更されたらtrue、そのままならfalse 
    */
    public bool EvalutionTargetCandidate(VisibilitySystem visiblity)
    {
        var visibility_list = visiblity.m_current_visibility_list;
        VisibilitySystem.VisibilityData most_candidate = null;
        float max_score = .0f;
        
        foreach (var view_object  in visibility_list)
        {
            float bias = m_param.GetPriorityParam(view_object.sender_tag);

            //とりあえず一番近いターゲットを狙うようにする(のちにここの部分をコンポーネント化する可能性あり)
            Vector3 self_to_target = (view_object.sender_object.transform.position - transform.position);
            float dist_sq = self_to_target.magnitude;
            //0除算対策
            if (dist_sq < .0f)
                dist_sq += 0.1f;

            float score = m_base_score * bias / dist_sq;
            if(score >= max_score)
            {
                max_score = score;
                most_candidate = view_object;
            }
        }

        if(ComparisonCurrentTarget(most_candidate,max_score))
        {
            ChangeTarget(most_candidate.sender_object, most_candidate.message_type, max_score);
            return true;
        }

        return false; 
    }

    public void TargetClear()
    {
        m_current_target = null;
        m_message_type = ViewMessageWrapper.MessageType.Error;
        m_score = .0f;
    }

    /**
    *@brief ターゲットが視認されているかを確認する
    */
    public bool IsInsightTarget()
    {
        var result = m_visibility.m_current_visibility_list.Find(x => x.sender_object == m_current_target);
        if (result != null)
            return true;
        return false;
    }
}

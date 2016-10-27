using UnityEngine;
using System.Collections;

public class PerceptionTrigger : MonoBehaviour {


    [SerializeField]
    private ViewMessageWrapper.MessageType m_trigger_enter_message_type = ViewMessageWrapper.MessageType.Error;

    private ViewMessageWrapper.MessageType m_trigger_out_message_type = ViewMessageWrapper.MessageType.Error;

    private VisibilityChecker m_owner;

    void Awake()
    {
        m_owner = transform.parent.GetComponent<VisibilityChecker>();
        if(m_trigger_enter_message_type == ViewMessageWrapper.MessageType.Error)
        {
            Debug.Log("PerceptionTrigger message_type is error !!");
        }

        m_trigger_out_message_type =
            (m_trigger_enter_message_type == ViewMessageWrapper.MessageType.InVisibilityRange) ?
            ViewMessageWrapper.MessageType.OutVisibilityRange : ViewMessageWrapper.MessageType.OutNearRange;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    private void PerceptionMessage(Collider other)
    {
    
        var visibility_system = other.gameObject.GetComponent<VisibilitySystem>();
        if (!visibility_system)
            return;

        ViewMessageWrapper send_data = new ViewMessageWrapper();
        send_data.sender_object = m_owner.m_owner_object;
        send_data.sender_tag = m_owner.m_owner_object.tag;
        send_data.message_type = m_trigger_enter_message_type;
        send_data.visibility_checker = m_owner;
        visibility_system.PerceptionMessage(send_data);
    }

    private void PerceptionOutMessage(Collider other)
    {
        var visibility_system = other.gameObject.GetComponent<VisibilitySystem>();
        if (!visibility_system)
            return;

        ViewMessageWrapper send_data = new ViewMessageWrapper();
        send_data.sender_object = m_owner.m_owner_object;
        send_data.sender_tag = m_owner.m_owner_object.tag;
        send_data.message_type = m_trigger_out_message_type;
        send_data.visibility_checker = m_owner;
        visibility_system.PerceptionMessage(send_data);
    }
       
    public void OnTriggerEnter(Collider other)
    {
        PerceptionMessage(other);
    }

    public void OnTriggerStay(Collider other)
    {
        // PerceptionMessage(other);
    }

    public void OnTriggerExit(Collider other)
    {
        PerceptionOutMessage(other);
    }
}

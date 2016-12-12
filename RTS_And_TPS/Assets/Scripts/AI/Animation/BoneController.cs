using UnityEngine;
using System.Collections;

public class BoneController : MonoBehaviour {


    [SerializeField, HeaderAttribute("制御するボーン")]
    private Transform ControllBone = null;

    private float m_slerp_speed = 1.0f;

    public Vector3 m_target_direction = Vector3.one;
    private Quaternion target_rotation = Quaternion.identity;

    void Awake()
    {
        if (ControllBone == null)
            UserLog.Terauchi(gameObject.name + "ControllBone is null");
    }

	// Use this for initialization
	void Start () {
	   target_rotation = ControllBone.transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        target_rotation =  Quaternion.LookRotation(ControllBone.transform.forward.normalized,
            m_target_direction.normalized);
        
        

        ControllBone.transform.rotation = Quaternion.Slerp(ControllBone.transform.rotation, target_rotation, m_slerp_speed);
	}



}

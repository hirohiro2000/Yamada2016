using UnityEngine;
using System.Collections;

public class BoneController : MonoBehaviour {


    [SerializeField, HeaderAttribute("制御するボーン")]
    private Transform ControllBone = null;

    private float m_slerp_speed = .1f;

    public Vector3 m_target_direction = Vector3.one;
    private Quaternion target_rotation = Quaternion.identity;

    void Awake()
    {
        if (ControllBone == null)
            UserLog.Terauchi(gameObject.name + "ControllBone is null");
    }

	// Use this for initialization
	void Start () {
	   target_rotation = ControllBone.transform.localRotation;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        //target_rotation *=  Quaternion.FromToRotation(ControllBone.InverseTransformDirection(ControllBone.transform.forward.normalized),
        //    ControllBone.InverseTransformDirection(m_target_direction.normalized));

        ControllBone.LookAt(m_target_direction);

        //ControllBone.transform.localRotation = Quaternion.Slerp(ControllBone.transform.localRotation, target_rotation, m_slerp_speed);
	}



}

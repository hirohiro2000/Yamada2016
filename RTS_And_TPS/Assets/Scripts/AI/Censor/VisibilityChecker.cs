using UnityEngine;
using System.Collections;

public class VisibilityChecker : MonoBehaviour
{
    [SerializeField, HeaderAttribute("視界判定を行う際のターゲットとなる場所")]
    Transform[] m_raycast_target = new Transform[1];

 

    [SerializeField, HeaderAttribute("このオブジェクトは動き回らないかどうか（Trueで動かない）")]
    private bool IsStaticObject = false;

    [SerializeField, HeaderAttribute("オブジェクトのタグ情報")]
    private PerceiveTag Tag = PerceiveTag.Error; 
    
    public GameObject m_owner_object { get; private set; }

    void Awake()
    {
        m_owner_object = transform.parent.gameObject;
    }

    public PerceiveTag GetPerceivetag()
    {
        return Tag;
    }

    public bool IsStatic()
    {
        return IsStaticObject;
    }

    public bool IsInsight(ref Vector3 eye_position)
    {
        Ray ray = new Ray();
        RaycastHit result = new RaycastHit();
        for (int target_i = 0; target_i < m_raycast_target.Length; target_i++)
        {
            var target_pos = m_raycast_target[target_i].position;
            var ray_vec = target_pos - eye_position;
            float dist = ray_vec.magnitude;
            ray.direction = ray_vec.normalized;
            ray.origin = eye_position;

            bool is_hit = Physics.Raycast(  ray,  out result, dist);
            if (!is_hit)
                return true;

            if (result.collider.gameObject == m_owner_object)
                return true;

        }
        return false;
    }

    public bool IsInFov(ref Vector3 eye_position, ref Vector3 eye_vec,float fov)
    {
        for (int target_i = 0; target_i < m_raycast_target.Length; target_i++)
        {
            var target_pos = m_raycast_target[target_i].position;
            var eye_to_target = (target_pos - eye_position).normalized;
            float angle = Vector3.Dot(eye_vec, eye_to_target);
            angle = Mathf.Acos(angle);
            angle = angle / Mathf.PI * 180.0f;
            if (angle < fov)
                return true;
        }
        return false;
    }
}

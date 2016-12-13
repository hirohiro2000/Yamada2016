using UnityEngine;
using System.Collections;

public class PFilterInsight : PointFilterBase {

    [SerializeField]
    private float Fov = 40.0f;


    public override bool IsCanCreate(
        Transform target_transform,
        Vector3 candidate_pos, 
        float target_height)
    {
        Vector3 target_to_caindidate_pos = candidate_pos - target_transform.position;
        float angle = Vector3.Dot(target_transform.forward.normalized, target_to_caindidate_pos.normalized);
        angle = Mathf.Acos(angle);
        angle = angle / Mathf.PI * 180.0f;
        if (angle > Fov)
            return false;

        return true;
    }

}

using UnityEngine;
using System.Collections;

public class PointFilterBase : MonoBehaviour
{

    public virtual bool IsCanCreate(
        Transform target_transform,
        Vector3 candidate_pos,
        float target_height)
    {
        return false;
    }

}

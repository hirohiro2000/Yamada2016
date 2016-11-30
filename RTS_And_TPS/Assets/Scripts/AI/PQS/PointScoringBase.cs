using UnityEngine;
using System.Collections;

public class PointScoringBase : MonoBehaviour {


    public virtual float Execute(
        Transform target_transform,
        Transform owner_transform,
        PointQuerySystem.PointData evalute_point)
    {
        return .0f;
    }

}

using UnityEngine;
using System.Collections;

public class PscoreingFarPoint : PointScoringBase {

    public override float Execute(
        Transform target_transform,
        Transform owner_transform, 
        PointQuerySystem.PointData evalute_point)
    {
        float distsq = (target_transform.position - evalute_point.pos).sqrMagnitude;
        return distsq;
    }

}

using UnityEngine;
using System.Collections;

public class PScoringRandom : PointScoringBase {

    public override float Execute(
        Transform target_transform, 
        Transform owner_transform,
        PointQuerySystem.PointData evalute_point)
    {
        return UnityEngine.Random.Range(.0f, 1.0f);
    }

}

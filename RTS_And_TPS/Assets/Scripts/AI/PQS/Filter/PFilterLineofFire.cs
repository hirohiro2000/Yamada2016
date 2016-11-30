using UnityEngine;
using System.Collections;

public class PFilterLineofFire : PointFilterBase
{

    public override bool IsCanCreate(
        Transform target_transform,
        Vector3 candidate_pos,
        float target_height)
    {
        Vector3 start_pos = candidate_pos;
        Vector3 end_pos = target_transform.position;
        start_pos.y += target_height;
        end_pos.y += target_height;

        Vector3 candidate_to_target = end_pos - start_pos;
        float dist = candidate_to_target.magnitude;

        //レイヤー設定
        LayerMask use_layer = LayerMask.GetMask("Field");

        //Point->Target
        if(Physics.Raycast(start_pos,candidate_to_target.normalized,dist,use_layer))
        {
            return false;
        }

        //現在メッシュの中にもNavMeshができているのでそれもはじくためにtargetから逆方向にRayを飛ばす
        if (Physics.Raycast(end_pos, -candidate_to_target.normalized, dist,use_layer))
        {
            return false;
        }

        return true;
    }

}

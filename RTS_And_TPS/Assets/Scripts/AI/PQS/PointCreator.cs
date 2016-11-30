using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
*@brief PQS用の評価Pointを作成するクラス
*/
public class PointCreator
{
    class BasicFilter
    {

        public bool IsCanCreate(PQSQuery query_info,Transform target,Vector3 candidate_pos)
        {
            Vector3 target_to_caindidate_pos = candidate_pos - target.position;
            float distsq = target_to_caindidate_pos.sqrMagnitude;
            if (distsq <= query_info.GetDisntaceExclusionSq())
                return false;

            float angle = Vector3.Dot(target.forward.normalized, target_to_caindidate_pos.normalized);
            angle = Mathf.Acos(angle);
            angle = angle / Mathf.PI * 180.0f;
            if (angle < query_info.GetEularFovExclusion())
                return false;

            return true;
        }

    }//BasicFilter

    BasicFilter m_basic_filter = null;

    public void Initialize()
    {
        m_basic_filter = new BasicFilter();
    }

    /**
    *@brief    PQSPointを作成する
    *@param PQSQueryの情報
    *@param PQSのターゲットとなるObjectのTransform
    *@param 評価者(自分自身)のTransform
    *@param ターゲットの高さ
    *@param 自分自身の高さ
    *@param この関数内で生成させたPQSPointを格納するList(呼び出し時sizeを0にしておくこと)
    */
    public  void Execute(
        PQSQuery query_info,
        Transform target_transform,
        Transform owner_transform,
        float          target_height,
        float          owner_height,
        List<PointQuerySystem.PointData> out_list)
    {
        float dist_size = query_info.GetPointFieldSize() / query_info.GetFieldDetailY();
        float base_radius = (Mathf.PI * 2.0f)  / query_info.GetFieldDetailX();
        Vector3 base_position = new Vector3();
        float dist = .0f;
        NavMeshHit hit_data = new NavMeshHit();
        for (int dist_i = 0; dist_i < query_info.GetFieldDetailY(); dist_i++)
        {
            dist = dist_size * dist_i;
            for(int  rotate_i = 0; rotate_i < query_info.GetFieldDetailX(); rotate_i++)
            {
                base_position.x = Mathf.Sin(base_radius * rotate_i) * dist;
                base_position.y = .0f;
                base_position.z = Mathf.Cos(base_radius * rotate_i) * dist;
                Vector3 candidate_pos = base_position + target_transform.position;

                //対象となる位置に対してのフィルタリングを行う
                if (!IsCanCreate(query_info,target_transform, candidate_pos, target_height))
                    continue;

                //Pointを生成する
                if (!NavMesh.SamplePosition(candidate_pos, out hit_data, .2f, NavMesh.AllAreas))
                {
                    continue;
                }

                 out_list.Add(new PointQuerySystem.PointData(hit_data.position));
               
            }//rotate_i

        }//dist_i

      //  m_use_filter_list.Clear();
    }

    private bool IsCanCreate(
        PQSQuery query_info,
        Transform target,
        Vector3 candidate_pos,float target_height)
    {
        //まずは計算の軽いBaseFilterでフィルタリングを行う
        if (!m_basic_filter.IsCanCreate(query_info,target, candidate_pos))
        {
            return false;
        }
        //Queryのフィルタをパスすれば生成可能とする
        return query_info.FilteringCandidatePoint(target, candidate_pos, target_height);
    }
}

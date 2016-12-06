using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
*@brief PQS用の評価Pointを作成するクラス
*/
public class PointCreator
{
    class Circle
    {
        private float m_radius = .0f;
        private float m_current_dist = .0f;
        private float m_begin_dist = .0f;
        private float m_end_dist = .0f;
        private float m_add_dist = .0f; 

        private float m_begin_radian = .0f;
        private float m_end_radian = .0f;
        private float m_add_radian = .0f;
        private float m_current_radian = .0f;

        private int m_max_create_point = 0;
        private int m_current_create_point = 0;

        public void BeginCreate(float radius,
                                     int dist_detail,
                                     int rotate_detail,
                                     float distance_exclusion,
                                     float fov_exclusion,
                                     int  max_sampling_point)
        {
            m_radius = radius;
            m_current_create_point = 0;
            //生成除外した角度から有効角度角度を計算する
            m_begin_radian = fov_exclusion / 180.0f * Mathf.PI;
            m_end_radian = (Mathf.PI * 2.0f) - m_begin_radian;
            m_add_radian = (Mathf.PI * 2.0f) / rotate_detail;
            m_current_radian = m_begin_radian;

            //初期の距離と加算していく距離を計算する
            m_end_dist = radius;
            m_begin_dist = distance_exclusion;
            m_add_dist = radius / dist_detail;
            //begin_distが0の場合無駄ループが回るので
            if (m_begin_dist <= 0)
                m_current_dist = m_add_dist;
            else
                m_current_dist = m_begin_dist;

            m_max_create_point = max_sampling_point;
        }

        public bool CreatePoint(ref Vector3 out_pos)
        {

            out_pos.x = Mathf.Sin(m_current_radian) * m_current_dist;
            out_pos.y = .0f;
            out_pos.z = Mathf.Cos(m_current_radian) * m_current_dist;  

            m_current_radian += m_add_radian;
            m_current_create_point++;
            if (m_current_create_point > m_max_create_point)
                return false;

            if(m_current_radian >= m_end_radian)
            {
                m_current_dist += m_add_dist;
                m_current_radian = m_begin_radian;
                //0.001はradiusが5の場合current_distが5に来たとき誤差で判定を間違えないため
                if (m_current_dist >= m_radius + 0.001)
                    return false;
            }
            return true;
        }

        public void EndCreate()
        {
            m_end_radian = .0f;
            m_current_dist = .0f;
            m_current_radian = .0f;
        }

    }

    class Square
    {
        
    }

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

    Circle m_circle_sampler = null;
    BasicFilter m_basic_filter = null;

    public void Initialize()
    {
        m_circle_sampler = new Circle();
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
        m_circle_sampler.BeginCreate(query_info.GetPointFieldSize(),
            query_info.GetFieldDetailY(),
            query_info.GetFieldDetailX(),
            query_info.GetDisntaceExclusion(),
            query_info.GetEularFovExclusion(),
            query_info.GetMaxSamplingCount());

        NavMeshHit hit_data = new NavMeshHit();
        Vector3 candidate_pos = new Vector3();
        while (true)
        {

            //生成ポイントの最大数を超えたら || サイズ以上のポイントを生成しだしたら終了する
            if (!m_circle_sampler.CreatePoint(ref candidate_pos))
                break;

            //形状の座標がローカル空間なのでtargetまで持ってくる
            candidate_pos += target_transform.position;

            //対象となる位置に対してのフィルタリングを行う
            if (!IsCanCreate(query_info, target_transform, candidate_pos, target_height))
                continue;

            //Pointを生成する
            if (!NavMesh.SamplePosition(candidate_pos, out hit_data, .2f, NavMesh.AllAreas))
            {
                continue;
            }
            out_list.Add(new PointQuerySystem.PointData(hit_data.position));
        }
        m_circle_sampler.EndCreate();
    }

    private bool IsCanCreate(
        PQSQuery query_info,
        Transform target,
        Vector3 candidate_pos,float target_height)
    {
        //まずは計算の軽いBaseFilterでフィルタリングを行う
        //if (!m_basic_filter.IsCanCreate(query_info,target, candidate_pos))
        //{
        //    return false;
        //}
        //Queryのフィルタをパスすれば生成可能とする
        return query_info.FilteringCandidatePoint(target, candidate_pos, target_height);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PointQuerySystem : MonoBehaviour
{
    public class PointData
    {
        public Vector3 pos;
        public float score;
        public GameObject debug_object;
        public PointData(Vector3 pos)
        {
            this.pos = pos;
            score = .0f;
            debug_object = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debug_object.transform.position = pos;
            debug_object.transform.localScale *= .2f;
        }

        public void ClearScore()
        {
            score = .0f;
        }
        ~PointData()
        {
            //Destroyはデストラクタやコンストラクタで呼ぶことができない（スレッドの問題？）
            //DestroyImmediate(debug_object);
        }

    }

    public class ResultData
    {
       public Vector3 pos;
       public Vector3 target_pos_offset;
       public  float     score;
    };

    private List<PointData> m_point_list = new List<PointData>();
    private PointCreator m_point_creator = null;

    public bool CalculateNewPoint(
        PQSQuery query,
        Transform target_transform,
        Transform owner_transform,
        float          target_height,
        float           owner_height,
        out ResultData out_result_data)
    {
        ClearPointList();
        CreatePointList(query, target_transform, owner_transform, target_height, owner_height);

        var point = ScoringAllPoint(query.GetPointRater(), target_transform, owner_transform);
        out_result_data = new ResultData();
        if(point == null)
        {
            UserLog.Terauchi(gameObject.name + "PQS::CalculateNewPoint can not evalute point!!");
            out_result_data.pos = Vector3.zero;

            return false;
        }

        out_result_data.pos = point.pos;
        out_result_data.score = point.score;
        out_result_data.target_pos_offset = point.pos - target_transform.position;
        //test
        var material = point.debug_object.GetComponent<MeshRenderer>();
        material.material.color = Color.red;
        return true;
    }

    public bool IsValidCurrentPoint(Transform target_transform,
        Vector3 point_offset_vec)
    {

        return false;
    }

    //temp
    GameObject player = null;
    PQSQuery m_query = null;
    void Awake()
    {
        m_point_creator = new PointCreator();
        m_point_creator.Initialize();
        player = GameObject.Find("Capsule");
    }

    void Start()
    {
        m_query = GetComponent<PQSQuery>();
    }

        void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResultData data;
            if(CalculateNewPoint(m_query, transform, player.transform, 0.3f, 0.3f,out data))
            {
               
            }
        }
    }

    void ClearPointList()
    {
        foreach( var it in m_point_list)
        {
            Destroy(it.debug_object);
        }
        m_point_list.Clear();
    }

    /**
    *@brief PointListを作成する
    *@param PQSQueryの情報
    *@param PQSのターゲットとなるObjectのTransform
    *@param 評価者(自分自身)のTransform
    *@param ターゲットの高さ
    *@param 自分自身の高さ
    *@return 一ヶ所でもポイントが作成できたらTrue: 失敗したらfalse
    */
    bool CreatePointList(PQSQuery query,
        Transform target_transform,
        Transform owner_transform,
        float target_height,
        float owner_height)
    {
        m_point_creator.Execute(query, target_transform, owner_transform, target_height, owner_height,m_point_list);
        if (m_point_list.Count <= 0)
            return false;
        return true;
    }

    PointData ScoringAllPoint(
        PointScoringBase rater,
        Transform target_transform,
        Transform owner_tranform)
    {
        PointData ret = null;
        float max_score = .0f;
        foreach(var point in m_point_list)
        {
            float score = rater.Execute(target_transform, owner_tranform,point);
            if(score >= max_score)
            {
                ret = point;
                max_score = score;
            }
        }

        return ret;
    }
}

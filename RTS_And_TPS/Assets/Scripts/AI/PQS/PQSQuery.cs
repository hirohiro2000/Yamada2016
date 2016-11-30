using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PQSQuery : MonoBehaviour {

    public enum Shape
   {
        circle,
        square
   }

    [SerializeField, HeaderAttribute("pointを生成しない距離")]
    private float DistanceExclusion = .0f;
    public float GetDisntaceExclusion() { return DistanceExclusion; }
    public float GetDisntaceExclusionSq() { return DistanceExclusion *  DistanceExclusion; }

    [SerializeField, HeaderAttribute("pointを生成しない視野角(度)")]
    private float EularFovExclusion = .0f;
    public float GetEularFovExclusion() { return EularFovExclusion; }

    [SerializeField, HeaderAttribute("候補Point生成のField状")]
    private Shape shape = Shape.circle;
   
    [SerializeField, HeaderAttribute("生成されるFieldのsize")]
    private float PointFieldSize = 5.0f;
    public float GetPointFieldSize() { return PointFieldSize; }

    [SerializeField, HeaderAttribute("Circleの場合RotateDetail")]
    private int FieldDetailX = 5;
    public int GetFieldDetailX() { return FieldDetailX; }

    [SerializeField, HeaderAttribute("Circleの場合DistanceDetail")]
    private int FieldDetailY = 5;
    public int GetFieldDetailY() { return FieldDetailY; }

    [SerializeField,HeaderAttribute("Pointの生成位置をランダムにするかどうか")]
    private bool IsRamdomPoint = false;
    public bool IsRamdomPointCreate() { return IsRamdomPoint; }

    [SerializeField, HeaderAttribute("Point最大数 : default = FieldDetailX * FieldDetailY")]
    private int MaxSamplingCount = -1;
    public int GetMaxSamplingCount() { return MaxSamplingCount; }

    private PointScoringBase m_point_rater = null;
    public PointScoringBase GetPointRater() { return m_point_rater; }

    private List<PointFilterBase> m_filter_list = new List<PointFilterBase>();
    /**
    *@brief candidate_posに対するフィルタリングを行う
    *@return true 生成可能 : false 生成不可 
    */
    public bool FilteringCandidatePoint(Transform target,
        Vector3 candidate_pos,
        float target_height)
    {
        foreach (var filter in m_filter_list)
        {
            if (!filter.IsCanCreate(target, candidate_pos, target_height))
                return false;
        }
        return true;
    }

    void Awake()
    {
        if(MaxSamplingCount == -1)
            MaxSamplingCount = FieldDetailX * FieldDetailY;
    }

    void Start()
    {
        var temp = GetComponents<PointFilterBase>();
        foreach(var filter in temp)
        {
            m_filter_list.Add(filter);
        }
        m_point_rater = GetComponent<PointScoringBase>();
        if(m_point_rater == null)
        {
            UserLog.Terauchi(gameObject.name + " no attach PQS:: PointScoring");
        }
    }
}

using UnityEngine;
using System.Collections;

//
//  処理内容:
//  子クラスの処理は[Execute]の実行処理
//  [Facillity]クラスをタレットとかのリソースと同じように処理するための初期化
//  ※初期化は[Start]で書くと[GameObject.Find(...)]でエラーが出たから[Update]で書いています
//  
//  [Update]内でゴリ押しの処理をしているのは
//  本当は[Facillity]を起動させるという処理を書きたいんだけど
//  そうすると起動させるクラスに特別な処理を追記する必要がある
//  だから、[m_collisionParam.m_level]をbool値のように使っています
//
[RequireComponent(typeof(ResourceParameter))]
public class Facility : MonoBehaviour
{
    public      float   m_lifespan  = 1.0f;
    public      bool    m_isRepeated = false;

    protected   ResourceInformation m_resourceInfo = null;
    protected   ItemController      m_itemController = null;
    protected   ResourceParameter   m_collisionParam = null;

    private     bool    m_isInit = false;
    private     bool    m_isActive = false;

    private void Update()
    {
        if (m_isInit == false)
        {
            m_resourceInfo = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
            m_itemController = GameObject.Find("MechanicalGirl").GetComponent<ItemController>();
            m_collisionParam = gameObject.GetComponent<ResourceParameter>();

            transform.position = m_resourceInfo.ComputeGridPosition(transform.position);
            m_resourceInfo.SetGridInformation(gameObject, transform.position, true);

            m_isInit = true;
        }

        if (m_isActive == false)
        {
            m_isActive = ( m_collisionParam.m_level != 1 );

            if (m_isActive)
            {
                Execute();
                if (m_isRepeated)
                {
                    m_collisionParam.m_level = 1;
                    m_isActive = false;
                }
                else // 仮
                {
                    GetComponent<ResourceParameter>().m_createCost = int.MaxValue;
                }
            }
        }
                
    }

    public      bool    isActive { get { return m_isActive; } }
    protected virtual void Execute() { }
    
}


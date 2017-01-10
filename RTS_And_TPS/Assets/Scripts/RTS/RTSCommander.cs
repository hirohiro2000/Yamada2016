using UnityEngine;
using System.Collections;
using   System.Collections.Generic;

public interface IRTSSoldierHandler
{
    void OnCommanderClick();
}

public class RTSCommander : MonoBehaviour
{
	ResourceInformation	m_resourceInformation   = null;
    List<GameObject>    m_soldiers              = null;

    void Start()
    {
		m_resourceInformation = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
    }

    void Update()
    {
        if ( RTSCursor.m_curMode != RTSCursor.MODE.eNone )      return; 
        if ( Input.GetMouseButtonDown(1) == false )             return; 

        Vector3 hitPoint;
        if (RaycastFromMouseToField(out hitPoint) == false )    return;

        foreach (var item in m_soldiers)
        {
            if(IsArignedPointEquals( hitPoint, item.transform.position ))
            {
                item.GetComponent<IRTSSoldierHandler>().OnCommanderClick();
            }
        }

    }

    bool RaycastFromMouseToField( out Vector3 hitPoint )
    {
        RaycastHit  hit             = new RaycastHit();
        int         layerMask       = LayerMask.GetMask( "Field" );
        Vector3     mousePosition   = Input.mousePosition;
        Ray         ray             = Camera.main.ScreenPointToRay(mousePosition);
        
        if ( Physics.Raycast(ray, out hit, float.MaxValue, layerMask) )
        {
            hitPoint = hit.point;
            return true;
        }

        hitPoint = Vector3.zero;
        return false;

    }
    bool IsArignedPointEquals( Vector3 a, Vector3 b )
    {
        a = m_resourceInformation.ComputeGridPosition( a );
        a.y = 0.0f;

        b = m_resourceInformation.ComputeGridPosition( b );
        b.y = 0.0f;

        return ( a-b ).sqrMagnitude <= 0.1f;
    }

    public void AddSoldier(GameObject soldier)
    {
        if ( m_soldiers == null )  m_soldiers = new List<GameObject>();

        m_soldiers.Add( soldier );
    }
}

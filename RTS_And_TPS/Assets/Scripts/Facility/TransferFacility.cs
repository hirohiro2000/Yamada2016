using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ResourceParameter))]
public class TransferFacility : Facility
{
	private	List<GameObject>	m_gateList		= null;

    protected override void Execute()
    {
        if ( m_gateList == null )
        {
   		    m_gateList = new List<GameObject>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform       trans = transform.GetChild(i);
                TransferGate    gate  = trans.GetComponent<TransferGate>();
                if (gate != null)
                {                    
                    GetComponent<ResourceParameter>().m_createCost = int.MaxValue;  //　仮
                    m_gateList.Add(trans.gameObject);
                }
            }
        }
       
        Debug.Log("TransferFacility.Execute()");

    }

    public List<GameObject> gateList
    {
        get { return m_gateList; }
    }    
}
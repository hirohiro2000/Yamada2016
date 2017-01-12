using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParameter))]
public class ResourceFacility : Facility
{
    public int   m_addResourceCost = 2;
    public float m_timeSpawn = 1.5f;
    protected override void Execute()
    {
        StartCoroutine(AddResourceCost());
    }

    IEnumerator AddResourceCost()
    {
        ResourceParameter collisionParam = GetComponent<ResourceParameter>();

        while (true)
        {
            m_itemController.AddResourceCost(m_addResourceCost);
            yield return new WaitForSeconds(m_timeSpawn);
        }
    }

}

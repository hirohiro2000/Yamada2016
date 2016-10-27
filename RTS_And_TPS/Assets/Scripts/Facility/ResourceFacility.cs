using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParam))]
[RequireComponent(typeof(CollisionParam))]
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
        CollisionParam collisionParam = GetComponent<CollisionParam>();

        while (true)
        {
            m_itemController.AddResourceCost(m_addResourceCost);
            yield return new WaitForSeconds(m_timeSpawn);
        }
    }

}

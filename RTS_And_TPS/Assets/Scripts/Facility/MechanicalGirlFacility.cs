using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParam))]
[RequireComponent(typeof(CollisionParam))]
public class MechanicalGirlFacility : Facility
{
    protected override void Execute()
    {
        Debug.Log("MechanicalGirlFacility.Execute()");
    }

}

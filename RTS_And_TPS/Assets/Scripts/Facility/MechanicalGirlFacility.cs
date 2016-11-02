using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParameter))]
public class MechanicalGirlFacility : Facility
{
    protected override void Execute()
    {
        Debug.Log("MechanicalGirlFacility.Execute()");
    }

}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParameter))]
public class RobotFacility : Facility
{
    protected override void Execute()
    {
        Debug.Log("RobotFacility.Execute()");
    }
}

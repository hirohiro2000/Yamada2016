using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParam))]
[RequireComponent(typeof(CollisionParam))]
public class RobotFacility : Facility
{
    protected override void Execute()
    {
        Debug.Log("RobotFacility.Execute()");
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParameter))]
public class GroundStation : Facility
{
    protected override void Execute()
    {
        Debug.Log("GroundStation.Execute()");

        Camera.main.GetComponent<RTSCamera>().enabled           = !Camera.main.GetComponent<RTSCamera>().enabled;      
        Camera.main.GetComponent<CameraGroundStation>().enabled = !Camera.main.GetComponent<CameraGroundStation>().enabled;
    }
}
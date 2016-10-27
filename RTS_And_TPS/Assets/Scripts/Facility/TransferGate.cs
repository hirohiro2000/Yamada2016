using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceParam))]
[RequireComponent(typeof(CollisionParam))]
public class TransferGate : Facility
{
    protected override void Execute()
    {
        TransferFacility transferFacility = transform.parent.GetComponent<TransferFacility>();

        if (transferFacility.isActive == true)
        {
            GameObject mg = GameObject.Find("MechanicalGirl");

            Vector3 targetPosition = transferFacility.gateList[0].transform.position;
            mg.transform.position = new Vector3(targetPosition.x, mg.transform.position.y, targetPosition.z);


            Debug.Log("TransferGate.Execute()");
        }
    }
}
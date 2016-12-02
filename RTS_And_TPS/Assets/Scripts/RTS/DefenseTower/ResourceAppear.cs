using UnityEngine;
using System.Collections;

public class ResourceAppear : MonoBehaviour
{
	[SerializeField]
	AnimationCurve height = null; 
    
	public float originHeight = 0.0f;

    [SerializeField]
    float timer = 0.0f;

    [SerializeField]
    float timeSpeed = 1.0f;

    [SerializeField]
    GameObject  originalPlate = null;

    GameObject plate = null;

    void Start()
    {
        timer = 0.0f;
        originHeight = transform.localPosition.y;

        {
            plate = Instantiate(originalPlate);
            plate.transform.parent   = this.transform;
        }

        //  高さを初期化
        {
            Vector3 localHeight = transform.localPosition;
            localHeight.y = originHeight + height.Evaluate( timer );

            transform.localPosition = localHeight;
        }


    }
    void Update()
    {
        Vector3 localPosition = transform.localPosition;

        // update tower
        localPosition.y = originHeight + height.Evaluate( timer );
        transform.localPosition = localPosition;

        // update grid
        float adjusHeight = 0.05f;
        localPosition.y = originHeight + height.Evaluate( 1.0f ) + adjusHeight;
        plate.transform.position    = localPosition;
        plate.transform.eulerAngles = Vector3.zero;

        timer += timeSpeed*Time.deltaTime;

        this.enabled = ( timer <= 1.0f );
        plate.SetActive( timer <= 1.0f );

    }
    public void End()
    {
        timer = 1.0f;
    }


}

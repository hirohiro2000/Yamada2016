using UnityEngine;
using System.Collections;

public class ResourceAppear : MonoBehaviour
{
    public  GameObject[]    c_CompleteEmission  =   null;
    public  GameObject      c_DuringSound       =   null;

    private GameObject      m_rDuringSound      =   null;

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
			plate.transform.parent = transform;
		}
	
		//  高さを初期化
		{
			Vector3 localHeight = transform.localPosition;
			localHeight.y = originHeight + height.Evaluate(timer);

			transform.localPosition = localHeight;
		}

        //  建設中の音
        if( c_DuringSound ){
            GameObject  rObj    =   Instantiate( c_DuringSound );
            Transform   rTrans  =   rObj.transform;
            rTrans.position     =   transform.position;
            rTrans.parent       =   transform;

            m_rDuringSound      =   rObj;
        }
	}
    void Update()
    {
        timer += timeSpeed * Time.deltaTime;
        timer =  Mathf.Min( timer, 1.0f );

		Vector3 localPosition = transform.localPosition;

		// update tower
		localPosition.y = originHeight + height.Evaluate(timer);
		transform.localPosition = localPosition;

		// update grid
		float adjusHeight = 0.05f;
		localPosition.y = originHeight + height.Evaluate(1.0f) + adjusHeight;
		plate.transform.position = localPosition;
		plate.transform.eulerAngles = Vector3.zero;

		

        //  建設完了
        if( timer >= 1.0f ){
            //  建設中の音を削除
            if( m_rDuringSound ){
                Destroy( m_rDuringSound );
            }

            //  完了時に発生させるオブジェクトを生成
            if( c_CompleteEmission != null ){
                for( int i = 0; i < c_CompleteEmission.Length; i++ ){
                    if( !c_CompleteEmission[ i ] )  continue;

                    GameObject  rObj    =   Instantiate( c_CompleteEmission[ i ] );
                    Transform   rTrans  =   rObj.transform;
                    rTrans.position     =   transform.position;
                }
            }
        }

		this.enabled = (timer < 1.0f);
		plate.SetActive(timer < 1.0f);

	}
    public void End()
    {
        timer = 1.0f;
    }
	 public bool IsEnd()
    {
        return timer >= 1.0f;
    }
}

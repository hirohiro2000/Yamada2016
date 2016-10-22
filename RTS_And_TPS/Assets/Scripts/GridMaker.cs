using UnityEngine;
using System.Collections;

public class GridMaker : MonoBehaviour
{
	[SerializeField]
	Vector3 cellsNum = Vector3.zero;

	[SerializeField]
	Vector3 cellSize = Vector3.zero;

	[SerializeField]
	Vector3 LineSize = Vector3.zero;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnValidate()
	{
		//子供を削除
		Transform[] destroyChildren = transform.GetComponentsInChildren<Transform>();
		for (int i = 0; i < destroyChildren.Length; i++)
		{
			Destroy(destroyChildren[i].gameObject);
		}
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

		//X軸を生成
		//GameObject xAxis = Instantiate(cube);
		//xAxis.transform.localScale = new Vector3(cellSize.x * (int)cellsNum.x, LineSize.y, LineSize.z);
		//xAxis.transform.position = 
		//for (int xAxis = 0; xAxis < length; xAxis++)
		//{

		//}
	}

}

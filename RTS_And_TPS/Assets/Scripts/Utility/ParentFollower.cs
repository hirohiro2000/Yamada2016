using UnityEngine;
using System.Collections;

/*	
 *	親のオブジェクトを親子解除して追跡します(生成前から親子付けされている必要あり)
 */
public class ParentFollower : MonoBehaviour {

	[HideInInspector]
	public Transform _followParent;

	public Transform followParent
	{
		get
		{
			return _followParent;
        }
	}

	void Awake()
	{
		followParent = transform.parent;
		transform.parent = followParent.parent;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (followParent == null)
			return;
		transform.position = followParent.position;
	
	}


}

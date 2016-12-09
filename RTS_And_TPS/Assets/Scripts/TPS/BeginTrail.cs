using UnityEngine;
using System.Collections;

//ParentFollowerトレイル用に拡張するスクリプト
[RequireComponent(typeof(ParentFollower))]
public class BeginTrail : MonoBehaviour {

	ParentFollower follower;
	int c = 2;
	// Use this for initialization
	void Awake () {
		follower = GetComponent<ParentFollower>();
		follower.enabled = false;
    }

	// Update is called once per frame
	void Update () {
		c--;
		if(c > 0)
		{
            transform.position = transform.position + (follower.followParent.position - transform.position).normalized * 0.001f;
		}
		else
		{
			follower.enabled = true;
		}
	}
}

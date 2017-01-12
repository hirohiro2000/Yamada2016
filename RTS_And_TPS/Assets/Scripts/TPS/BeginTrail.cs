using UnityEngine;
using System.Collections;

//ParentFollowerトレイル用に拡張するスクリプト
[RequireComponent(typeof(ParentFollower))]
[RequireComponent(typeof(TrailRenderer))]
public class BeginTrail : MonoBehaviour {

	ParentFollower follower;
	TrailRenderer trail;
	public Material alphaMat;
	Material startMat;
	int c = 2;
	float alpha;
	// Use this for initialization
	void Awake () {
		follower = GetComponent<ParentFollower>();
		trail = GetComponent<TrailRenderer>();
		startMat = trail.material;
        follower.enabled = false;
    }

	// Update is called once per frame
	void Update () {
		c--;
		if (follower.followParent != null)
		{

			if (c > 0)
			{
				transform.position = transform.position + (follower.followParent.position - transform.position).normalized * 0.001f;
			}
			else
			{
				follower.enabled = true;
			}
		}

		if(follower.followParent == null)
		{
			alpha += 1.0f / trail.time * Time.deltaTime;
			trail.material.Lerp(startMat, alphaMat, alpha);

        }
	}
}

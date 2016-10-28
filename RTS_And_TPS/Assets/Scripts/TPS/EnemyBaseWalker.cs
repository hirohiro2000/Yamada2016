﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyBaseWalker : NetworkBehaviour {

	public float walkSpeed;

	public bool isStopToAttack = true;

	Transform basePoint;

	NavMeshAgent agent;

	TPS_EnemyAttacker attacker;

	// Use this for initialization
	void Start () {
        //  サーバーでのみ処理を行う
        if( !isServer ) return;

		attacker = GetComponent<TPS_EnemyAttacker>();
		agent = GetComponent<NavMeshAgent>();
		basePoint = GameObject.Find("Homebase").transform;

        agent.SetDestination( basePoint.position );
    }
	
	// Update is called once per frame
	void Update () {
        //  サーバーでのみ処理を行う
        if( !isServer ) return;

        //agent.SetDestination(basePoint.position);
        agent.speed = walkSpeed;
        bool stop = false;

        if (isStopToAttack)
        {
            //撃ってたら動かない
            if (attacker.isAttack)
                stop = true;
        }

        if(stop)
        {
            agent.Stop();
        }
        else
        {
            agent.Resume();
        }
		
		//Vector3 direction = basePoint.position - transform.position;
		//direction.y = .0f;
		//direction.Normalize();

		//transform.position = transform.position + direction * Time.deltaTime * walkSpeed;

		//transform.rotation = Quaternion.LookRotation(direction);

	}
}

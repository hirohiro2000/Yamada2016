﻿using UnityEngine;
using System.Collections;

// [使用しないでください]
public class TPS_PlayerStateMoveGround : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    { 
        //新しいステートに移り変わった時に実行
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //ステートが次のステートに移り変わる直前に実行
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        //スクリプトが貼り付けられたステートマシンに遷移してきた時に実行
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        //スクリプトが貼り付けられたステートマシンから出て行く時に実行
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //MonoBehaviour.OnAnimatorMoveの直後に実行される
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("--------------");

        //最初と最後のフレームを除く、各フレーム単位で実行
        AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        foreach (var info in currentAnimatorClipInfo)
        {
            Debug.Log(info.clip.name);
        }

        Debug.Log("--------------");

    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //MonoBehaviour.OnAnimatorIKの直後に実行される
    }

}
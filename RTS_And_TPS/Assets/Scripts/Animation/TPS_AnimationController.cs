using UnityEngine;
using System.Collections;

public class TPS_AnimationController : MonoBehaviour
{
    [SerializeField]
    Animator animator = null;

    public void ChangeStateMove()
    {
        animator.enabled = true; 
    }

    public void ChangeStateIdle()
    {
        animator.enabled = false;
    }

}

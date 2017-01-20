using UnityEngine;
using System.Collections;

public class RTS_PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator    animator    = null;

    bool        forward     = false;
    float       idlingTimer = 0.0f;     // [ -1 ~ +1 ]
        
    public void ChangeStateMove( float speed )
    {
        forward = true;
        animator.SetBool( "Forward", true );
        animator.SetFloat( "Speed", speed );
    }
    public void ChangeStateIdle()
    {
        if ( forward )
        {
            forward = false;
            idlingTimer = -1.0f;
        }
        animator.SetBool( "Forward", false );
        animator.SetFloat( "Speed", 0.0f );
    }

    public void Update()
    {          
        if ( !forward )
        {
            idlingTimer += 0.05f*Time.deltaTime;
            if ( idlingTimer > 1.0f )
            {
                idlingTimer = 1.0f;
            }
            animator.SetFloat( "IdlingTimer", idlingTimer );
        }
    }

}



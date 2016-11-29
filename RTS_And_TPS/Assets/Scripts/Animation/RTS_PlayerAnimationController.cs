using UnityEngine;
using System.Collections;

public class RTS_PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator animator = null;
        
    public void ChangeStateMove( float speed )
    {
        animator.SetBool( "Forward", true );
        animator.SetFloat( "Speed", speed );
    }
    public void ChangeStateIdle()
    {
        animator.SetBool( "Forward", false );
        animator.SetFloat( "Speed", 0.0f );
    }


//   public void Update()
//   {
//       if ( Input.GetKey(KeyCode.W) )           ChangeStateMove(15.0f);
//       else if ( Input.GetKey(KeyCode.D) )      ChangeStateMove(5.0f);
//       else                                     ChangeStateIdle();
//   }

}



using UnityEngine;
using System.Collections;

public class TPS_PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator animator = null;
    
    public enum InputDpad {
        eFORWARD,
        eBACK,
        eRIGHT,
        eLEFT,
        eNONE
    }

    private Hashtable       inputFlags = null;
    private InputDpad       currentMoveDir = InputDpad.eNONE;


    public void Start()
    {
        inputFlags = new Hashtable();
        inputFlags[InputDpad.eFORWARD] = "Forward";
        inputFlags[InputDpad.eBACK]    = "Back";
        inputFlags[InputDpad.eRIGHT]   = "Right";
        inputFlags[InputDpad.eLEFT]    = "Left";
    }
    
    public void ChangeStateMove(InputDpad flag)
    {
        if ( flag == currentMoveDir )   return;

        animator.SetBool( (string)inputFlags[flag], true );
        
        if (flag != InputDpad.eNONE)
        {
            animator.SetBool((string)inputFlags[currentMoveDir], false);
            currentMoveDir = flag;
        }
    }
    public void ChangeStateIdle()
    {
        if ( currentMoveDir == InputDpad.eNONE )    return;
        animator.SetBool( (string)inputFlags[currentMoveDir], false );        
        currentMoveDir = InputDpad.eNONE;
    }


//   public void Update()
//   {
//       if ( Input.GetKey(KeyCode.W) )          ChangeStateMove(InputDpad.eFORWARD);
//       else if ( Input.GetKey(KeyCode.S) )     ChangeStateMove(InputDpad.eBACK);
//       else if ( Input.GetKey(KeyCode.A) )     ChangeStateMove(InputDpad.eLEFT);
//       else if ( Input.GetKey(KeyCode.D) )     ChangeStateMove(InputDpad.eRIGHT);
//       else                                    ChangeStateIdle();
//
//   }

}



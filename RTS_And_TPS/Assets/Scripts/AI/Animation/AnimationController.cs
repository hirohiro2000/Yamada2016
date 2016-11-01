using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/**
*@breif Animatorのラッパークラス
*/
public class AnimationController : MonoBehaviour {

    private Animator m_animator;

//    [SerializeField, HeaderAttribute("このアニメーターの遷移フラグ変数名の名前配列")]
  //  bool[] m_state_transition_flg_name;

	// Use this for initialization
	void Start () {
       m_animator = transform.FindChild("ModelRoot").GetComponent<Animator>();
        if(!m_animator)
        {
            Debug.Log(gameObject.name + "AnimationController::m_animator is null!");
        }
        
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void SetFloat(string param_name,float val)
    {
        m_animator.SetFloat(param_name, val);
    }

   public void SetTrigger(string trigger_name)
    {
        m_animator.SetTrigger(trigger_name);
    }

    public AnimatorStateInfo GetCurrentAnimatorStateInfo()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0);
    }

}

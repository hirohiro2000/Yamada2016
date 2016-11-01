using UnityEngine;
using System.Collections;

public class RendererSwitch : MonoBehaviour {

    [SerializeField]
    private MeshRenderer mesh_renderer = null;

    bool enable_switch;

    private float switch_second = 0.5f;

    private bool is_enable = true;

	// Use this for initialization
	void Start () {
        if(!mesh_renderer)
            mesh_renderer = GetComponent<MeshRenderer>();
        //StartCoroutine(SwitchLoop());
        //Activate();
	}
	
    IEnumerator SwitchLoop()
    {
		
        while (is_enable)
        {
            enable_switch = !enable_switch;
            mesh_renderer.enabled = enable_switch;
            yield return new WaitForSeconds(switch_second);
        }
    }

    public void Activate()
    {
        is_enable = true;
        StartCoroutine(SwitchLoop());
    } 

    public void Disable()
    {
        mesh_renderer.enabled = true;
        is_enable = false;
    }
        

	// Update is called once per frame
	void Update ()
    {

    }

    public void ReturnColor()
    {
        mesh_renderer.material.color = Color.white;
    }

    public void ChangeColor(Color col)
    {
        mesh_renderer.material.color = col;
    }


}

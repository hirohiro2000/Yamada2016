using UnityEngine;
using System.Collections;

public class NamakemosanTeam : MonoBehaviour {

    FadeController m_controller = null;

    void Awake()
    {
        m_controller = GetComponent<FadeController>();
    }

	// Use this for initialization
	void Start () {

        m_controller.BeginFade(FadeController.FadeMode.Out, Color.white, 0.5f,
            () =>
            {
                StartCoroutine(DisplayRogo());
            });
	
	}

    IEnumerator DisplayRogo()
    {
        yield return new WaitForSeconds(0.5f);

        m_controller.BeginFade(FadeController.FadeMode.In, Color.white, 0.5f, BeginGameFade);
    }
	
    void BeginGameFade()
    {
        GameObject.Find("FadeObject").GetComponent<FadeController>().BeginFade(FadeController.FadeMode.In, Color.black, 0.5f);
    }

	// Update is called once per frame
	void Update () {
	
	}
}

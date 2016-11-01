using UnityEngine;
using System.Collections;

// 使用しないでください
public class SoundManagerDemo : MonoBehaviour {

    GameObject go;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            int id = SoundManager.FindOfIndex("sen_ge_kijuu01");
            go = SoundManager.Create(id, Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Destroy(go);
            go = null;
        }

        if (go != null)
        {
            AudioSource src = go.GetComponent<AudioSource>();
            if (src.time > 2.2f)
            {
                src.time = 0.0f;
            }
        }
    }

}

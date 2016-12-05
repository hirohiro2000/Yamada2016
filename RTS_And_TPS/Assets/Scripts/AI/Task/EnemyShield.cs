using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyShield : MonoBehaviour {



    [SerializeField, HeaderAttribute("防ぐ攻撃tag")]
    private List<string> DeffendTagList = new List<string>();

    void OnTriggerEnter(Collider other)
    {
        if(DeffendTagList.Contains( other.gameObject.tag))
        {
            Destroy(other.gameObject);
        }

    }

}

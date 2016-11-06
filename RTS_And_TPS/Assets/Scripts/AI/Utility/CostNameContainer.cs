using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
/**
*@note Navmeshの全エリア名前取得できたら消す
*/
public class CostNameContainer : MonoBehaviour {


    //[SerializeField]
    //private string[] LayerNameArray = new string[1];

    [SerializeField]
    private StringList LayerNameArray = new StringList();

    //private List<Text> text_list;

    public StringList GetLayerNameArray()
    {
        return LayerNameArray;
    }

    void Start()
    {
        //text_list = new List<Text>();
        //var ui = GameObject.Find("Cost");
        //for(int i  = 0; i < ui.transform.childCount;i++)
        //{
        //    text_list.Add(ui.transform.GetChild(i).GetComponent<Text>());
        //}

        //var test = NavMesh.CalculateTriangulation();
       
    }


    void Update()
    {
        //for(int i = 0; i < LayerNameArray.Length; i++ )
        //{
        //    text_list[i].text = LayerNameArray[i] + ":" + NavMesh.GetAreaCost(NavMesh.GetAreaFromName(LayerNameArray[i]));
        //}
    }
}

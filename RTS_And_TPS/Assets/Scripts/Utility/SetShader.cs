using UnityEngine;
using System.Collections;

public class SetShader : MonoBehaviour {

    public  Shader  c_Shader    =   null;
    public  Color   c_Color     =   Color.white;

	// Use this for initialization
	void    Awake()
    {
        //  アクセスを取得
        Renderer    rRenderer       =   GetComponent< Renderer >();
        if( !rRenderer )    return;

        //  シェーダーをセット
        Color       color           =   rRenderer.material.color;
        rRenderer.material.shader   =   c_Shader;
        rRenderer.material.color    =   c_Color;

        //  非アクティブ化
        this.enabled    =   false;
    }
}

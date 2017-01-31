
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class NetGUI_ShowControl : MonoBehaviour {
    public  bool    c_ShowGUI   =   false;

	// Use this for initialization
	void    Start()
    {
        MyNetworkManagerHUD rNetHUD =   NetworkManager.singleton.GetComponent< MyNetworkManagerHUD >();
        if( rNetHUD ){
            rNetHUD.showGUI    =   c_ShowGUI;
        }
	}
}

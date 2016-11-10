
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class EditData_Config : MonoBehaviour {

    public  int                         m_MapWidth      =   1;
    public  int                         m_MapHeight     =   1;
    public  int                         m_MapDepth      =   1;

	// Use this for initialization
	void    Start()
    {
	    
	}
	
	// Update is called once per frame
	void    Update()
    {
	    
	}

    //  操作
    public  void    Clear()
    {
        m_MapWidth  =   1;
        m_MapHeight =   1;
        m_MapDepth  =   1;

        foreach( Transform rTrans in transform ){
            Destroy( rTrans.gameObject );
        }
    }
}

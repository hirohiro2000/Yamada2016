
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class ExplodedManager : MonoBehaviour {

    public  List< GameObject >  m_rExpObjList   =   new List< GameObject >();
    public  List< Collider >    m_rDebrisList   =   new List< Collider >();
	public int SavingDeblisCount = 0;
	// Use this for initialization
	void    Start()
    {
	    
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  空になった項目を削除
        //for( int i = 0; i < m_rDebrisList.Count; i++ ){
        //    m_rDebrisList.Remove( null );
        //}
        for( int i = 0; i < m_rExpObjList.Count; i++ ){
            m_rExpObjList.Remove( null );
        }
		SavingDeblisCount = 0;

	}
}

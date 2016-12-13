
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class C4Shell_Control : MonoBehaviour {
    public  List< GameObject >  m_rC4List   =   null;

	// Use this for initialization
	void    Start()
    {
	    
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  無効になった項目を削除
        for( int i = 0; i < m_rC4List.Count; i++ ){
            if( m_rC4List[ i ] )    continue;
            
            //  項目を削除
            m_rC4List.RemoveAt( i );

            //  最初に戻る
            i   =   -1;
        }
	}
}


using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class GameManager : NetworkBehaviour {

    //  シーン内の状態
    enum    State{
        Ready,      //  準備時間
        InGame,     //  ゲーム中
        Reuslt,     //  結果発表
    }

    //  内部パラメータ
    private State           m_SceneState    =   State.Ready;

	// Use this for initialization
	void    Start()
    {

	}
	
	// Update is called once per frame
	void    Update()
    {
        //  状態に応じて処理を行う
	    switch( m_SceneState ){
            case    State.Ready:    Update_Ready();     break;
            case    State.InGame:   Update_InGame();    break;
            case    State.Reuslt:   Update_Result();    break;
        }
	}
    void    Update_Ready()
    {

    }
    void    Update_InGame()
    {

    }
    void    Update_Result()
    {

    }
}

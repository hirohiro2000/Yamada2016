
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class CharacterMover : MonoBehaviour {

    private NetworkIdentity m_rIdentity =   null;

	CharacterController _characterController;
	CharacterController characterController
	{
		get
		{
			if (_characterController == null)
			{
				_characterController = GetComponent<CharacterController>();
			}
			return _characterController;
		}
	}

	Vector3 totalSpeed;
	// Use this for initialization
	void    Start()
    {
	    m_rIdentity =   GetComponent< NetworkIdentity >();   
	}


	public void AddSpeed(Vector3 speed)
	{
		totalSpeed += speed;
    }
    public  Vector3  GetTotalSpeed()
    {
        return  totalSpeed;
    }

	// Update is called once per frame
	void Update ()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rIdentity.isLocalPlayer )    return;

		characterController.Move(totalSpeed * Time.deltaTime);
		totalSpeed = Vector3.zero;
    }
}

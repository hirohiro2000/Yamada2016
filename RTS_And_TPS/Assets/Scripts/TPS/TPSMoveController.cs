
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSMoveController : MonoBehaviour
{
    //  ダッシュチェック
    class   DashChecker
    {
        enum    State{
            Neutral,
            Push,
            DoublePush,
        }

        private KeyCode m_CheckKey      =   KeyCode.W;
        private float   m_Threshold     =   0.0f;
        private float   m_PushTimer     =   0.0f;
        private State   m_State         =   State.Neutral;

        public  DashChecker( KeyCode _CheckKey, float _Threshold ){
            m_CheckKey  =   _CheckKey;
            m_Threshold =   _Threshold;
        }

        public  void    Update(){
            //  状態に応じた処理を行う
            switch( m_State ){
                case    State.Neutral:      Update_Neutral();       break;
                case    State.Push:         Update_Push();          break;
                case    State.DoublePush:   Update_DoublePush();    break;
            }
        }
        private void    Update_Neutral(){
            //  入力があれば一回押された状態へ
            if( Input.GetKeyDown( m_CheckKey ) ){
                m_State     =   State.Push;
                m_PushTimer =   0.0f;
            }
        }
        private void    Update_Push(){
            //  一定時間以内に入力があれば次の状態へ
            if( Input.GetKeyDown( m_CheckKey ) ){
                m_State     =   State.DoublePush;
                return;
            }

            //  一回目の入力から一定時間経過でニュートラルに戻る
            m_PushTimer     +=  Time.deltaTime;
            m_PushTimer     =   Mathf.Min( m_PushTimer, m_Threshold );
            if( m_PushTimer >= m_Threshold ){
                m_State     =   State.Neutral;
            }
        }
        private void    Update_DoublePush(){
            //  キーが離されたらニュートラルに戻る
            if( !Input.GetKey( m_CheckKey ) ){
                m_State     =   State.Neutral;
            }
        }

        public  bool    IsDash(){
            return  m_State == State.DoublePush;
        }
    }
    //  ダッシュ処理
    class   DashControl
    {
        //  状態
        enum    State{
            Neutral,    //  通常
            Dash,       //  ダッシュ
            Recovery,   //  回復
        }

        //  固定パラメータ
        private KeyCode     c_DashKey           =   KeyCode.W;
        private float       c_DashThreshold     =   0.2f;
        private float       c_MaxEnergy         =   10.0f;
        private float       c_RecoveryEnergy    =   2.5f;
        private float       c_DashEnergy        =   2.5f;
        private float       c_DashSpeed         =   5.0f;
        private float       c_StartCost         =   4.0f;

        //  パラメータ
        private float       m_Energy            =   0.0f;
        private State       m_State             =   State.Neutral;
        private DashChecker m_rDashChecker      =   null;
        
        //  初期化
        public  DashControl()
        {
            m_rDashChecker  =   new DashChecker( c_DashKey, c_DashThreshold );
            m_Energy        =   c_MaxEnergy;
        }

        //  更新
        public  void    Update( TPSMoveController _rParent )
        {
            //  ダッシュ入力チェッカー更新
            m_rDashChecker.Update();

            //  状態に応じた処理を行う
            switch( m_State ){
                case    State.Neutral:  Update_Neutral( _rParent );     break;
                case    State.Dash:     Update_Dash( _rParent );        break;
                case    State.Recovery: Update_Recovery( _rParent );    break;
            }

            //  ＵＩ更新
            TPSBoosterBar.SetGage( m_Energy / c_MaxEnergy );
        }
        private void    Update_Neutral( TPSMoveController _rParent )
        {
            //  ダッシュ 
            if( m_rDashChecker.IsDash()
            &&  m_Energy >= c_StartCost ){
                m_State =   State.Dash;
                return;
            }

            //  エネルギー回復
            m_Energy    +=  c_RecoveryEnergy * Time.deltaTime;
            m_Energy    =   Mathf.Min( m_Energy, c_MaxEnergy );
        }
        private void    Update_Dash( TPSMoveController _rParent )
        {
            //  消費エネルギー計算
            float   energyCost  =   c_DashEnergy * Time.deltaTime;
            //  エネルギーが足りるかどうかチェック
            if( m_Energy < energyCost
            ||  !m_rDashChecker.IsDash() ){
                //  回復状態へ
                m_State =   State.Recovery;
                return;
            }

            //  エネルギー消費
            m_Energy    -=  energyCost;
            m_Energy    =   Mathf.Max( m_Energy, 0.0f );

            //  ダッシュ
            _rParent.characterMover.AddSpeed( _rParent.transform.forward.normalized * c_DashSpeed );
        }
        private void    Update_Recovery( TPSMoveController _rParent )
        {
            //  エネルギー回復
            m_Energy    +=  c_RecoveryEnergy * Time.deltaTime;
            m_Energy    =   Mathf.Min( m_Energy, c_MaxEnergy );

            //  ニュートラルへ
            if( !m_rDashChecker.IsDash() ){
                m_State =   State.Neutral;
                return;
            }
        }

        public  float   GetEnergy(){
            return  m_Energy;
        }
        public  void    UseEnergy( float _UseEnergy ){
            m_Energy    -=  _UseEnergy;
            m_Energy    =   Mathf.Max( m_Energy, 0.0f );
        }
    }

    private float           c_AnimeSpeed    =   0.5f;
    private float           c_AnimeRatio    =   0.73f;
    private NetworkIdentity m_rIdentity     =   null;
    private TPSPlayer_HP    m_rTPSHP        =   null;

	[SerializeField]
	float speed = .0f;

	//[SerializeField, Range(0.1f, 1.0f)]
	//float sensitivity = 0.1f;

	Vector3 inputDir;
	Vector3 m_KnockBackSpeed;

	Rigidbody _rigidBody;
	Rigidbody rigidBody
	{
		get
		{
			if (_rigidBody == null)
			{
				_rigidBody = GetComponent<Rigidbody>();
			}
			return _rigidBody;
		}
	}

	CharacterMover _characterMover;
	CharacterMover characterMover
	{
		get
		{
			if (_characterMover == null)
			{
				_characterMover = GetComponent<CharacterMover>();
			}
			return _characterMover;
		}
	}

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

    // アニメーション用
    TPS_PlayerAnimationController m_animationController = null;

    // 加速用
  	float   avoidStepPower  = 18.0f;
  	float   boosterPower    = 0.0f;
	float   maxDrivingPower = 45.0f;
    Vector3 impluseForce    = Vector3.zero;
	float   dampRate        = 5.0f;
    float   avoidEnergy     = 4.0f;

    //  ダッシュ用
    private DashControl m_rDashControl  =   null;

    // ２回連続に押下したかの判定用
    class DoublePress
    {
        public enum STATE { eIdle, ePress, eNotPress, eDoublePressDown, eDoublePress }

        KeyCode m_key;
        float   m_timer;
        STATE   m_state;
        public DoublePress( KeyCode key )
        {
            m_key = key;
            m_timer = 0.0f;
            if ( Input.GetKey(m_key) )  m_state = STATE.ePress;
            else                        m_state = STATE.eIdle;
        }

        public  void Update()
        {
            switch (m_state)
            {
                case STATE.eIdle:               IdleProc();             break;
                case STATE.ePress:              PressProc();            break;
                case STATE.eNotPress:           NotPressProc();         break;
                case STATE.eDoublePressDown:    DoublePressDownProc();  break;
                case STATE.eDoublePress:        DoublePressProc();      break;
                default:break;
            }
        }
        private void IdleProc()
        {
            if ( Input.GetKey(m_key) )
            {
                m_state = STATE.ePress;
                m_timer = 0.0f;
            }
        }
        private void PressProc()
        {
            m_timer += Time.deltaTime;
            if ( Input.GetKeyUp(m_key) )
            {
                if (m_timer > 0.2f)
                {
                    m_state = STATE.eIdle;
                }
                else
                {
                    m_state = STATE.eNotPress;
                }

                m_timer = 0.0f;
            }
        }
        private void NotPressProc()
        {
            if (Input.GetKey(m_key))
            {
                m_state = STATE.eDoublePressDown;
            }

            m_timer += Time.deltaTime;
            if (m_timer > 0.2f)
            {
                m_state = STATE.eIdle;
            }

        }
        private void DoublePressDownProc()
        {
            m_state = STATE.eDoublePress;
        }
        private void DoublePressProc()
        {
            if ( Input.GetKey(m_key) == false )
            {
                m_state = STATE.eIdle;
            }
        }

        public STATE GetState()         { return m_state;   }
        public KeyCode GetKeyCode()     { return m_key;     }

    }
    DoublePress[] m_doublePressKeys;

	// Use this for initialization
	void Start()
	{
        m_rIdentity =   GetComponent< NetworkIdentity >();
        m_rTPSHP    =   GetComponent< TPSPlayer_HP >();

        m_doublePressKeys = new DoublePress[4];
        m_doublePressKeys[0] = new DoublePress( KeyCode.W );
        m_doublePressKeys[1] = new DoublePress( KeyCode.S );
        m_doublePressKeys[2] = new DoublePress( KeyCode.A );
        m_doublePressKeys[3] = new DoublePress( KeyCode.D );

        m_animationController = GetComponent< TPS_PlayerAnimationController >();

        //  パラメータ初期化
        m_rDashControl  =   new DashControl();
    }

	// Update is called once per frame
	void Update()
	{
		//GameWorldParameterで強制的に書き換える
		{
			speed = GameWorldParameter.instance.TPSPlayer.WalkSpeed;
			avoidStepPower = GameWorldParameter.instance.TPSPlayer.StepPower;
		}
		//  自分のキャラクター以外は処理を行わない
		if ( !m_rIdentity.isLocalPlayer )    return;

        if ( Camera.main == null )          return;
        Transform camTransform = Camera.main.transform;

		Vector3 forward = ( transform.position - camTransform.position );
		forward.y = .0f;
		forward.Normalize();

		Vector3 right = Vector3.Cross(Vector3.up, forward);
		right.Normalize();

        Vector2 controllerAxis = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") );

		Debug.Log(controllerAxis);
        inputDir = Vector3.zero;



		inputDir += right   * (controllerAxis.x * speed);
		inputDir += forward * (controllerAxis.y * speed);

        //  瀕死状態なら減速
        if( m_rTPSHP.m_IsDying ){
            inputDir    =   inputDir * 0.15f;
        }

		characterMover.AddSpeed(inputDir);

        //  ダッシュ （瀕死状態は不可）
        if( !m_rTPSHP.m_IsDying ){
            m_rDashControl.Update( this );
        }

        // アニメーション 
        float   totalSpeed  =   characterMover.GetTotalSpeed().magnitude;
        //if( totalSpeed > 0.0f
        //&&  characterController.isGrounded )
        //{
        //rigidBody.velocity += addDir;
                
        // 加速処理（ブースト＆ステップ）
        //UpdateAdjustMoveForce( forward, right );

        // アニメーション
        if( controllerAxis.sqrMagnitude    >  0.0f )
        {
            float expValue = 0.05f;

            if ( controllerAxis.y > expValue )
                m_animationController.ChangeStateMove(TPS_PlayerAnimationController.InputDpad.eFORWARD);
            else if ( controllerAxis.y < -expValue )
                m_animationController.ChangeStateMove(TPS_PlayerAnimationController.InputDpad.eBACK);

            if ( controllerAxis.x > expValue )
                m_animationController.ChangeStateMove(TPS_PlayerAnimationController.InputDpad.eRIGHT);
            else if ( controllerAxis.x < -expValue )
                m_animationController.ChangeStateMove(TPS_PlayerAnimationController.InputDpad.eLEFT);

            m_animationController.ChangeSpeed( totalSpeed * c_AnimeRatio * c_AnimeSpeed );
        }
        else
        {
            m_animationController.ChangeStateIdle();
            m_animationController.ChangeSpeed( 1.0f );
        }

        //  浮いている間はアニメーションを再生しない 
        if( !characterMover.isGrounded)
		{
            m_animationController.ChangeSpeed( 0.0f );
        }


		//ノックバック処理
		{
			characterMover.AddSpeed(m_KnockBackSpeed);
			m_KnockBackSpeed *= Mathf.Max(1.0f - 8.0f * Time.deltaTime,.0f);

		}
	}

    void UpdateAdjustMoveForce( Vector3 forward, Vector3 right )
    {
        // キーの更新
        foreach (var key in m_doublePressKeys)
        {
            key.Update();
        }

        // 加速方向（上下左右）
        Vector3[] directions = {
            forward,
            -forward,
            -right,
            right
        };

        // ステップによる力取得
        Vector3 impluse = AvoidPower( directions );

        // 
        impluseForce += impluse;

        // 上限を超えないようclamp処理
        if (impluseForce.sqrMagnitude > maxDrivingPower * maxDrivingPower)
        {
            impluseForce = impluseForce.normalized * maxDrivingPower;
        }

        // ブーストダッシュによる力取得
        //Vector3 velocity = BoostPower( directions );

        // 移動処理
        Vector3 totalForce = impluseForce;// + velocity;   // インパルス + 等加速運動
        float   minPower   = 0.5f*0.5f;                 // ※適当な値を設定しています
        if (totalForce.sqrMagnitude > minPower )
        {
            characterMover.AddSpeed(totalForce);
        }

        // ステップ力の減速処理
        if (impluseForce.sqrMagnitude > 0.5f * 0.5f)
        {
            impluseForce = Vector3.Lerp(impluseForce, Vector3.zero, dampRate * Time.deltaTime);
        }

		characterMover.AddSpeed(totalForce);

	}
    Vector3 AvoidPower(Vector3[] directions)
    {
        Vector3 outVec = Vector3.zero;
        for (int i = 0; i < m_doublePressKeys.Length; i++)
        {
            DoublePress key = m_doublePressKeys[i];

            if (key.GetState() == DoublePress.STATE.eDoublePressDown)
            {
                //  エネルギーチェック
                if( m_rDashControl.GetEnergy() >= avoidEnergy ){
                    //  エネルギー消費
                    m_rDashControl.UseEnergy( avoidEnergy );

                    //  加速
                    outVec += directions[i] * avoidStepPower;
                }
            }
        }
        return outVec;
    }
    Vector3 BoostPower(Vector3[] directions)
    {
        Vector3 outVec = Vector3.zero;

        for (int i = 0; i < m_doublePressKeys.Length; i++)
        {
            DoublePress key = m_doublePressKeys[i];

            if (key.GetState() == DoublePress.STATE.eDoublePress)
            {
                outVec += directions[i] * boosterPower;
            }
        }

        return outVec;

    }
    
    	//敵との衝突用
	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("Field"))
			return;
		if(Mathf.Abs(hit.normal.y) < 0.5f )
		{
			Vector3 dir = hit.normal;
			dir.y = .0f;
			dir.Normalize();
			transform.position = transform.position + dir * 0.2f;
			m_KnockBackSpeed += dir * 20.0f;
		}
    }

}

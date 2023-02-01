using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;
using DG.Tweening;



// プレイヤーコントローラー
//	※一時停止処理を行うため、処理順をGameControllerや他のエネミーより先にすること

public class PlayerController :	_CharaBase
{

	readonly float JumpPower		        = 360.0f;			// ジャンプ力
	readonly float JumpCoolTime_	        = 0.10f;			// ジャンプクールタイム
	readonly int   smokeIntervalTime        = 30;				// 煙出現間隔
	readonly float smokeDestryTime	        = 1.0f;				// 煙の破棄時間
	readonly float ropeLengthMaxSpeed       = 2.0f;				// ロープの長さが限界の時の速度(この速度で引っ張る)
	readonly float damageBlinkTime	        = 2f;				// ダメージで点滅する時間
	readonly float disconnectCoolTime_      = 1.0f;				// コネクトオブジェを切り離すクールタイム
	readonly float throwConnectObjeCoolTime = 0.8f;				// コネクトオブジェを投げた時の硬直時間
	readonly float DistanceToFeet_			= 0.715f;			// 地面までの距離

	// ドア脱出時
	readonly float Exit_MoveToCenterTime = 0.2f;				// 脱出時、ドアの中央へキャラが移動する時間(ズレていても中心にくるよう)
	readonly float Exit_FadeTime		 = 1.0f;				// 脱出時のフェード時間

	// アニメーション
	readonly string[] AnimName = { "Idle",	 "Walk",   "JumpStart",	  "Idle", "Damage",	"Throw", "Ladder", "Back",
								   "Idle_c", "Walk_c", "JumpStart_c", "Idle", "Damage",	"Throw", "Ladder", "Back_c"};
	enum Anim
	{
		NONE = -1,
		IDLE,	WALK,	JUMP,	DOOR,	DAMAGE,	  THROW,   LADDER,	 BACK,
		IDLE_C,	WALK_C,	JUMP_C,	DOOR_C,	DAMAGE_C, THROW_C, LADDER_C, BACK_C
	};
	readonly float[] AnimSpeed = {	0.5f,	  1,		1,			   0.5f,   1,		 1.0f,	  1.0f,		0.1f,
									0.5f,	  1,		1,			   0.5f,   1,		 1.0f,	  1.0f,		0.1f};
	Anim playAnimNum = Anim.NONE;

	// エフェクト
	readonly float[] smokeYOffset =	{ 0.70f, 0.60f };

	// パワーアップ
	readonly int	 maxPower =	3;
	readonly float[] arrayMoveSpeed	=							// 速度テーブル
	{
		0.50f, 1.50f, 2.0f
	};
	readonly float[] arrayJumpPower	=							// ジャンプパワーテーブル
	{
		8f,	12.5f, 12.5f
	};

	readonly float connectObjXOffset = 0.3f;					// コネクトオブジェの X ずらし値


	[SerializeField]
	public int id;												// プレイヤー識別番号
	[SerializeField]
	float DistanceToFeet;										// 身体の中心から足元までの距離
	[SerializeField]
	string SmokeFile, LandingSmokeFile;							// エフェクト
	[SerializeField]
	BoxCollider2D bodyCollider;									// コライダー
	[SerializeField]
	GameObject radarObject;										// レーダー
	[SerializeField]
	public BoxCollider2D catchCollider;							// キャッチ用のコライダー

	// コネクト用のコライダー
	[SerializeField]
	public GameObject connectPlug;								// コネクト部分
	[SerializeField]
	public ConnectObjController	connectPlugObj;
	[SerializeField]
	BoxCollider2D connectPlugCollBox;
	[SerializeField]
	CircleCollider2D connectPlugCollCircle;
	[SerializeField]
	CapsuleCollider2D connectPlugCollCapsule;
	[SerializeField]
	Animator[] anim;
	[SerializeField]
	public SpriteRenderer[] sprite;
	[SerializeField]
	PlayerCatchObjectCollisionController catchObjCollisionCont;	// キャッチオブジェクトリリース後にレイヤーを変更する用
	[SerializeField]
	PlayerRadarController radar;								// レーダー
	[SerializeField]
	PlayerWallController[] wall;								// 左右の壁チェック用

	[SerializeField]
	bool	canThrow, canDetach, canClimb;


	// 状態
	public bool	 isThrowMotionEnd { set;	get; }				// true	: 投げるモーションが終了した
	public float dir { set;	get; } = 0;							// 向き 1	or -1
	public bool  isStop { set; get; } = false;					// true	: スクロール中
	bool  isJump	 = false;									// ジャンプフラグ
	bool  isHanging;											//@@ ぶら下がり ture
	int	  numPower;												// 0:通常(足枷)、-1:逆パワーアップ、1:パワーアップ
	int	  walkCount	 = 0;
	float jumpCoolTime;											// ジャンプのクールタイム
	public float moveSpeedX { set; get; }						// 移動したい方向と速度
	public float moveSpeedY { set; get; }
	public bool isStatusPanel { set; get; }						// ステイタスパネル表示中


	GameObject smoke;											// 煙
	GameObject landingSmoke;									// 着地の時の煙
																// 移動用
	// バックアップ
	float   an_speedBak;										// アニメーション速度バックアップ
	Vector3	localScaleBak;										// スケールバックアップ
	Vector3	positionBak;										// 表示座標バックアップ
	Vector3	prevPosition;										// ロープが伸びた時に戻す用(ハシゴの上り下りで使う)

	// ドア
	public bool	isDoor { set; get; }							// true	: ドア前か
	public DoorController door { set; get; }					// ドア
	bool isExitDoor	= false;									// true	: 脱出した

	// キャッチ	& コネクト
	readonly string	connectObjDefaultPrefabName	= "IronBall";	// 最初のコネクトオブジェプレハブ名
	readonly float connectObjDefaultRadius = 0.174f;			// コネクトオブジェの半径デフォルト値(手に収まるようにずらす)
	public GameObject hitConnectObj	{ set; get;	}				// 身体に衝突しているコネクトオブジェ
	public bool	isBodyHit {	set; get; }							// true	: 身体に当たっている
	public bool	isConnectObjHold { set;	get; }					// true	: コネクトオブジェ(ロープにつながっているもの)を持っている
	public bool	isConnectObjPullWait { set;	get; }				// true	: コネクトオブジェを引き寄せている最中
	public bool	isConnectObj { set;	get; }						// true	: コネクトオブジェがチェーンにつながれている
	public float connectObjRadius {	set; get; }					// コネクトオブジェの半径
	
	string objHitSe = "obj_hit_destroy";						// コネクトオブジェをなにかにぶつけたときの音(キャッチオブジェクトより得る)
	string objAttackSe = "";									// コネクトオブジェでの攻撃音(キャッチオブジェクトより得る)
	string connectObjPrefabName	= "IronBall";					// コネクトオブジェプレハブ名
	int	   connectObjHp, connectObjMaxHp;						// コネクトオブジェの HP
	bool   isConnecting;										// true	: コネクトオブジェをロープに接続中(完全につながった後 false)
	float  disconnectCoolTime;									// 切り離したときのクールタイム

	// キャッチオブジェ(コネクトできるオブジェ)の回転値
	RigidbodyConstraints2D catchObjRbConstraints = RigidbodyConstraints2D.FreezeRotation;

	// コネクトプラグ(コネクトオブジェからアニメとスプライトと剛体をコピーして使う)
	public Rigidbody2D connectPlugRB { set;	get; }				// コネクトプラグの剛体
	SpriteRenderer connectPlugSp;								// コネクトプラグのスプライト
	PolygonCollider2D connectPlugCollPoly;						// コネクトプラグのコライダー
	Animator connectPlugAnim;									// コネクトプラグのアニメーター

	// ObiCollider
	Obi.ObiRigidbody2D obiRb;									// ロープが切れないようにするよう


	void Start()
	{
		rb				= GetComponent<Rigidbody2D>();
		connectPlugRB	= connectPlug.GetComponent<Rigidbody2D>();
		connectPlugSp	= connectPlug.GetComponent<SpriteRenderer>();
		connectPlugAnim	= connectPlugObj.GetComponent<Animator>();
		smoke			= (GameObject)Resources.Load(SmokeFile);
		landingSmoke	= (GameObject)Resources.Load(LandingSmokeFile);
		obiRb			= GetComponent<Obi.ObiRigidbody2D>();

		// バックアップ
		an_speedBak	  =	anim[0].speed;
		localScaleBak =	transform.localScale;
		positionBak	  =	transform.position;

		// 登録
		Global.player[id] =	gameObject;
		Global.plc[id]	  =	this;
		Global.numPlayer++;

		Restart();
	}

	// リスタート
	public override void Restart()
	{
		InitCheckGround();

		// 状態
		isDead			 = false;

		//isPrevGround	 = false;
		//onGround		 = true;
		//isFloor			 = false;
		numPower		 = 0;
		isThrowMotionEnd = true;
		isJump			 = false;
		isHanging		 = false;
		//isLanding		 = false;
		jumpCoolTime	 = 0;									// ジャンプクールタイム
		walkCount		 = 0;
		moveSpeedX		 = 0;
		moveSpeedY		 = 0;
		dir				 = 1;									// 右向きから始める
		obiRb.kinematicForParticles = false;					// true だとロープが伸びるのでオフにする
		prevPosition	 = xy;									// 前回の座標を保存

		// ドア
		isDoor	   = false;
		isExitDoor = false;
		door	   = null;

		hitConnectObj    = null;
		isBodyHit		 = false;
		isConnectObjHold = false;
		isConnectObjPullWait = false;
		isConnectObj	 = false;
		connectObjPrefabName = connectObjDefaultPrefabName;		// 最初に持っているコネクトオブジェ
		connectObjRadius = connectObjDefaultRadius;				// コネクトオブジェの半径
		connectObjHp	 = 0;									// コネクトオブジェのHP
		connectObjMaxHp  = 0;									// コネクトオブジェのMaxHP
		isConnecting	 = false;
		disconnectCoolTime = 0;									// コネクトオブジェ切り離し時のクールタイム

		//キャッチオブジェ(コネクトできるオブジェ)
		catchObjRbConstraints = RigidbodyConstraints2D.FreezeRotation;	// 回転値

		// ハシゴ
		prevonLadderMid	 = false;
		onLadderMid		 = false;
		onLadderBottom	 = false;

		// イベント
		ev				 = null;

		// 移動
		moveSpeedX = 0;
		moveSpeedY = 0;

		walkCount =	0;
		numPower  =	maxPower / 2;								// パワーアップ値

		anim[0].speed =	an_speedBak;
		anim[1].speed =	an_speedBak;
		transform.localScale = localScaleBak;
		transform.position	 = positionBak;
		rb.isKinematic	= false;
		sprite[0].color	= new Color(1, 1, 1, 1);
		sprite[1].color	= new Color(1, 1, 1, 1);

		// コネクトアイテム
		connectObjRadius   = connectObjDefaultRadius;			// 最初のコネクトオブジェ
		isConnectObj	   = true;
		Rigidbody2D objRb = GameObject.Find(connectObjPrefabName +	"_Clone").GetComponent<Rigidbody2D>();
		connectPlugRB.mass = objRb.mass;						// _Clone から重さを貰う
		disconnectCoolTime = 0;									// 切り離したときのクールタイム

		// キャッチコライダー
		catchCollider.enabled =	false;							// 最初に鉄球を持っているのでキャッチコライダーはオフにする

		PlayAnim(Anim.IDLE);

		// カメラ座標設定
		SetupCamera();
	}

	void Update()
	{
		// ルーム
		//		int	xRoom =	(int)( (transform.position.x + Define.xRoomSize	/ 2f) /	Define.xRoomSize);
		//		int	yRoom =	(int)(-(transform.position.y - Define.yRoomSize	/ 2f) /	Define.yRoomSize);
		// ローカル座標
		//		float xx =	(transform.position.x +	Define.xRoomSize / 2f) - xRoom * Define.xRoomSize;
		//		float yy = -(transform.position.y -	Define.yRoomSize / 2f) - yRoom * Define.yRoomSize;

		//Global.debug.text	= "xy:"	+ xx.ToString()	+ "," +	yy.ToString();

		// キー入力〇
		// 物理演算×

		/*	if ( game.getOver()) { an.speed	= 0; return; }
			if (!game.getGame()) { return; }
			if (isDead)	{ return;}
			if (Exit())	{ return;}
		*/


		if (Global.isPauseStart){ Pause(true);}
		if (Global.isPauseEnd){	Pause(false);}
		if (Global.isPause)	{ return; }							// 一時停止か?

		if (Global.fade.Check()) { return; }					// フェード中か
		if (Global.isOver) { return; }							// ゲームオーバーか
		if (Global.isMessage) {	return;}						// メッセージ表示中か?
		if (isExitDoor)	{ return; }								// ドアをくぐったか
		if (isDamage) {	return;	}								// ダメージ中か?

		prevPosition = xy;										// 前回の座標を保存

		HoldConnectObjControll();								// コネクトオブジェを持っている時

		CheckGround(bodyCollider.size.x / 2f - 0.05f, DistanceToFeet_);	// 足元の状態を得る

		LandingControll();										// 停止か着地
		StopControll();											// 停止か着地
		if(UpdateCamera())										// 画面切り替え
		{
			ConnectObjMoveBodyIntoPos();						// コネクトオブジェを身体へよせる(鎖につながっているが、手に持っていないとき)
		}

		if (Global.isMapScroll) {	return;	}					// スクロール中か
		JumpControll();											// ジャンプ
		DoorControll();											// ドア
		ConnectObjControll();									// キャッチオブジェクト

		LeftRightMoveControll();								// 左右移動

		// アニメーション切り替え
		sprite[0].enabled =	!isConnectObjHold;
		sprite[1].enabled =	isConnectObjHold;

		UpDownMoveControll();									// 上下移動

		EventControll();										// イベント
	}

	void FixedUpdate()
	{
		// キー入力×
		// 物理演算〇

		// プレイヤーとコネクトプラグの距離
		if(isStop){ return;}

		if (!Global.isMapScroll && Global.isPause) { return; }	//	一時停止か?
		if (Global.fade.Check()) { return; }					// フェード中か
		if (Global.isOver) { return; }							// ゲームオーバーか
		if (isExitDoor)	{ return; }								// ドアをくぐったか

		if (isDamage) {	return;	}								// ダメージ中か?

		if(isHanging){ return;}//@@

		// 移動
		rb.velocity = new Vector2(moveSpeedX, moveSpeedY + rb.velocity.y);
	}

	
	// ハシゴの昇り降り
	void UpDownMoveControll()
	{
		// ジャンプ中ならここでおわり
		if (isJump)	{ return; }

		// ジャンプのクールタイム中ならここでおわり(ジャンプ直後に入力すると重力0で動作するため)
		if (jumpCoolTime > 0) {	return;	}

		// ハシゴの上に来たら重力オン・オフ
		rb.gravityScale	= onLadder ? 0:	1;

		moveSpeedY = 0;

		// 左右の足元を調べ降りる
		if (onLadder)
		{
			rb.velocity	= Vector2.zero;
			if (Global.CheckKey(id,	Key.down))
			{
				PlayAnim(Anim.LADDER);
				moveSpeedY = -arrayMoveSpeed[numPower];
			}
		}

		// 左右の足元より少し上を調べ降りる
		bool isLittleHigher	= CheckFeet(y -	DistanceToFeet + rayDistance, bodyCollider.size.x / 2f, maskLadder);
		if (isLittleHigher)
		{
			rb.velocity	= Vector2.zero;
			if (Global.CheckKey(id,	Key.up))
			{
				PlayAnim(Anim.LADDER);
				moveSpeedY = arrayMoveSpeed[numPower];
			}
		}

		// 移動してない場合はアニメーション停止
		if(rb.velocity == Vector2.zero && moveSpeedY ==	0 && playAnimNum ==	Anim.LADDER)
		{
			PauseAnim(true);									// ハシゴの上で入力がない時はアニメーション停止
		}
	}

	// プレイヤー座標よりカメラ位置を設定する
	void SetupCamera()
	{
		Global.mapArea.x =  (int)(x / Define.xRoomSize);		// スクロールしないならば mapArea の値を書き換えるだけで ok
		Global.mapArea.y = -(int)(y / Define.yRoomSize);
		Global.camController.SetupCamera(xy);					// カメラ位置設定
	}

	// マップスクロール
	bool UpdateCamera()
	{
		if (Global.isMapScroll) { return false; }				// マップスクロール中は処理しない

		float xdir = 0,	ydir = 0;
		float xx = x - Global.mapArea.x * Define.xRoomSize;
		float yy = y + Global.mapArea.y * Define.yRoomSize;
		float x_ = Global.camera.transform.position.x;
		float y_ = Global.camera.transform.position.y;
		float z_ = Global.camera.transform.position.z;

		// 右へ
		if (     xx >=  Define.xScrollPosition){ xdir =  1; Global.mapArea.x++;}
		// 左へ
		else if (xx <= -Define.xScrollPosition){ xdir = -1; Global.mapArea.x--;}
		// 下へ
		else if (yy <= -Define.yScrollPosition){ ydir = -1; Global.mapArea.y++;}
		// 上へ3
		else if (yy >=  Define.yScrollPosition){ ydir =  1; Global.mapArea.y--;}
		// スクロールしないならここで終わり
		else { return false;}

		// プレイヤーの移動後座標(画面端よりすこし内側)
		Vector2	afterPos = new Vector2(x + xdir * Define.mapScrollDistance, y + ydir * Define.mapScrollDistance);

		// プレイヤー移動開始
		transform.DOLocalMove(afterPos, Define.scrollTime)
			.OnUpdate(() =>	{ rb.velocity =	Vector2.zero; })
			.OnComplete(() => {	Global.isMapScroll = false; PauseAnim(false); });

		// アニメーション
		if (onGround &&	!onLadder) { PlayAnim(Anim.WALK); }

		// スクロール
		Global.camController.StartCameraMove(afterPos);

		return true;
	}

	// コネクトオブジェを身体へよせる(鎖につながっているが、手に持っていないとき)
	void ConnectObjMoveBodyIntoPos()
	{
		if (isConnectObj && !isConnectObjHold)
		{
			connectPlugObj.Reset();
			connectPlug.transform.DOLocalMove(new Vector2(0, connectPlug.transform.localPosition.y), Define.scrollTime)
				.OnUpdate(() =>
				{
					connectPlugRB.velocity = new Vector2(0,	0);	// つなげている間はこうしないと鎖がどこかへいってしまう
				});
		}	}

	// コネクトオブジェを投げる・引き寄せる
	void ConnectObjControll()
	{
		// クールタイム
		if (disconnectCoolTime > 0)	{ disconnectCoolTime -=	Time.deltaTime;	return;	}

		// ハシゴを昇り降りしている場合は終了
		//if (playAnimNum == Anim.LADDER){ return; }

		// キーが放されるのを待つ
		//if (WaitiToBeDisconnectKeyReleased &&	Global.CheckKey(id,	Key.disconnect)) { return; }
		//WaitiToBeDisconnectKeyReleased = false;

		// 投げる・引き寄せる(階段の時も〇)
		if (isConnectObj &&	isThrowMotionEnd &&	Global.CheckPressKey(id, Key.pullThrow))
		{
			if (!isConnectObjHold) { PullObj(true);	}							// 引き寄せる
			else if(playAnimNum != Anim.LADDER){ StartCoroutine("throwConnectObj"); }	// 投げる(階段の時は×)
		}
		// コネクトオブジェを切り放す(階段の時も〇)
		else if	(!isConnecting && Global.CheckPressKey(id, Key.down) &&	Global.CheckKey(id,	Key.disconnect))
		{
			if (isConnectObj &&	onGround)
			{
				//WaitiToBeDisconnectKeyReleased = true;
				disconnectCoolTime = disconnectCoolTime_;		// クールタイム
				PlaySE(SE.PLY_DISCONNECT);
				StartCoroutine(DisconnectObj(isConnectObjHold));// ホールドしている時は左右に移動する
			}
		}
		// オブジェクトを鎖につなげる(階段の時は×)
		else if	(!isConnectObj && radar.GetNearObject()	&& Global.CheckPressKey(id,	Key.disconnect) && playAnimNum != Anim.LADDER)
		{
			//WaitiToBeDisconnectKeyReleased = true;
			disconnectCoolTime = disconnectCoolTime_;		// クールタイム
			hitConnectObj =	radar.GetNearObject();			// レーダーの中から一番近いゲームオブジェクトを探す
			isConnecting = true;
			PlaySE(SE.PLY_CONNECT);
			connectPlug.transform.DOMove(hitConnectObj.transform.position, 0.5f)
				.OnUpdate(() =>
				{
				})
				.OnComplete(() =>
				{
					isConnecting = false;
					CatchObject(hitConnectObj);				// オブジェクトを鎖につなぐ
					hitConnectObj =	null;
					PullObj(true);							// オブジェクトを引き寄せる
					numPower = 1;
				});
		}
	}

	// コネクトオブジェを持っている時に身体の位置に合わせて座標をずらす
	void HoldConnectObjControll()
	{
		if (isThrowMotionEnd &&	isConnectObjHold &&	isBodyHit)
		{
			connectPlugRB.velocity = new Vector2(0,	0);

			// 背中を向けている時は中央へ、正面を向いている時は左右のどちらかに配置
			float x = -connectObjXOffset * dir;
			if(playAnimNum == Anim.LADDER || playAnimNum == Anim.LADDER_C){ x = 0;}

			connectPlug.transform.localPosition	= new Vector2(x, -connectObjRadius);
		}
	}

	// 左右移動
	int hangingCount = 0;
	void LeftRightMoveControll()
	{
		moveSpeedX = 0;

		//Dbg.Print("hangingCount:" + hangingCount);
		//@@
		float connectDir = GetAngle(connectPlugObj.transform.position);
	//	Dbg.Print("connectDir:" + connectDir.ToString());
		if(connectDir > 90 - 45 && connectDir < 90 + 45
		&& connectPlugObj.onGround)
		{
			//Dbg.Print("skip:" + connectDir.ToString());
			hangingCount++;
			if(hangingCount >= 1)
			{
				hangingCount = 1;
				//connectPlugRB.bodyType = RigidbodyType2D.Static;
				//connectPlugRB.mass = 30000;
			}


			if (	 Global.CheckPressKey(id, Key.left ))
			{
				rb.AddForce(Vector2.left * 30f);
					//rb.velocity = new Vector2(-5, moveSpeedY + rb.velocity.y);
			}
			else
			if (	 Global.CheckPressKey(id, Key.right ))
			{
				rb.AddForce(Vector2.right * 30f);
			}

			isHanging = true;
			//connectPlugR
			return;
		}
		else
		{
			hangingCount = 0;
			isHanging = false;
		}

		// 落下
		//if (onPrevGround &&	!onGround && !isFeetLadder)	{ PlayAnim(Anim.JUMP); }
		if (onPrevGround &&	!onGround && !onLadder)	{ PlayAnim(Anim.JUMP); }

		// 投げ終わってなければここで終わり
		if (!isThrowMotionEnd) { return; }

		// 昇り降り中、左右に壁があるならば移動できない
		if (wall[0].isWallHit && wall[1].isWallHit)	{ return; }

		// ハシゴは左右移動できないが
		//if (isFeetLadderMid	&& !onGround) {	return;	}
		if (onLadderMid	&& !onGround) {	return; }

		// 移動(ハシゴの上は左右移動できない
		float a	= 0;
		if (	 Global.CheckKey(id, Key.left )) { a = -1; }
		else if	(Global.CheckKey(id, Key.right)) { a =	1; }
		if (a != 0)
		{
			dir	= a;
			// 左右に移動
			// プレイヤーからコネクトオブジェまでの距離(この距離以上ならば移動できない)
			float lengthBallAndPlayer =	Vector2.Distance(transform.position, connectPlug.transform.position);
			if (x >	connectPlug.transform.position.x) {	lengthBallAndPlayer	*= -1; }
			moveSpeedX = arrayMoveSpeed[numPower] *	dir;

			if (onGround &&	!isJump) { PlayAnim(Anim.WALK);	Smoke(true); }

			sprite[0].flipX	= sprite[1].flipX =	dir	== -1;				// 左右反転しない

			// 空中に浮いている時は移動しない	@@

			//if(!onGround){ moveSpeedX = 0;}

			// ロープの長さが限界ならコネクターを引っ張る
			if (dir	== 1 &&	lengthBallAndPlayer	< -Define.ropeMaxLength)
			{
				connectPlugRB.velocity = new Vector2(ropeLengthMaxSpeed, connectPlugRB.velocity.y);
			}
			else if	(dir ==	-1 && lengthBallAndPlayer >	Define.ropeMaxLength)
			{
				connectPlugRB.velocity = new Vector2(-ropeLengthMaxSpeed, connectPlugRB.velocity.y);
			}

			// レーダーの向き
			radarObject.transform.localScale = new Vector3(dir,	1, 1);
		}
	}

	// 着地
	void LandingControll()
	{
		if (isLanding)
		{
			// 普通のハシゴに接触していなくて、床に設置したハシゴに接触している場合は地面ではないので終了
			if(!prevonLadderMid	&& onLadderBottom){	return;	}

			isJump		 = false;
			isLanding	 = false;
			jumpCoolTime = JumpCoolTime_;						// クールタイム(この時間はジャンプできない)
			//if (!isOnPlayer) { Smoke(false); }
			Smoke(false);
			PlaySE(SE.PLY_LANDING);
			rb.velocity	= new Vector2(rb.velocity.x, 0);

			// ハシゴの上に着地した場合はめり込まないように補正する
			if (CheckFeet(y	- DistanceToFeet, bodyCollider.size.x / 2f, maskLadderMid))
			{
				Vector2	pos	= new Vector2(x, y);
				RaycastHit2D r = Physics2D.Raycast(pos,	Vector2.down, 1f, maskLadderMid);
				y =	r.point.y +	0.715f;
				rb.gravityScale	= 0;
				moveSpeedY = 0;
			}
		}
	}
	
	// 停止
	void StopControll()
	{
		if(!isThrowMotionEnd) {	return;	}

		// 停止
		if (onGround &&	moveSpeedX == 0)
		{
			if (playAnimNum	!= Anim.LADDER && !isJump)
			{
				PlayAnim(Anim.IDLE);
			}
			if (moveSpeedX == 0) { rb.velocity = new Vector2(moveSpeedX, rb.velocity.y); }
		}
	}

	// ジャンプ
	void JumpControll()
	{
		// クールタイム(時間中はジャンプできない)
		if (jumpCoolTime > 0) {	jumpCoolTime -=	Time.deltaTime;	return;	}
		// 投げるモーション中は終了
		if (!isThrowMotionEnd) { return; }
		// ジャンプ中ならば終了
		if (isJump)	{ return; }
		// 左右に壁がある(ハシゴ)の場合は終了
		if (wall[0].isWallHit && wall[1].isWallHit)	{ return; }
		// 足元が地面についていなければ終了
		if (!onGround) { return; }
		// 足元が地面についていても	LadderBottom の場合は終了
		if (onLadderBottom)	{ return; }

		// ハシゴにアニメーション中はジャンプできない
		if(playAnimNum == Anim.LADDER){	return;	}

		// 地面についていて、足元の少し上にハシゴはなくて、下を押してなくて、ジャンプを押した瞬間にジャンプ!
		// 下ボタンを禁止しているのはハシゴの上でしたボタンを押しながらジャンプすると落ちるため
		if (Global.CheckPressKey(id, Key.jump) && connectPlugObj.CheckJump())
		{
			jumpCoolTime       = JumpCoolTime_;					// クールタイム(この時間はジャンプできない)
			isJump		       = true;
			rb.velocity	       = Vector2.zero;
			rb.angularVelocity = 0;
			rb.gravityScale	   = 1;								// ハシゴあたりでジャンプしたときの対策
			rb.AddForce(Vector2.up * arrayJumpPower[numPower] *	JumpPower);
			PlaySE(SE.PLY_JUMP);
			PlayAnim(Anim.JUMP);
		}
	}

	// コネクトオブジェを投げる
	IEnumerator	throwConnectObj()
	{
		// ここは投げるモーションの管理だけで、実際には Anim.THROW アニメーションのモーションに合わせて投げている1
		// (PlayerThrowController.cs の AnimationEvent 9フレーム目)

		catchCollider.enabled =	false;
		//connectPlugObj.Attack();								// ここではなく、玉が手を離れる瞬間に呼び出す
		PlaySE(SE.PLY_THROW);
		PlayAnim(Anim.THROW);									// 実際にはこのアニメーション中から呼び出される
																// 攻撃音
		if(objAttackSe != ""){ PlaySound(SE.ID.ConnectPlug, objAttackSe, 0.25f);}

		// 身体が横に動かないようにする
		rb.constraints =  RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

		isThrowMotionEnd = false;
		isConnectObjHold = false;
		connectPlugRB.bodyType    = RigidbodyType2D.Kinematic;
		connectPlugRB.constraints =	catchObjRbConstraints;

		// 時間をおいてからフラグ解除
		yield return new WaitForSeconds(throwConnectObjeCoolTime);
		isBodyHit =	false;
		
		// 身体が動くようにする
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	// コネクトオブジェを落とす
	void dropObj()
	{
		catchCollider.enabled	  =	false;
		isThrowMotionEnd		  =	true;
		isConnectObjHold		  =	false;
		connectPlugRB.bodyType	  =	RigidbodyType2D.Dynamic;
		connectPlugRB.constraints =	catchObjRbConstraints;
		isBodyHit				  =	false;

		Vector2	dir	= Define.throwDirection;
		if (sprite[0].flipX) { dir.x *=	-1;	}
		connectPlug.layer =	LayerMask.NameToLayer(Define.layerConnectObj);
		connectPlugRB.AddForce(dir * Define.DropPower);
		numPower = 2;
	}

	// オブジェクトを引き寄せる		bBody == true : 身体の中心に引き寄せる
	public void	PullObj(bool bBody)
	{
		// 身体の中心に引き寄せる
		if(bBody)
		{
			PlaySE(SE.PLY_PULL);
			// 引き寄せは DOTween で行う
			connectPlug.transform.DOLocalMove(new Vector2(-connectObjXOffset * dir, -connectObjRadius), 0.1f)
				.OnUpdate(() =>
				{
					connectPlugRB.velocity = new Vector2(0,	0);							// つなげている間はこうしないと鎖がどこかへいってしまう
				})
				.OnComplete(() =>
				{
					connectPlug.layer =	LayerMask.NameToLayer(Define.layerHoldObj);
					catchCollider.enabled =	true;
					connectPlugRB.constraints =	RigidbodyConstraints2D.FreezeRotation;	// 引き寄せ後はオブジェクトの回転禁止
					connectPlugObj.Reset();
					isConnectObjHold = true;
					numPower = 1;
					//SetRopeBreak(true);													// ロープを切れなくする(プレイヤーがロープの影響を受けないがロープは切れる)
				});
		}
		// 足元に引き寄せる(階段の昇り降り・画面切り替え時)
		else
		{
			connectPlugObj.Reset();
			connectPlug.transform.DOLocalMove(new Vector2(0, - DistanceToFeet),	Define.scrollTime)
				.OnUpdate(() =>
				{
					connectPlugRB.velocity = new Vector2(0,	0);	// つなげている間はこうしないと鎖がどこかへいってしまう
				});
		}
	}

	// コネクトオブジェが壊れる
	public void	ConnectObjClash()
	{
		if (connectObjHp > 0)
		{
			connectObjHp--;
			if (connectObjHp ==	0)
			{
				ReleaseConnectObj(false);

				// プレハブを読み込み
				//GameObject obj = (GameObject)Resources.Load(Define.folderCatchObj + connectObjPrefabName);
				// クローンをコピーしてそれをバラバラにする。バラバラになるオブジェクトは読み込みでは動作しないため
				GameObject obj = GameObject.Find(connectObjPrefabName +	"_Clone");
				GameObject gc =	Instantiate(obj, connectPlug.transform.position, connectPlug.transform.rotation);
				ObjectController gcOC =	gc.GetComponent<ObjectController>();
				//Rigidbody2D gcRb = gc.GetComponent<Rigidbody2D>();
				//gcRb.bodyType = RigidbodyType2D.Dynamic;
				gcOC.IimmediateClash();
			}
			else if	(connectObjHp <= connectObjMaxHp / 2)
			{	// ダメージ
				PlaySound(SE.ID.Player, objHitSe);
				connectPlugAnim.Play(Define.animDamage);
			}
		}
	}

	// コネクトオブジの接続解除
	// in  : isCreateObj:切り離したオブジェクトを作成する
	// out : 切り離した放したオブジェクトの GameObject
	public GameObject ReleaseConnectObj(bool isCreateObj)
	{
		if (!isConnectObj) { return	null; }

		connectPlugObj.Reset();
		isConnectObj		 = false;
		isConnectObjHold	 = false;
		connectPlugSp.sprite = null;
		hitConnectObj		 = null;

		// 小さい CircleCollider	を作る
		connectPlugCollCircle.radius   = 0.05f;
		connectPlugRB.mass			   = 0.1f;					// 軽くする
		connectPlugCollCircle.offset   = new Vector2(0,	0);
		connectPlugCollBox.enabled	   = false;
		connectPlugCollCircle.enabled  = true;
		connectPlugCollCapsule.enabled = false;
		connectPlugAnim.enabled		   = false;
		connectPlugAnim.runtimeAnimatorController = null;
		connectPlugSp.sprite = null;

		// スケールを 1 倍にする
		connectPlug.gameObject.transform.localScale = new Vector3(1, 1, 1);

		if (connectPlugCollPoly	!= null)						// PolygonColliderは削除
		{
			Destroy(connectPlugCollPoly);
			connectPlugCollPoly	= null;
		}
		// コネクト部分を身体の中心へ引き寄せる
		connectPlug.transform.DOLocalMove(new Vector2(0, -connectObjRadius), 0.1f)
			.OnUpdate(() =>
			{
				connectPlugRB.velocity = new Vector2(0,	0);		// つなげている間はこうしないとロープがどこかへいってしまう
			});

		// 放すオブジェクトは新規に作成する
		GameObject go =	null;
		if (isCreateObj)
		{
			// クローンをコピーする。バラバラになるオブジェクトは読み込みでは動作しないため
			GameObject obj = GameObject.Find(connectObjPrefabName +	"_Clone");

			// プレハブを読み込み
			//GameObject obj = (GameObject)Resources.Load(Define.folderCatchObj + connectObjPrefabName);

			// 実体化
			go = Instantiate(obj, connectPlug.transform.position, connectPlug.transform.rotation);

			// リリース後にレイヤーを変更するために GO を与える
			catchObjCollisionCont.SetReleaseObj(go);

			// 剛体のパラメーターをコピーする
			Rigidbody2D	rdb	= go.GetComponent<Rigidbody2D>();
			rdb.bodyType = RigidbodyType2D.Dynamic;
			rdb.velocity = connectPlugRB.velocity;
			rdb.angularVelocity	= connectPlugRB.angularVelocity;

			// HPも設定
			_CharaBase cb = go.GetComponent<_CharaBase>();
			cb.Hp =	connectObjHp;
			cb.maxHp = connectObjMaxHp;
			cb.isDontMove = false;
			cb.isThrowStart = true;
			if(isConnectObjFlip){ cb.isLeft = sprite[0].flipX;}

			//connectPlugObj.SetCoolTime();					// コネクトオブジェにもクールタイムを設定する(切り離した時にコネクトプラグに当たるのを回避するため)
		}
		return go;
	}

	// コネクトオブジェを切り離す	in	bPushOut ==	true:向いているほうに押し出す
	public void	startDisconnectObj(bool	bPushOut = true)
	{
		StartCoroutine(DisconnectObj(bPushOut));
	}

	// コネクトオブジェを捨てる
	IEnumerator	DisconnectObj(bool bPushOut)
	{
		// ホールドをやめる
		connectPlugObj.Reset();
		isConnectObjHold = false;
		numPower = 2;

		// 向いている方に押し出す
		if (bPushOut)
		{
			float x	= sprite[0].flipX ?	-1 : 1;
			connectPlugCollCircle.radius = 0.05f;
			connectPlugRB.AddForce(new Vector2(x * 0f, 500f));
		}
		// すこし待つ
		yield return new WaitForSeconds(0.3f);
		ReleaseConnectObj(true);
	}

	// 対象オブジェクトをキャプチャする
	void CatchObject(GameObject	go)
	{
		SpriteRenderer spr = go.GetComponent<SpriteRenderer>();

		// スケールをコピー
		connectPlug.gameObject.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y, 1);

		// 子オブジェクトをコピーしてバラバラにする
		_CharaBase ch = go.GetComponent<_CharaBase>();
		if(ch.isDisassemble){ Global.DisassemblyGameObject(go);	}
		
		// スプライトを貰う
		connectPlug.GetComponent<SpriteRenderer>().sprite =	spr.sprite;

		// 各コライダーをコピー
		BoxCollider2D box =	go.GetComponent<BoxCollider2D>();
		if (box	!= null)
		{
			connectPlugCollBox.size	= box.size;					// box は size
			connectPlugCollBox.offset =	box.offset;				// と offset
			connectPlugCollBox.enabled = true;
			connectPlugCollCircle.enabled =	false;
			connectPlugCollCapsule.enabled = false;
			//connectPlugcolliderType =	ColliderType.Box;
			if (connectPlugCollPoly	!= null) { Destroy(connectPlugCollPoly); connectPlugCollPoly = null; }	// PolygonColliderは削除
		}
		CircleCollider2D cir = go.GetComponent<CircleCollider2D>();
		if (cir	!= null)
		{
			connectPlugCollCircle.radius = cir.radius;			// circle は radius
			connectPlugCollCircle.offset = cir.offset;			// と offset
			connectPlugCollBox.enabled = false;
			connectPlugCollCircle.enabled =	true;
			connectPlugCollCapsule.enabled = false;
			//connectPlugcolliderType =	ColliderType.Circle;
			if (connectPlugCollPoly	!= null) { Destroy(connectPlugCollPoly); connectPlugCollPoly = null; }	// PolygonColliderは削除
		}
		CapsuleCollider2D cap =	go.GetComponent<CapsuleCollider2D>();
		if (cap	!= null)
		{
			connectPlugCollCapsule.size	= cap.size;				// capsule は size
			connectPlugCollCapsule.offset =	cap.offset;			// と offset
			connectPlugCollBox.enabled = false;
			connectPlugCollCircle.enabled =	true;
			connectPlugCollCapsule.enabled = false;
			//connectPlugcolliderType =	ColliderType.Capsule;
			if (connectPlugCollPoly	!= null) { Destroy(connectPlugCollPoly); connectPlugCollPoly = null; }	// PolygonColliderは削除
		}
		PolygonCollider2D pol =	go.GetComponent<PolygonCollider2D>();
		if (pol	!= null)
		{
			// ポリゴンコライダーは頂点情報を削除する関係で毎回新規に作成しコピーする
			if (connectPlugCollPoly	!= null) { Destroy(connectPlugCollPoly); connectPlugCollPoly = null; }	  // PolygonColliderは削除

			connectPlugCollPoly	= connectPlug.gameObject.AddComponent<PolygonCollider2D>();					// PolygonColliderは追加する
			connectPlugCollPoly.pathCount =	pol.pathCount;
			connectPlugCollPoly.points = new Vector2[pol.points.Length];
			//connectPlugcolliderType =	ColliderType.Polygon;
			var	myPoints = connectPlugCollPoly.points;			// こうしないと値は書きこまれない
			for	(int i = 0;	i <	pol.points.Length; i++)
			{
				myPoints[i]	= pol.points[i];
			}
			connectPlugCollPoly.points = myPoints;
			connectPlugCollBox.enabled = false;
			connectPlugCollCircle.enabled =	false;
			connectPlugCollCapsule.enabled = false;
		}

		// 剛体の値も貰う
		Rigidbody2D	rdb	= hitConnectObj.GetComponent<Rigidbody2D>();
		connectPlugRB.gravityScale = rdb.gravityScale;			// 剛体は重力と
		connectPlugRB.mass         = rdb.mass;					// 重さ
		catchObjRbConstraints      = rdb.constraints;			// 回転値

		// 回転を貰う
		connectPlug.transform.rotation = hitConnectObj.transform.rotation;

		// 半径とプレハブを貰う
		_CharaBase conCont = hitConnectObj.GetComponent<_CharaBase>();
		connectObjPrefabName = conCont.prefabName;
		connectObjRadius = conCont.radius;
		connectObjHp     = conCont.Hp;
		connectObjMaxHp	 = conCont.maxHp;

		// アニメーションがあればもらう
		Animator conContAnim = go.GetComponent<Animator>();
		if (conContAnim	!= null)
		{
			connectPlugAnim.enabled	= true;
			connectPlugAnim.runtimeAnimatorController =	conContAnim.runtimeAnimatorController;
			if (connectObjMaxHp > 0 && connectObjHp <=	connectObjMaxHp	/ 2)
			{	// ダメージがある時
				connectPlugAnim.Play(Define.animConnect_Damage);
			}
			else
			{	// ノーダメージの時
				connectPlugAnim.Play(Define.animConnect_Idle);
			}
		}
		else
		{
			connectPlugAnim.enabled	= false;
			connectPlugAnim.runtimeAnimatorController = null;
		}

		isConnectObjFlip = ch.isConnectObjFlip;					// 接続時に反転するか?

		// 効果音をもらう(ダメージ音・攻撃音)
		objHitSe    = conCont.se_damage;
		objAttackSe = conCont.se_attack;

		// フラグ
		isConnectObj = true;

		// キャプチャー元を削除する
		Destroy(go);
	}

	// アニメーション再生
	void PlayAnim(Anim anim_)
	{
		if (isThrowMotionEnd)
		{
			// コネクトオブジェを持っている場合は
			anim[0].Play(AnimName[(int)anim_]);
			anim[1].Play(AnimName[(int)anim_ + AnimName.Length / 2]);
			// 速度は保持しておく
			anim[0].speed =	AnimSpeed[(int)anim_];
			anim[1].speed =	anim[0].speed;

			playAnimNum	= anim_;
		}
	}

	// アニメーション一時停止
	public override	void PauseAnim(bool	bStop)
	{
		anim[0].speed =	bStop ?	0 :	AnimSpeed[(int)playAnimNum];
		anim[1].speed =	anim[0].speed;
	}

	// ドア
	void DoorControll()
	{
		//	if (isDoor && moveSpeedX ==	0 && !game.getDoorClose() && Global.CheckKey(id, Key.down) && !isDoorExit
		//		&& (game.getDoorControll() == null || game.getDoorControll() ==	door))

		if (isDoor && door.CheckExitPossible() && moveSpeedX ==	0 && Global.CheckKey(id, Key.down))
		{
			door.Open();
			isExitDoor = true;

			// 自身をドアの中心へ移動する
			rb.bodyType	= RigidbodyType2D.Kinematic;
			this.transform.DOMove(door.transform.position, Exit_MoveToCenterTime).OnComplete(()	=>
			{	// 自身に設定されている Collider を全てオフ
				var	cols = GetComponents<CircleCollider2D>();
				foreach	(var col in	cols) {	col.enabled	= false; }
				var	boxs = GetComponents<BoxCollider2D>();
				foreach	(var box in	boxs) {	box.enabled	= false; }
			}
			);

			/*if (ironBall)
			{	// コネクトオブジェもドアの中心へ移動する
				ironBall.transform.DOMove(door.transform.position, Exit_MoveToCenterTime);
				// コリジョンも無効
				var	cols = ironBall.GetComponents<CircleCollider2D>();
				foreach	(var col in	cols) {	col.enabled	= false; }
			}*/

			// 自身をフェードアウトする
			sprite[0].DOFade(0,	Exit_FadeTime);
			sprite[1].DOFade(0,	Exit_FadeTime);

			/*if (ironBall)
			{	// コネクトオブジェもフェードアウトする
				ironBall.GetComponent<SpriteRenderer>().DOFade(0, Exit_FadeTime).OnComplete(() =>
				{//	※チェーンのスケールを変えると終了後にエラーがでるので、消すだけにする
					chain.SetActive(false);
				}).SetDelay(Exit_IronBallDelayTime);
				//chain.transform.DOScale(new Vector3(0.1f,	0, 1), Exit_FadeTime).OnComplete(()	=>
				//{
					//chain.SetActive(false);
				//}
				//).SetDelay(Exit_IronBallDelayTime);
			}*/

			Global.game.IncExitPlayer();
			PlaySE(SE.PLY_EXIT);

			/*if (!game.getClear())
			{	// クリアでないならドアを開ける
				game.DoorStart(door);
			}
			ZeroVelocity = true;					// プレイヤーの動きも止める
			OffKinematic = true;
			an.speed	 = 0;
			*/
		}
	}

	// 歩くときの煙
	void Smoke(bool	walking)
	{
		if (walking)
		{	// 歩き
			if ((walkCount % smokeIntervalTime)	== 0)
			{
				Vector3	a =	this.transform.position;
				a.y	-= smokeYOffset[id];
				//Destroy(Instantiate(smoke, a,	Quaternion.identity), smokeDestryTime);
				Global.effect.Start(Eff.Smoke, a);
			}
			walkCount++;
			if (!CheckSound(SE.ID.Player)) { PlaySE(SE.PLY_WALK); }
		}
		else
		{	// 着地
			Vector3	a =	this.transform.position;
			a.y	-= smokeYOffset[id];
			//Destroy(Instantiate(landingSmoke,	a, Quaternion.identity), smokeDestryTime);
			Global.effect.Start(Eff.Landing, a);
		}
	}

	// 点滅
	public override	IEnumerator	DamageBlink(bool bDead)
	{
		gameObject.layer = LayerMask.NameToLayer(Define.layerPlayerThrough);

		Global.camController.Shake(0.5f, 0.2f);					// 画面揺らす(時間・力)

		Vector2	a =	new	Vector2(-1,	4);
		if (sprite[0].flipX) { a.x *= -1; }
		rb.velocity	= a;
		isDamage = true;
		for	(int i = 0;	i <	4; i++)
		{
			sprite[0].DOColor(new Color(1, 1, 1, 0), damageDuration);
			sprite[1].DOColor(new Color(1, 1, 1, 0), damageDuration);
			yield return new WaitForSeconds(0.1f);
			sprite[0].DOColor(new Color(1, 1, 1, 1), damageDuration);
			sprite[1].DOColor(new Color(1, 1, 1, 1), damageDuration);
			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(0.2f);
		gameObject.layer = LayerMask.NameToLayer(Define.layerPlayer);
		PlayAnim(Anim.IDLE);
		isDamage = false;

		// 死亡
		/*if (bDead)
		{
			Dead();	
		}*/
	}

	// コリジョン
	void OnCollisionEnter2D(Collision2D	col)
	{
		if (isDamage) {	return;	}

		// エネミー・エネミー弾
		if (col.gameObject.tag == Define.tagEnemy || col.gameObject.tag	== Define.tagEnemyBullet)
		{
			dropObj();
			PlayAnim(Anim.DAMAGE);
			PlaySE(SE.PLY_DAMAGE);
			Destroy(col.gameObject);
			dir	= col.gameObject.transform.position.x <	x ?	-1:	1;
			sprite[0].flipX	= sprite[1].flipX =	dir	== -1;

			StartCoroutine(DamageBlink(false));					// ダメージ開始
		}
	}

	// 脱出したか
	public bool	CheckExit()
	{
		return isExitDoor;
	}

	// 効果音再生(ゲームオーバー時は再生しない)
	void PlaySE(string snd)
	{
		if (!Global.isOver && Global.isPlayerSE)
		{
			PlaySound(SE.ID.Player, snd);
		}
	}

	// コネクトオブジェのコライダーを有効にする
	public void	ValidConnectObjCollider()
	{
		connectPlug.layer =	LayerMask.NameToLayer(Define.layerConnectObj);
	}

	// イベント
	void EventControll()
	{
		if(ev == null){	return;}
		if(Global.isMessage){ return; }
		if (Global.CheckPressKey(id, Key.up))
		{
			PlaySE(SE.EVENT);
			PlayAnim(Anim.BACK);
			Global.plc[id].message.SetTextStart(ev.message);
		}
	}

		// 座標を前のフレームに戻す
	public void SetPrevPos()
	{
		xy = prevPosition;										// 前回の座標を保存
	}
	

	// ロープが切れるスイッチ		true : 伸びて切れる、false : 切れなくなる
	/*public void SetRopeBreak(bool sw)
	{
		// ※ obiRb.kinematicForParticles が true の場合、落下時のロープが伸びる

		// true でロープが切れなくなるがプレイヤーがロープの動きの影響を受ける(プレイヤーの体重が軽いため)
		// プレイヤーの体重を軽くすると設定をしてもロープがきれるので、プログラム中でこまめにスイッチを設定する
		obiRb.kinematicForParticles = sw;
	}*/
}

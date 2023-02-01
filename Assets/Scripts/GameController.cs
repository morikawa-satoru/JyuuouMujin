using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using DG.Tweening;


public class GameController	: _Base
{

	enum Mode {	Stop, GameStart, Game, Clear, Over };

	readonly string[] yesnoText	= {"<size=150%>Continue?</size>\n<color=white><size=180%>はい</size></color>\n<size=100%>いいえ</size>",
								   "<size=150%>Continue?</size>\n<size=100%>はい</size>\n<color=white><size=180%>いいえ</size></color>" };

	readonly float OverWait			  =	1.0f;					// ゲームオーバー時のコンティニューメッセージ表示までのウエイト時間
	readonly float OverClearSpYpos	  =	3.3f;					// ゲームオーバー・クリアの表示 Y	座標
	readonly float TimeLimitViewDelay =	0.5f;					// 制限時間表示遅延時間

	Mode	mode;
	int		numExitPlayer  = 0;									// 脱出したプレイヤー数
	int		selectContinue = 0;
	float	delayTime	   = 0;									// 遅延時間

	SpriteRenderer	overSp,	clearSp;
	TextMeshPro		continueText;
	DoorController	door;
	//public bool isMenu { set;	get; }							// true	: メニューを開いている

	[SerializeField]
	int		TimeLimit;											// ステージの制限時間
	[SerializeField]
	int		Stage;												// ステージ数
	[SerializeField]
	Text	debug_;												// デバックテキスト


	void Awake()
	{
		Global.Initialize();
		Global.game			 = GetComponent<GameController>();
		Global.camera		 = GameObject.Find("Camera").GetComponent<Camera>();
		Global.cameraUI		 = GameObject.Find("CameraUI").GetComponent<Camera>();
		Global.camController = Global.camera.GetComponent<CameraController>();
		Global.effect		 = GameObject.Find("Effect").GetComponent<EffectController>();
		Global.map			 = GameObject.Find("Grid");
		Global.fade			 = GetComponent<FadeController>();
		Global.debug		 = debug_;
		Global.player[0] = null;
		Global.player[1] = null;

		overSp       = GameObject.Find("OverImage").GetComponent<SpriteRenderer>();
		clearSp      = GameObject.Find("ClearImage").GetComponent<SpriteRenderer>();
		door         = GameObject.Find("Door").GetComponent<DoorController>();
		continueText = GameObject.Find("ContinueText").GetComponent<TextMeshPro>();
		overSp.enabled       = false;
		clearSp.enabled      = false;
		continueText.enabled = false;
	}

	void Start()
	{
		Global.scale	    = 1;								// 画面スケール
		Global.numStage	    = Stage;
		Global.isOver       = false;
		Global.isPlayerSE   = false;							// ゲーム開始時は着地音を鳴らさないようにする
		Global.isPause      = false;
		Global.prevPause    = false;
		Global.isPauseStart = false;
		Global.isPauseEnd   = false;
		Global.isMessage    = false;
		Global.isReset		= false;
		Global.isGame       = false;
		Global.isFocus      = false;
		Global.numPlayer    = 0;
		Global.isMapScroll  = false;

		//Global.mapArea      = new Vector2Int(0, 0);				// 開始

		GameObject.Find("StageText").GetComponent<TextMeshPro>().text =	"Stage:" + Global.numStage.ToString().PadLeft(2, '0');

		GetComponent<SpriteRenderer>().sprite = null;

		mode		  =	Mode.GameStart;
		delayTime	  =	0.5f;									// ゲーム開始時にすこし遅らせる(オブジェクトの地面設置時の音対策)
		numExitPlayer =	0;
		_ =	Global.WaitForAsync(0.5f, () =>	Global.fade.In());
		_ =	Global.WaitForAsync(0.5f, () =>	TimerCounter_.Start(TimeLimit, TimeLimitViewDelay));	// 制限時間、制限時間の表示遅延時間
		_ =	Global.WaitForAsync(1,	  () =>	Global.isPlayerSE =	true);
	}

	void ReStart()
	{
		Scene loadScene	= SceneManager.GetActiveScene();
		SceneManager.LoadScene(loadScene.name);
		Global.isPause = Global.prevPause =	Global.isPauseStart	= Global.isPauseEnd	= false;
	}

	void Update()
	{
		// 一時停止チェック
		Global.isPauseStart	= Global.isPause &&	!Global.prevPause;
		Global.isPauseEnd	= !Global.isPause && Global.prevPause;
		Global.prevPause	= Global.isPause;

		// ゲームの進行チェック
		if (Global.fade.Check()) { return;}
		delayTime -= Time.deltaTime;
		if (delayTime >	0) { return;}

		// モード別に分岐
		switch (mode)
		{
			case Mode.Stop:
				if (Global.CheckPressKey(0,	Key.menu))
				{
					mode = Mode.Game;
					//isMenu = false;
					//Global.isPause = false;
					Time.timeScale = 1;
					//Global.plc[0].message.SetTextStart("解除します#key#end");
				}
				break;
			case Mode.GameStart:
				Global.isGame =	true;
				mode = Mode.Game;
				break;

			case Mode.Game:
				if(Global.isOver){ OverStart();	}

				// 一時停止
				if (!Global.isPause	&& Global.CheckPressKey(0, Key.menu))
				{
					//mode = Mode.Stop;
					//isMenu = true;
					Global.plc[0].message.SetTextStart("#一時停止#key#end");
					return;
				}
				if (door.CheckTimeOver())
				{
					OverStart();
					mode = Mode.Over;
				}
#if	UNITY_EDITOR
				// やり直し
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
#endif
				break;
			// ゲームオーバー
			case Mode.Over:
				Over();
				break;
			// クリア
			case Mode.Clear:
				Clear();
				break;
		}
	}


	//*** ゲームオーバー	******************************
	void OverStart()
	{
		this.GetComponent<ScreenShot>().StartCapture();

		GameObject.Find("TimerText").SetActive(false);
		PlaySound(SE.ID.System, SE.OVER);
		mode = Mode.Stop;
		Global.fade.HalfOut(OverWait);
		overSp.enabled = true;
		overSp.transform.position =	new	Vector3(0, OverClearSpYpos,	0);

		// コンティニューメッセージは OverWait	後に表示
		_ =	Global.WaitForAsync(OverWait, () =>
		{
			continueText.text =	yesnoText[0];
			continueText.enabled = true;
			mode = Mode.Over;
		});
	}

	void Over()
	{
		// はい・いいえの選択
		for	(int id	= 1; id	<= Define.maxPlayer; id++)
		{
			if (Global.CheckPressKey(id, Key.up))
			{
				selectContinue--;
				if (selectContinue < 0)	{ selectContinue = 0; }
				else { PlaySound(SE.ID.System, SE.SELECT); }
			}
			else if	(Global.CheckPressKey(id, Key.down))
			{
				selectContinue++;
				if (selectContinue > 1)	{ selectContinue = 1; }
				else { PlaySound(SE.ID.System, SE.SELECT); }
			}
			// 決定
			if (Global.CheckPressKey(id, Key.ok))
			{
				PlaySound(SE.ID.System, SE.OK);
				continueText.enabled = false;

				// はい -> コンティニュー
				if (selectContinue == 0)
				{
					Global.fade.Out(0.5f);
					_ =	Global.WaitForAsync(1.0f, () =>
					{
						ReStart();
					});
				}
				// いいえ ->	タイトルへ
				else
				{
					Global.fade.SetNextScene("Title");
					Global.fade.Out(1.0f);
				}
			}
			continueText.text =	yesnoText[selectContinue];
		}
	}

	//*** クリア	**************************************
	void ClearStart()
	{
		mode = Mode.Clear;
		door.Pause();											// ドアの動作を停止する
		delayTime =	1;
		TimerCounter_.Off();
		_ =	Global.WaitForAsync(delayTime, () =>
		{
			door.Close();
			clearSp.enabled	= true;
			clearSp.transform.position = new Vector3(0,	OverClearSpYpos, 0);
			Global.fade.Out(1.0f);
		});
	}
	void Clear()
	{
		Global.numStage++;
		if (Global.numStage	>= Define.maxStage)	{ Global.numStage =	Define.maxStage;}
		Global.LoadStage(Global.numStage);
	}

	// 脱出したプレイヤー数をカウントアップ
	public void	IncExitPlayer()
	{
		numExitPlayer++;
		if(numExitPlayer >=	Define.maxPlayer)
		{
			ClearStart();
			mode = Mode.Clear;
		}
	}
}


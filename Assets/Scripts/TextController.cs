using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Common;


public class TextController	: _Base
{

	readonly float xMargin	 = 2f;								// 横の余白
	readonly float yMargin	 = 1f;								// 縦の余白
	readonly float xOffset	 = 1.6f;							// プレイヤーからの距離
	readonly float yOffset	 = 0.5f;
	readonly float moveSpeed = 0.1f;							// ウィンドウの移動速度


	[SerializeField]
	GameObject playerGameObject;								// プレイヤーゲームオブジェクト
	[SerializeField]
	Camera cam;
	[SerializeField]
	RectTransform rectCanvas;
	[SerializeField]
	RectTransform rectMessage;
	[SerializeField]
	SpriteRenderer messageWindow, cursor;

	PlayerController pl;										// プレイヤーコントローラー
	float			 waitTime;
	string			 str;
	bool			 isPlay;
	bool			 isEnd;										// 終了待ち
	bool			 isKey;										// キー待ち
	TextMeshProUGUI	 txtp;
	int				 textCount;
	Vector3			 windowPos;									// 表示位置,前回の座標
	Vector3			 cursorPos;									// カーソル	Y 位置
	float			 cursorCounter;								// カーソルアニメーション用
	float			 delay;


	void Start()
	{
		txtp = GetComponent<TextMeshProUGUI>();
		pl	 = playerGameObject.GetComponent<PlayerController>();

		// カーソル位置の Y 座標を得る(初期位置となる)
		cursorPos =	cursor.gameObject.transform.localPosition;

		Reset();
	}

	// リセット
	void Reset()
	{
		txtp.text =	"";
		isPlay	  =	false;
		isKey	  =	false;

		// メッセージウィンドウ・カーソルは表示オフから始める
		messageWindow.enabled =	false;
		cursor.enabled = false;
	}

	void Update()
	{
		if(!messageWindow.enabled){	return;	}

		if(delay > 0){ delay -=	Time.unscaledDeltaTime;	return;	}	// 遅延時間

		UpdateMessageWindowPosition();							// メッセージウィンドウの表示位置を変える
		AnalyzeString();										// メッセージ解析&実行
		UpdateCursor();											// カーソル更新
	}

	// 一文字ずつ文字表示
	void AnalyzeString()
	{
		if(!isPlay){ return;}
		if(isKey){ return;}

		if (isEnd)												 //	終了処理()
		{
			PlaySound(SE.ID.Player, SE.PLY_WINDOW_CLOSE);
			isEnd =	false;
			isPlay = false;
			waitTime = 0;
			str	= "";
			txtp.text =	"";
			textCount =	0;
			messageWindow.enabled =	false;
			cursor.enabled = false;
			Global.isPause = false;
			Global.isMessage = false;
			Time.timeScale = 1;
			return;
		}

		// コマンド
		if (str.IndexOf("#key")	== textCount)
		{
			cursor.enabled = true;
			textCount += 4;
			isKey =	true;
		}
		else if	(str.IndexOf("#end") ==	textCount)
		{
			delay =	0.5f;										// すぐに終わらないように遅延時間を設ける
			isEnd =	true;
		}
		else
		{
			// 一文字ずつ表示
			waitTime +=	Time.unscaledDeltaTime;					// 一時停止でも動作する
			if (waitTime > 0.1f)
			{
				waitTime -=	0.1f;
				txtp.text += str[textCount];
				textCount++;
				isPlay = textCount != str.Length;
			}
		}
	}

	// メッセージウィンドウの表示位置を得る
	Vector3	GetWindowPosition()
	{
		Vector3	target = new Vector3(playerGameObject.transform.position.x,	playerGameObject.transform.position.y, 0);

		float x = (float)((int)((target.x + Define.canvasSize.x / 2) / Define.canvasSize.x)) * Define.canvasSize.x;
		float y = (float)((int)((target.y - Define.canvasSize.y / 2) / Define.canvasSize.y)) * Define.canvasSize.y;
		float xx = (target.x - x) + xOffset * pl.dir;
		float yy = (target.y - y) + yOffset;
		if (     xx < -Define.canvasSize.x / 2 + xMargin) { xx = -Define.canvasSize.x / 2 + xMargin; }
		else if (xx >  Define.canvasSize.x / 2 - xMargin) { xx =  Define.canvasSize.x / 2 - xMargin; }
		if (     yy < -Define.canvasSize.y / 2 + yMargin) { yy = -Define.canvasSize.y / 2 + yMargin; }
		else if (yy >  Define.canvasSize.y / 2 - yMargin) { yy =  Define.canvasSize.y / 2 - yMargin; }
		target.x = x + xx ;
		target.y = y + yy;

		return target;
	}

	// 拡大率設定
	public void SetScale(float scale)
	{
		gameObject.transform.localScale = new Vector3(0.5f *  scale, 0.5f * scale, 1);
	}


	// メッセージウィンドウの表示位置を設定する
	void SetWindowPosition(Vector3 pos)
	{
		// 座標を設定(Rect	から座標を計算)
		Vector2	newPos = Vector2.zero;
		Vector3	screenPos =	RectTransformUtility.WorldToScreenPoint(cam, pos);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas,	screenPos, cam,	out	newPos);
		rectMessage.localPosition =	newPos;
	}

	// メッセージウィンドウの表示位置を変える
	void UpdateMessageWindowPosition()
	{
		// プレイヤー位置よりメッセージウィンドウの表示座標を求める
		Vector3	target = GetWindowPosition();

		// メッセージウィンドウを動かす(x は少しずつ y は計算した座標をそのまま渡す)
		float speed	= Mathf.Abs(target.x - windowPos.x);
		if (speed >	moveSpeed) { speed = moveSpeed;	}
		windowPos.x	+= speed * pl.dir;
		if(pl.dir == -1)
		{
			if(windowPos.x < target.x){ windowPos.x = target.x; }
		}
		else
		{
			if (windowPos.x > target.x) { windowPos.x = target.x; }
		}
		windowPos.y	= target.y;
		

		//windowPos = target;

		// メッセージウィンドウの座標を設定
		SetWindowPosition(windowPos);

	}

	// キー待ちマークを上下に動かす
	void UpdateCursor()
	{
		if(!isKey){	return;	}

		// ok ボタンチェック
		if(Global.CheckPressKey(pl.id, Key.ok))	
		{
			PlaySound(SE.ID.Player, SE.HIT_KEY);
			isKey =	false;
			return;
		}

		// キー待ちマークを上下
		cursorCounter += Time.unscaledDeltaTime;
		float y	= cursorCounter	< 0.5f ? 0 : 0.05f;
		if(cursorCounter >=	1f){ cursorCounter -= 1f;}
		cursor.gameObject.transform.localPosition =	new	Vector3(cursorPos.x	, cursorPos.y -	y, cursorPos.z);
	}

	// 文字列セット
	public void	SetText(string p)
	{
		str	= p;
		isPlay = false;
	}

	// 開始
	public void	StartText()
	{
		// メッセージウィンドウの表示位置
		windowPos =	new	Vector3(playerGameObject.transform.position.x, playerGameObject.transform.position.y, 0);
		isPlay = true;
		messageWindow.enabled =	true;
		waitTime = 0;
		textCount =	0;
		Time.timeScale = 0;
		Global.isMessage = true;

		// プレイヤー位置よりメッセージウィンドウの表示座標を求めて設定
		SetWindowPosition(windowPos	= GetWindowPosition());
	}


	private	const char SEPARATE_COMMAND	= '!';
	string[] cmdQueue;

	public void	SetTextStart(string	p)
	{
		cmdQueue = p.Split('#');

		str	= p;
		txtp.text =	"";
		StartText();
		Global.isPause = true;
	}

	public void	StopText()
	{
		isPlay = false;
	}
}

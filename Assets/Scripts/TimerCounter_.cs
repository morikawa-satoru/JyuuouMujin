using UnityEngine;
using TMPro;
using DG.Tweening;
using Common;


public class TimerCounter_ : MonoBehaviour
{
	readonly float		  yPos		= 9.44f;					// 制限時間表示座標
	readonly float		  moveTime	= 1.0f;						// 画面中央からの移動時間
	readonly static	float blinkTime	= 1.0f;						// 点滅時間
	readonly static	float fadeTime	= 0.5f;						// フェードイン時間

	public enum	Mode { Stop, Blink,	Main };						// 動作モード

	static	TextMeshPro	timerText;								// テキスト
	static	float		counter, blinkCounter;					// タイマーのカウンター、点滅カウンター
	static	Mode		mode;                                   // モード


	void Awake()
	{
		Init();
	}

	static void	Init()
	{
		mode	= Mode.Stop;
		counter	= 0;
	}

	void Update()
	{
		switch(mode)
		{
		case Mode.Stop:
			break;
		case Mode.Blink:
			Blink();
			break;
		default:
				if (counter	> 0)
				{
					counter	-= Time.deltaTime;
					if (counter	<= 0)
					{
						counter	= 0;
						mode = Mode.Stop;
						Global.isOver =	true;
					}
					ScoreUpdate();
				}
			break;
		}

	}

	static void	ScoreUpdate()
	{
		// 制限時間がない場合は表示を変える
		timerText.text = string.Format("{0,5:f2}", counter)	+ Define.normalMessage;
	}

	// 点滅
	void Blink()
	{
		timerText.enabled =	Global.GetBlink(blinkCounter);

		blinkCounter -=	Time.deltaTime;
		if (blinkCounter <=	0)
		{
			mode = Mode.Main;
			timerText.transform.DOMove(new Vector3(timerText.transform.localPosition.x,	yPos, timerText.transform.localPosition.z),	moveTime).SetEase(Ease.InSine);
		}
	}

	// 引数 :	制限時間時間、遅延時間
	public static void Start(float time_, float	delay_)
	{
		if (timerText == null){ Init();}

		counter	= time_;
		// n 秒までに脱出せよ
		if (time_ >	0)
		{
			timerText.enabled =	true;
			timerText.transform.localPosition =	new	Vector3(timerText.transform.localPosition.x, 0,	timerText.transform.localPosition.z);
			timerText.DOFade(1.0f, fadeTime).SetDelay(delay_);
			_ =	Global.WaitForAsync(fadeTime + delay_, () => mode =	Mode.Blink);
			ScoreUpdate();
			blinkCounter = blinkTime;
		}
		// 次の部屋に行け
		else if	(time_ == 0)
		{
			timerText.enabled =	true;
			timerText.transform.localPosition =	new	Vector3(timerText.transform.localPosition.x, 0,	timerText.transform.localPosition.z);
			timerText.DOFade(1.0f, fadeTime).SetDelay(delay_);
			_ =	Global.WaitForAsync(fadeTime + delay_, () => mode =	Mode.Blink);
			timerText.text = Define.nextRoomMessage;
			blinkCounter   = blinkTime;
		}
		// 表示なし
		else if	(time_ == 0)
		{
			timerText.enabled =	false;
		}
		mode = Mode.Stop;
		Global.isOver =	false;
	}

	public static void Off()
	{
		mode = Mode.Stop;
		timerText.enabled =	false;
	}
}


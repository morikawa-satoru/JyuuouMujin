using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using DG.Tweening;
using TMPro;


public class DoorController	: MonoBehaviour
{

	readonly float OpenTime		  =	5.0f;						// ドアが開いている時間
	readonly float CloseAfterTime =	1.0f;						// ドアが開いている時間

	[SerializeField]
	TextMeshPro	LeftTime;										// 残り時間

	Animator an;
	bool	 isOpen;											// true	: ドアが開いている
	bool	 isTimeOver;										// true	: タイムオーバー
	bool	 isPause;											// true	: 一時停止
	bool	 isExitPossible;									// true	: 脱出可能
	float	 RemainingTime;


	void Start()
	{
		an = GetComponent<Animator>();
		ReStart();
		isPause	= false;
		isExitPossible = true;
	}

	public void	ReStart()
	{
		an.Play("Stop");
		isOpen = false;
		isTimeOver = false;
		LeftTime.enabled = false;
	}

	void Update()
	{
		if (!isOpen) { return;}
		if (Global.isOver) { return; }
		if (isPause) { return; }

		RemainingTime -= Time.deltaTime;
		if(RemainingTime <=	0)
		{
			RemainingTime =	0;
			isOpen		  =	false;
			isExitPossible = false;
			an.Play("Close");
			AudioManager.Instance.PlaySE(SE.ID.Player, SE.DOOR_CLOSE);
			_ =	Global.WaitForAsync(CloseAfterTime,	() => isTimeOver = true);
		}
		LeftTime.text =	string.Format("{0,5:f2}", RemainingTime) + Define.LeftTimeText;
	}

	// ドアを開く
	public void	Open()
	{
		AudioManager.Instance.PlaySE(SE.ID.Player, SE.DOOR_OPEN);
		an.Play("Open");
		RemainingTime =	OpenTime;
		LeftTime.enabled = true;
		isOpen = true;
	}

	// ドア閉じる
	public void	Close()
	{
		AudioManager.Instance.PlaySE(SE.ID.Player, SE.DOOR_CLOSE);
		an.Play("Close");
		isOpen = false;
		isExitPossible = false;
	}

	public bool	CheckTimeOver()
	{
		return isTimeOver;
	}

	// 脱出可能か調べる
	public bool	CheckExitPossible()
	{
		return isExitPossible;
	}

	public void	Pause()
	{
		isPause	= !isPause;
	}
}


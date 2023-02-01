using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


// NPC モブ
public class MobMen1 : _FriendBase
{

	public bool	isGround { set;	get; } = false;					// 地面

	readonly float turnWait	= 0.5f;								// 振り返り時のウエイト
	//readonly float[] SponeXSpeed = { -90, 0, 90	};				// 胞子の飛ぶ方向
	//readonly float	 SponeYSpeed = 200;
	//readonly float offsetY = 0.85f;								// Y ずらし値(弾発射時)
	//readonly float DistanceToOutsideOfRoom = 3f;				// 部屋の外までの距離(押し返しの距離)
	readonly float cageStartWaitTime = 2;						// 檻から登場時の待ち時間

	RadarController	wallRadar, attackRadar,	groundRadar;
	float wait;
	int	  xRoom, yRoom;


	public override void Initialize()
	{
		wallRadar	= wallRadarObj.GetComponent<RadarController>();
		attackRadar	= attackRadarObj.GetComponent<RadarController>();
		groundRadar	= groundRadarObj.GetComponent<RadarController>();
		SetDir();

		// 投げられて開始
		if(isThrowStart)
		{
			isThrowStart = false;
			SetMode(Mode.SitStart);
		}

		// 自分のいるルームを得る
		xRoom =	(int)( (transform.position.x + Define.xRoomSize	/ 2f) /	Define.xRoomSize);
		yRoom =	(int)(-(transform.position.y - Define.yRoomSize	/ 2f) /	Define.yRoomSize);
	}


	void Update()
	{
		UpdateSub();

		// 一時停止
		if (Global.isPauseStart) { Pause(true);	}
		if (Global.isPauseEnd) { Pause(false); }
		if (Global.isPause)	{ return; }							// 一時停止か?
		if (isDead)	{ return; }									// 死んでいるか?
		if (isDontMove)	{ return; }								// 動かないか?
		
		if (wait > 0){ wait	-= Time.deltaTime; return;}

		// このルームの座標(y は正の値になる)
		float xx =	(transform.position.x +	Define.xRoomSize / 2f) - xRoom * Define.xRoomSize;
		float yy = -(transform.position.y -	Define.yRoomSize / 2f) - yRoom * Define.yRoomSize;

		switch (mode)
		{
		case Mode.StartFromCage:
			break;

		case Mode.Move:
			PlayAnim(Define.animWalk);
			// 壁・床がない・部屋の端ならターンする
			if (wallRadar.isHit	|| !groundRadar.isWall)
			//|| (xx < DistanceToOutsideOfRoom &&	isLeft)	|| (xx > (Define.xRoomSize - DistanceToOutsideOfRoom) && !isLeft))
			{
				an.Play(Define.animIdle);
				wait = turnWait;
				SetMode(Mode.Turn);
			}
			else
			{
				x += moveSpeed * Time.deltaTime;
			}
			break;
		case Mode.Turn:
			// 壁に当たったので向きを変える
			wait = turnWait;
			isLeft = !isLeft;
			SetDir();
			SetMode(Mode.Move);
			break;
		case Mode.SitStart:
			wait = 3f;	// この時間座っている
			PlayAnim(Define.animSit);
//			SetMode(Mode.Sit);
			SetMode(Mode.Move);
			break;
		case Mode.Sit:
			SetMode(Mode.Move);
			break;
		}
	}

	// 檻から登場時の初期化
	public override void StartFromCageInitialize()
	{
		PlayAnim(Define.animIdle);
		SetMode(Mode.Move);
		wait = cageStartWaitTime;								// 檻があくまで待つ
	}
}

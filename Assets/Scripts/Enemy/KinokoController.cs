using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class KinokoController :	_EnemyBase
{

	[SerializeField]
	string bulletName;											// 弾
	[SerializeField]
	float moveSpeedDefault = 1f;								// 移動速度

	readonly float　 turnWait	= 0.5f;							// 振り返り時のウエイト
	readonly float[] SponeXSpeed = { -90, 0, 90	};				// 胞子の飛ぶ方向
	readonly float	 SponeYSpeed = 200;
	readonly float offsetY = 0.85f;								// Y ずらし値(弾発射時)

	float wait;


	// 初期化
	public override void Initialize()
	{
		wallRadar	= wallRadarObj.GetComponent<RadarController>();
		playerRadar	= playerRadarObj.GetComponent<RadarController>();
		groundRadar	= groundRadarObj.GetComponent<RadarController>();
		SetDir();
		onGround = false;
		onCatchObj = false;
		isDead   = false;
		mode = Mode.Move;
		// 自分のいるルームを得る
		xRoom =	(int)( (transform.position.x + Define.xRoomSize	/ 2f) /	Define.xRoomSize);
		yRoom =	(int)(-(transform.position.y - Define.yRoomSize	/ 2f) /	Define.yRoomSize);
		an.Play(Define.animIdle);
	}

	// 檻から登場時の初期化
	public override void StartFromCageInitialize()
	{
		targetObject = GetNearPlayer();							// 最寄りのプレイヤーを得る
		Vector3 playerPos = targetObject.transform.position;
		isLeft = playerPos.x < x;								// 向きを設定
		SetDir();
		// 自分のいるルームを得る
		xRoom =	(int)( (transform.position.x + Define.xRoomSize	/ 2f) /	Define.xRoomSize);
		yRoom =	(int)(-(transform.position.y - Define.yRoomSize	/ 2f) /	Define.yRoomSize);
		an.Play(Define.animIdle);
	}

	void Update()
	{
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
		case Mode.Attack:
			break;
		case Mode.Move:
			if (playerRadar.isPlayer)
			{
				an.Play(Define.animAttack);
				mode = Mode.Attack;
				break;
			}
			an.Play(Define.animWalk);
			// 壁・床がない・部屋の端ならターンする
			if (wallRadar.isHit	|| !groundRadar.isWall 
			|| (xx < DistanceToOutsideOfRoom &&	isLeft)	|| (xx > (Define.xRoomSize - DistanceToOutsideOfRoom) && !isLeft))
			{
				an.Play(Define.animIdle);
				wait = turnWait;
				mode = Mode.Turn;
			}
			else
			{
				x += moveSpeed * Time.deltaTime;
			}
			break;
		case Mode.Turn:
			// 壁に当たったので向きを変える
			wait   = turnWait;
			isLeft = !isLeft;
			SetDir();
			mode = Mode.Move;
			break;
		}
	}

	// 向きを設定する
	void SetDir()
	{
		if(isLeft)
		{	// 左
			moveSpeed =	-moveSpeedDefault;
			sp.flipX  =	false;
			wallRadarObj.transform.localScale =	new	Vector2(1, 1);
			playerRadarObj.transform.localScale	= new Vector2(1, 1);
			groundRadarObj.transform.localScale	= new Vector2(1, 1);
		}
		else
		{	// 右
			moveSpeed =	moveSpeedDefault;
			sp.flipX  =	true;
			wallRadarObj.transform.localScale =	new	Vector2(-1,	1);
			playerRadarObj.transform.localScale	= new Vector2(-1, 1);
			groundRadarObj.transform.localScale	= new Vector2(-1, 1);
		}
	}

	// 弾を飛ばす
	public void	AttackEnd()
	{
	
    	// クローンをコピーする。バラバラになるオブジェクトは読み込みでは動作しないため
		GameObject obj = (GameObject)Resources.Load(Define.prefabEnemyBullet);
        //GameObject obj = GameObject.Find(bulletName + "_Clone");

		Vector3	v =	new	Vector3(x, y + offsetY,	0);
		for(int	i =	0; i < 3; i++)
		{
			GameObject go =	Instantiate(obj, v,	transform.rotation);
			Rigidbody2D	rb = go.GetComponent<Rigidbody2D>();
			rb.AddForce(new	Vector2(SponeXSpeed[i],	SponeYSpeed));
		}
		PlaySound(SE.ID.Enemy, SE.ENE_SHOT_KINOKO);
		wait = turnWait;
		mode = Mode.Turn;
	}
}

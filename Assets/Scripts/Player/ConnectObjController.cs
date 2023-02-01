using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


//
// コネクトオブジェ(プレイヤーのロープの先のオブジェで接続されている状態の時)
//	※ロープの先には、1:何もない、2:何らかのオブジェ、3:キャラクターが付く(未実装)
//
public class ConnectObjController :	_CharaBase
{

	[SerializeField]
	GameObject player;

	[SerializeField]
	float DistanceToFeet_ = 0.257f;								// 地面まで距離(オブジェによって違う)


	readonly float attackCoolTime	 = 0.1f;					// 攻撃時の硬直時間
	readonly float attackOffPower	 = 10;						// これ以下の時は攻撃オフのチェックをする
	readonly int   attackOffCountMax = 3;						// attackOffPowerがこの数続いたとき攻撃オフ
	//readonly float DistanceToFeet_	 = 0.16f;					// 地面までの距離




	public PlayerController	pl { set; get; }					// 接続先のプレイヤー
	public float dist { set; get; }								// プレイヤーとコネクトオブジェの距離


	int slowSpeedCounter = 0;									// 低速のカウンター



	public override void Initialize()
	{
		onGround   = false;
		onCatchObj = false;
		isDead     = false;

		pl				 = player.GetComponent<PlayerController>();
		isAttack		 = false;
		slowSpeedCounter = 0;
		dist			 = 0;

		Restart();
	}

	public override	void Restart()
	{
		InitCheckGround();
	}

	void Update()
	{
		if (Global.isPauseStart) { Pause(true);	}
		if (Global.isPauseEnd) { Pause(false); }
		if (Global.isPause)	{ return; }							// 一時停止か?

//@@
		if(pl.isConnectObjHold)
		{
			onGround = false;
		}
		else
		{
			CheckGround(0.1f, DistanceToFeet_);					// 足元の状態を得る
		}

//@@Dbg.Print("onGround:" + onGround);


		// 硬直時間(ここではリターンしない)
		if(coolTime > 0){ coolTime -= Time.deltaTime;}

		// スプライトの向きをプレイヤーと合わせる
		if(pl.isConnectObjFlip){ sp.flipX = pl.sprite[0].flipX;	}
		else{					 sp.flipX = false;}

		// プレイヤーとの距離
		dist = Vector2.Distance(transform.position, player.transform.position);

		// 離れすぎた場合はプレイヤーを動かさない
		bool dontMove = false;
		if (dist > Define.ropeMaxLength2)
		{
			pl.SetPrevPos();
			dontMove = true;
		}
		// 飛びすぎ制限(※ロープ切れ対策)
		else if (dist > Define.ropeMaxLength)
		{
			// 鉄球がプレイヤーの左側にあって右に動こうとしている時	  鉄球がプレイヤーの右側にあって左に動こうとしている時
			if((x < pl.transform.position.x && pl.moveSpeedX > 0) || (x > pl.transform.position.x && pl.moveSpeedX < 0))
			{
				dontMove = true;
			}
			// ハシゴの場合はプレイヤーの動きを停める
			else if(pl.onLadder)
			{
				pl.SetPrevPos();
			}
		}
		if (dontMove)
		{	// 飛びすぎ対策としてコネクトオブジェのベロシティをクリアする(攻撃時以外)
			rb.velocity	= new Vector2(0, rb.velocity.y);
		}

		// ペロシティが小さいものは攻撃フラグをオフにする
		if(isAttack && coolTime <= 0)
		{
			if(rb.velocity.magnitude < attackOffPower)
			{
				slowSpeedCounter++;
				if(slowSpeedCounter >= attackOffCountMax)
				{
					slowSpeedCounter = 0;
					isAttack = false;
				}
			}
			else
			{
				slowSpeedCounter = 0;
			}
		}
	}

	// 攻撃
	public override void Attack()
	{
		isAttack  =	true;
		coolTime  = attackCoolTime;
		slowSpeedCounter = 0;
		PlayAnim(Define.animConnect_Attack);					// コネクトプラグを攻撃モーションへ
	}

	public void	Reset()
	{
		isAttack = false;
		PlayAnim(Define.animConnect_Idle);
	}

	public void	Damage()
	{
		pl.ConnectObjClash();
	}

	// コネクトオブジェとプレイヤーの距離からジャンプ可能かを調べる
	// ジャンプ可能なら true
	public bool CheckJump()
	{
		// とりあえずロープの長さ1.0倍とする
		return  (dist < Define.ropeMaxLength * 1.0f);
	}

	void OnCollisionEnter2D(Collision2D	col)
	{
		if (!pl.isConnectObj){ return; }						// コネクトオブジェがない場合はなにもしない

		if (col.gameObject.tag == Define.tagWall 
		||	col.gameObject.tag == Define.tagCatchObj
		||	col.gameObject.tag == Define.tagEnemy )
		{
			if(isAttack)
			{
				Reset();
				PlaySound(SE.ID.ConnectPlug, SE.OBJ_HIT);
				rb.velocity	= Vector2.zero;
				Global.effect.Start(Eff.Spark, col.contacts[0].point);
				isAttack = false;
			}
			else
			{
				// 地面
				PlaySound(SE.ID.ConnectPlug, SE.OBJ_HIT_GROUND);
				Global.effect.Start(Eff.Smoke, col.contacts[0].point);
				rb.velocity	= Vector2.zero;
			}
		}
	}
}

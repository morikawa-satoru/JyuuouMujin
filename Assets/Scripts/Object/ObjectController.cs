using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using DG.Tweening;


public class ObjectController :	_CharaBase
{

	Explodable explodable;										// 破片


	// 初期化
	public override	void Initialize()
	{
		if (maxHp != 0 && Hp <=	maxHp /	2)						// 最初から壊れている場合
		{
			PlayAnim(Define.animDamage);
		}
	}

	// 更新
	public override	void UpdateSub()
	{
		if(attakTime > 0)
		{
			attakTime -= Time.deltaTime;
			isAttack = attakTime > 0;
		}
	}

	// ダメージ開始
	public override bool StartDamage() // true : 壊れた
	{
		// HP が 0	のものは壊れない
		if (Hp == 0)
		{
			PlaySound(SE.ID.Object, se_damage);
			return false;
		}
		// HP 減らす。0	ならバラバラになる
		Hp--;
		if (Hp == 0)
		{
			// 何かをぶつけられバラバラになる
			explodable = GetComponent<Explodable>();			// ここでやらないと取得できない
			if(explodable != null)
			{
				explodable.explode();
			}
			Dead();
			return true;
		}
		else if	(Hp	<= maxHp / 2)
		{
			PlayAnim(Define.animDamage);
		}
		PlaySound(SE.ID.Object, se_damage); 
		coolTime = coolTime_;
		return false;
	}

	// 即時にバラバラにする
	public void	IimmediateClash()
	{
		PlaySound(SE.ID.Object, se_dead);
		explodable = GetComponent<Explodable>();				// ここでやらないと取得できない
		if(explodable != null){ explodable.explode();}
	}

	public void	Reset()
	{
		isAttack = false;
		attakTime =	0;
	}

	// 衝突判定
	void OnCollisionEnter2D(Collision2D	other)
	{
		if (!Global.isGame)	{ return; }
		if (coolTime > 0) {	return;	}
	
		bool bEffect = false;

		// 壁床
		if (other.gameObject.tag ==	Define.tagWall)
		{
			bEffect	= true;
			coolTime = coolTime_;
		}

		// キャッチオブジェ(ロープを切り離して投げたキャッチオブジェとの判定)
		if (other.gameObject.tag ==	Define.tagCatchObj)
		{
			// このオブジェクトは攻撃中か?(このゲームオブジェクトを投げた時など)
			if (isAttack)
			{	// ぶつけられた方
				_CharaBase oc = other.gameObject.GetComponent<_CharaBase>();
				oc.StartDamage();								// ぶつけた方のダメージ

				// ぶつけた方(このオブジェクト)
				StartDamage();
				coolTime = coolTime_;
				bEffect	= true;
			}
		}

		// コネクトオブジェ(ロープに繋がれているオブジェ)
		else if (other.gameObject.tag == Define.tagConnectObj)
		{
			if (Global.plc[0].connectPlugObj.isAttack)
			{	// ぶつけた方にダメージ
				Global.plc[0].ConnectObjClash();

				// ぶつけられた方(このオブジェクト)
				StartDamage();
				Reset();										// ロープに繋がっている場合は一度ぶつかったら攻撃状態をやめる
				coolTime = coolTime_;
				bEffect	= true;
			}
		}

		// エフェクト
		if (bEffect)//&& rb.velocity.magnitude > 2)
		{
			//float r	= Vector3.Angle(rb.velocity, Vector3.up);

			// 50～130の角で1.2以上速度が出ていたら攻撃とみなす
			//if (rb.velocity.magnitude >	Define.thresholdSpeed && (r	>= 90 -	60 && r	<= 90 +	60))
			if(rb.velocity.magnitude > Define.thresholdSpeed && isAttack)
			{
				PlaySound(SE.ID.Object2, SE.OBJ_HIT);
				Global.effect.Start(Eff.Spark, other.contacts[0].point);
				isAttack = false;
			}
			else
			{
				PlaySound(SE.ID.Object2, SE.OBJ_HIT_GROUND);
				Global.effect.Start(Eff.Smoke, other.contacts[0].point);
			}
			coolTime = coolTime_;
		}
	}

	// 死亡 & 消去
	void Dead()
	{
		PlaySound(SE.ID.Object, se_dead);

		// マザーと子オブジェクトをコピーしてバラバラにする。バラバラにならないものは母体も飛ばす
		Global.DisassemblyGameObject(gameObject, explodable == null);
	}

	// 檻から登場
	public override void StartFromCage()
	{
		// アルファイン
        SetAlpha(sp, 0);
        sp.DOFade(1, cageStartTime).OnComplete(() =>
		{
			// 動かす
			isDontMove = false;
			sp.sortingOrder = spriteOrder;
		});
		rb.bodyType = RigidbodyType2D.Dynamic;
        sp.sortingOrder = spriteOrder; 

		// 子もアルファイン
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
			SpriteRenderer childSp = g.GetComponent<SpriteRenderer>();
			Rigidbody2D childRb = g.GetComponent<Rigidbody2D>();
			if(childSp)
			{
				SetAlpha(childSp, 0);
				childSp.DOFade(1, cageStartTime).OnComplete(() =>
				{
					childSp.sortingOrder = spriteOrder;
				});
			}
			if(childRb){ childRb.bodyType = RigidbodyType2D.Dynamic;}
		}
	}
}



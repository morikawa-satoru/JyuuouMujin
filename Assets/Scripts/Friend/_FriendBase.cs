using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common;


public class _FriendBase : _CharaBase
{
	public enum	Mode
	{
		Start,
		StartFromCage,
		Idle,
		Move,
		Turn,
		SitStart,
		Sit,
	};

	[SerializeField]
	public GameObject targetObject;
	[SerializeField]
	public GameObject prefabDeadEffect;								// 死亡時のエフェクト
	[SerializeField]
	public Vector2	deadEffectOffset;								// 死亡時のエフェクトのずらし値
	[SerializeField]
	public float moveSpeedDefault = 1f;								// 移動速度
	[SerializeField]
	public GameObject wallRadarObj, attackRadarObj, groundRadarObj;	// レーダー


	public Mode	prevMode {   set; get; }						// 前回の動作モード
	public Mode	mode {       set; get; } = Mode.Start;			// 動作モード
	public int direction {   set; get; }						// 向き
	public float moveSpeed { set; get; }


	public override void Initialize()
	{
		// 投げられて開始
		if(isThrowStart)
		{
			isThrowStart = false;
			SetMode(Mode.SitStart);
		}
	}
	
	// 更新
	public override	void UpdateSub()
	{
		if(coolTime > 0)
		{
			coolTime -= Time.deltaTime;
		}
		if(attakTime > 0)
		{
			attakTime -= Time.deltaTime;
			isAttack = attakTime > 0;
		}
	}

	// 攻撃
	public override void Attack()
	{
		attakTime =	Define.attackTime;
		isAttack = true;
	}

	// 壊れる
	/*public bool	Damage()			// true : 壊れた
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
	}*/

	// コリジョン
	public virtual void	OnCollisionEnter2D(Collision2D other)
	{
		if (!Global.isGame)	{ return; }
		if (coolTime > 0) {	return;	}
		if (isDamage) {	return;	}

		bool bEffect = false;

		// 壁床
		if (other.gameObject.tag ==	Define.tagWall)
		{
			bEffect	= true;
			coolTime = coolTime_;
		}

		// キャッチオブジェ(ロープを切り離して投げたキャッチオブジェとの判定)
		if (other.gameObject.tag ==	Define.tagConnectObj)
		{
			ConnectObjController co	= other.gameObject.GetComponent<ConnectObjController>();
			if (co.isAttack) { HitConnectObj(other.gameObject);	}
			bEffect	= true;
			coolTime = coolTime_;
			//return;
		}
		OnCollisionEnter2DSub(other);
	
		// キャッチオブジェ(ロープを切り離して投げたキャッチオブジェとの判定)
		if (other.gameObject.tag ==	Define.tagCatchObj)
		{
			// ぶつけた方
			_CharaBase oc = other.gameObject.GetComponent<_CharaBase>();
			oc.StartDamage();									// ぶつけた方のダメージ

			// ぶつけられた方(このオブジェクト)
			coolTime = coolTime_;
			bEffect	= true;
		}
		// コネクトオブジェ(ロープに繋がれているオブジェ)
		else if (other.gameObject.tag == Define.tagConnectObj)
		{
			if (Global.plc[0].connectPlugObj.isAttack)
			{	// ぶつけた方にダメージ
				Global.plc[0].ConnectObjClash();

				coolTime = coolTime_;
				bEffect	= true;
			}
		}
		// エフェクト
		if (bEffect)
		{
			if(rb.velocity.magnitude > Define.thresholdSpeed)
			{
				PlaySound(SE.ID.Object2, SE.OBJ_HIT);
				Global.effect.Start(Eff.Spark, other.contacts[0].point);
			}
			else
			{
				PlaySound(SE.ID.Object2, SE.OBJ_HIT_GROUND);
				Global.effect.Start(Eff.Smoke, other.contacts[0].point);
			}
			coolTime = coolTime_;
		}

		// その他のコリジョン
		OnCollisionEnter2DSub(other);
	}

	// その他のコリジョン
	public virtual void	OnCollisionEnter2DSub(Collision2D other)
	{
	}

	// コネクトオブジェをぶつけられた場合
	public void	HitConnectObj(GameObject go)
	{
		// ダメージアニメ
		PlayAnim(Define.animDamage);

		PlaySound(SE.ID.Enemy, SE.OBJ_HIT);

		// 剛体
		Rigidbody2D	connectObjRb = go.GetComponent<Rigidbody2D>();
		connectObjRb.velocity =	Vector2.zero;

		// コライダーオフ
		//CircleCollider2D cir = GetComponent<CircleCollider2D>();
		//if (cir	!= null) { cir.enabled = false;	}
		//BoxCollider2D box =	GetComponent<BoxCollider2D>();
		//if (box	!= null) { box.enabled = false;	}

		// 動かないようにする
		isDontMove = true;

		// やられて座り込むがしばらくしたら動く
		SetMode(Mode.SitStart);
		
		// 剛体を無効にする
		if(rb != null)
		{
			rb.bodyType	= RigidbodyType2D.Kinematic;
			rb.velocity	= Vector2.zero;
		}

		// 点滅後死亡
		StartCoroutine(DamageBlink(false));
	}

	// ダメージ開始
	public override bool StartDamage() // true : 壊れた
	{
		// HP が 0	のものは壊れない
		if (Hp == 0)
		{
			PlayAnim(Define.animDamage);
			PlaySound(SE.ID.Object, se_damage);
			//return false;
		}
		// HP 減らす。0	ならバラバラになる
		Hp--;
		if (Hp == 0)
		{
			// 何かをぶつけられバラバラになる
			/*explodable = GetComponent<Explodable>();			// ここでやらないと取得できない
			if(explodable != null)
			{
				explodable.explode();
			}*/
			Dead();
			return true;
		}
		/*else if	(Hp	<= maxHp / 2)
		{
			PlayAnim(Define.animDamage);
		}
		SndPlay(SE.ID.Object, se_damage); 
		*/
		coolTime = coolTime_;
		return false;
	}

	// 点滅
	public override	IEnumerator	DamageBlink(bool bDead)
	{
		isDamage = true;
		for	(int i = 0;	i <	3; i++)
		{
			sp.DOColor(new Color(1,	1, 1, 0), damageDuration);
			yield return new WaitForSeconds(0.1f);

			sp.DOColor(new Color(1,	1, 1, 1), damageDuration);
			yield return new WaitForSeconds(0.1f);
		}
		isDamage = false;

		yield return new WaitForSeconds(0.5f);
		
		PlayAnim(Define.animSit);								// 座り

		yield return new WaitForSeconds(0.5f);

		//connectObjPrefabName
		//GameObject obj = GameObject.Find("MobMen01Ball" + "_Clone");
		//GameObject go = Instantiate(obj, transform.position, transform.rotation);

		//Destroy(gameObject);

		//if(bDead){ Dead();}
	}
	
	// 死亡処理
	public void	Dead()
	{
		// ダメージ処理 
		PlaySound(SE.ID.Enemy, se_dead);

		// スプライト
		sp.enabled = false;

		// エフェクト
		Vector3	a =	new	Vector3(x +	deadEffectOffset.x,	y +	deadEffectOffset.y,	-1.0f);	
		Instantiate(prefabDeadEffect, a, Quaternion.identity);

		Kill();
	}

	public void	SetMode(Mode m)
	{
		prevMode = mode;
		mode = m;
	}


	// 向きを設定する
	public virtual void SetDir()
	{
		if(isLeft)
		{	// 左
			moveSpeed =	-moveSpeedDefault;
			sp.flipX  =	true;
			wallRadarObj.transform.localScale =	new	Vector2(-1, 1);
			attackRadarObj.transform.localScale	= new Vector2(-1, 1);
			groundRadarObj.transform.localScale	= new Vector2(-1, 1);
		}
		else
		{	// 右
			moveSpeed =	moveSpeedDefault;
			sp.flipX  =	false;
			wallRadarObj.transform.localScale =	new	Vector2(1, 1);
			attackRadarObj.transform.localScale	= new Vector2(1, 1);
			groundRadarObj.transform.localScale	= new Vector2(1, 1);
		}
	}
}

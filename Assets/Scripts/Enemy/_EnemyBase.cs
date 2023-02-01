using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common;


public class _EnemyBase	: _CharaBase
{
	public enum	Mode
	{
		Start,
		Idle,
		Move,
		Turn,
		Attack,
	};
	readonly public float DistanceToOutsideOfRoom = 3f;			// 部屋の外までの距離(押し返しの距離)


	[SerializeField]
	public GameObject targetObject;
	[SerializeField]
	public GameObject wallRadarObj, playerRadarObj, groundRadarObj;	// レーダー
	[SerializeField]
	GameObject prefabDeadEffect;								// 死亡時のエフェクト
	[SerializeField]
	Vector2	deadEffectOffset;									// 死亡時のエフェクトのずらし値

	public Mode	prevMode { set;	get; }							// 前回の動作モード
	public Mode	mode { set;	get; } = Mode.Start;				// 動作モード
	public int direction { set;	get; }							// 向き
	public int xRoom { set;	get; }								// 部屋
	public int yRoom { set; get; }

	public float moveSpeed { set; get; }						// 移動速度

	public RadarController wallRadar { set; get; }				// レーダー
	public RadarController playerRadar { set; get; }
	public RadarController groundRadar { set; get; }


	// 最寄りのプレイヤーを得る
	public GameObject GetNearPlayer()
	{
		int	ret	= 0;

		if(Global.numPlayer	== 2)
		{
			float a	= (Global.player[0].transform.position - transform.position).magnitude;
			float b	= (Global.player[1].transform.position - transform.position).magnitude;

			if(a < b){ ret = 0;	}
			else{	   ret = 1;	}
		}
		else
		{
			for(int	i =	0; i < 2; i++)
			{
				if(Global.player[i]	!= null){ break;}
			}
		}
		return Global.player[ret];
	}

	// 一定間隔で近くのプレイヤーを探す
	public IEnumerator IntervalSearchPlayer(float interval)
	{
		while(!isDead)
		{
			targetObject = GetNearPlayer();
			yield return new WaitForSeconds(interval);
		}
		yield break;
	}

	// コリジョン
	public virtual void	OnCollisionEnter2D(Collision2D other)
	{
		if (isDamage) {	return;	}

		if (other.gameObject.tag ==	Define.tagConnectObj)
		{
			ConnectObjController co	= other.gameObject.GetComponent<ConnectObjController>();
			if (co.isAttack) { HitConnectObj(other.gameObject);	}
			return;
		}

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
		an.Play(Define.animDamage);

		PlaySound(SE.ID.Enemy, SE.OBJ_HIT);

		// 剛体
		Rigidbody2D	connectObjRb = go.GetComponent<Rigidbody2D>();
		connectObjRb.velocity =	Vector2.zero;

		// コライダーオフ
		CircleCollider2D cir = GetComponent<CircleCollider2D>();
		if (cir	!= null) { cir.enabled = false;	}
		BoxCollider2D box =	GetComponent<BoxCollider2D>();
		if (box	!= null) { box.enabled = false;	}

		// 動かないようにする
		isDontMove = true;
		
		// 剛体を無効にする
		if(rb != null)
		{
			rb.bodyType	= RigidbodyType2D.Kinematic;
			rb.velocity	= Vector2.zero;
		}

		// 点滅後死亡
		StartCoroutine(DamageBlink(true));
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
		
		if(bDead){ Dead();}
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
}

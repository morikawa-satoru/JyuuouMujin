using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;
using DG.Tweening;


public class PlayerThrowController : MonoBehaviour
{

	// コネクトオブジェを投げるときのずらし値(投げるものの中心座標)
	readonly Vector2[] handOffset =
	{
		new	Vector2(-0.0f,	 -0.174f),
		new	Vector2(-0.395f, -0.174f),
		new	Vector2(-0.55f,	 -0.003f),
		new	Vector2(-0.393f, 0.333f),
		new	Vector2(-0.168f, 0.499f),
		new	Vector2( 0.133f, 0.499f),							// ここで攻撃開始とする isAttack を true
		new	Vector2( 0.595f, 0.205f),
	};

	[SerializeField]
	bool isCatchAnim;
	SpriteRenderer sprite;
	PlayerController pl;
	bool isThrow;												// 投げている途中か?


	void Start()
	{
		pl = gameObject.transform.parent.GetComponent<PlayerController>();
		sprite = GetComponent<SpriteRenderer>();
		isThrow = false;
	}

	// アニメーションからの割り込み
	public void	AnimationEvent(int num)
	{
		// 停止	※停止はアニメーションの終端ではないので注意
		if(num == -1)
		{
			pl.isConnectObjHold	= false;
			pl.isThrowMotionEnd	= true;
			return;
		}
		else if(num	== 0)
		{
			isThrow	= false;
		}
		else if(num	< handOffset.Length)
		{
			if(isCatchAnim)
			{
				var	a =	handOffset[num];
				if (sprite.flipX){ a.x *= -1;}
				pl.connectPlug.transform.localPosition = new Vector2(a.x, a.y);
			}
			// 攻撃開始 (投げるタイミングだと1フレーム遅れるのでここでやる)
			if(num ==  handOffset.Length - 1)
			{
				if(pl.isConnectObj)
				{
					pl.connectPlugObj.Attack();						// コネクトプラグを攻撃状態にする
				}
			}
		}
		// 投げる
		else if(num	== handOffset.Length &&	!isThrow)
		{
			// ここから攻撃とする(ここだと遅いので上へ移動)
			/*if(pl.isConnectObj)
			{
				pl.connectPlugObj.Attack();							// コネクトプラグを攻撃状態にする
			}*/
			isThrow	= true;

			// 投げた時にコネクトオブジェのコライダーをオンにする
			pl.ValidConnectObjCollider();

			if (isCatchAnim)
			{
				pl.connectPlugRB.bodyType =	RigidbodyType2D.Dynamic;
				Vector2	dir	= Define.throwDirection;

				if (sprite.flipX){ dir.x *=	-1;	}

				// コネクトオブジェを切り離す
				if(Global.CheckKey(pl.id, Key.disconnect))
				{
					GameObject gc =	pl.ReleaseConnectObj(true);
					gc.GetComponent<_CharaBase>().Attack();		// 攻撃時間設定
					Rigidbody2D	gcRb = gc.GetComponent<Rigidbody2D>();
					gcRb.bodyType = RigidbodyType2D.Dynamic;
					gcRb.AddForce(dir *	Define.ThrowPower);
				}
				// こちらは普通に投げる
				else
				{
					pl.connectPlug.layer = LayerMask.NameToLayer(Define.layerConnectObj);
					pl.connectPlugRB.velocity = Vector2.zero;							// ベロシティをクリアしないと斜め下へ行ってしまう
					pl.connectPlugRB.AddForce(dir *	Define.ThrowPower);
				}
			}
		}
	}
}


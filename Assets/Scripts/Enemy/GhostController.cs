using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

//
//	おばけ
//
public class GhostController : _EnemyBase
{

	[SerializeField]
	Vector2	floatingSpeed =	new	Vector2(1, 1);
	[SerializeField]
	Vector2	floatingScale =	new	Vector2(0, 0);
	[SerializeField]
	float moveSpeed	= 0.05f;									// 移動速度
	[SerializeField]
	float nearDistance = 3f;									// 近づく距離(この距離以上なら近づく)
	[SerializeField]
	float searchInterval = 3f;									// プレイヤーのサーチ間隔


	Vector2	floating;											// 回転値
	Vector2	homePosition;


	void Start()
	{
		StartCoroutine(IntervalSearchPlayer(searchInterval));	// 一定間隔でプレイヤーを探す
		sp = GetComponent<SpriteRenderer>();
		an = GetComponent<Animator>();
		mode = Mode.Idle;
	}

	void Update()
	{
		if (Global.isPauseStart) { Pause(true);	}
		if (Global.isPauseEnd) { Pause(false); }
		if (Global.isPause)	{ return; }							// 一時停止か?
		if (isDead)	{ return; }									// 死んでいるか?
		if (isDontMove)	{ return; }								// 動かないか?

		switch (mode)
		{
		case Mode.Idle:
			SetMode(Mode.Move);
			break;
		case Mode.Move:
			x =	homePosition.x + Mathf.Sin(floating.x) * floatingScale.x;
			y =	homePosition.y + Mathf.Cos(floating.y) * floatingScale.y;
			floating +=	floatingSpeed;
			if(targetObject	!= null)
			{
				LookTarget(targetObject);

				// ターゲットに近づく
				Vector3	posA = homePosition;
				Vector3	posB = targetObject.transform.position;
				if (Vector3.Distance(posA, posB) > nearDistance)
				{
					Vector3	posC = posB	- posA;
					posC.Normalize();
					homePosition = posA	+ posC * moveSpeed;
				}
			}
			break;
		}
	}
}

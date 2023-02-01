using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

//
//	�R�E����
//
public class BatController : _EnemyBase
{

	[SerializeField]
	Vector2	jumpPower =	new	Vector2(-100, -400);				// �W�����v��
	[SerializeField]
	float jumpInterval = 5f;									// �W�����v�Ԋu
	[SerializeField]
	GameObject radarObject;										// ���[�_�[

	float wait;
	RadarController	radar;


	void Start()
	{
		sp = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		an = GetComponent<Animator>();
		radar =	radarObject.GetComponent<RadarController>();
		wait = jumpInterval;
		SetDir();
	}

	void Update()
	{
		if (Global.isPauseStart) { Pause(true);	}
		if (Global.isPauseEnd) { Pause(false); }
		if (Global.isPause)	{ return; }							// �ꎞ��~��?

		if (isDead)	{ return; }									// ����ł��邩?
		if (isDontMove)	{ return; }								// �����Ȃ���?

		switch (mode)
		{
			case Mode.Idle:
				wait -=	Time.deltaTime;
				if (wait < 0)
				{
					rb.AddForce(jumpPower);
					wait = jumpInterval;
					SetMode(Mode.Move);
					an.Play("Move");
				}
				break;
			case Mode.Move:
				break;
		}
	}

	void SetDir()
	{
		if (radar.isWall) {	jumpPower.x	*= -1; }
		sp.flipX = jumpPower.x > 0;
		radarObject.transform.localScale = new Vector2(sp.flipX	? -1 : 1, 1);
	}

	public override	void OnCollisionEnter2DSub(Collision2D other)
	{
		if (other.gameObject.tag ==	Define.tagWall)
		{
			SetDir();
			an.Play("Idle");
			SetMode(Mode.Idle);
			rb.velocity	= Vector2.zero;
		}
	}
}

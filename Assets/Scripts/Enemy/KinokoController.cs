using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class KinokoController :	_EnemyBase
{

	[SerializeField]
	string bulletName;											// �e
	[SerializeField]
	float moveSpeedDefault = 1f;								// �ړ����x

	readonly float�@ turnWait	= 0.5f;							// �U��Ԃ莞�̃E�G�C�g
	readonly float[] SponeXSpeed = { -90, 0, 90	};				// �E�q�̔�ԕ���
	readonly float	 SponeYSpeed = 200;
	readonly float offsetY = 0.85f;								// Y ���炵�l(�e���ˎ�)

	float wait;


	// ������
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
		// �����̂��郋�[���𓾂�
		xRoom =	(int)( (transform.position.x + Define.xRoomSize	/ 2f) /	Define.xRoomSize);
		yRoom =	(int)(-(transform.position.y - Define.yRoomSize	/ 2f) /	Define.yRoomSize);
		an.Play(Define.animIdle);
	}

	// �B����o�ꎞ�̏�����
	public override void StartFromCageInitialize()
	{
		targetObject = GetNearPlayer();							// �Ŋ��̃v���C���[�𓾂�
		Vector3 playerPos = targetObject.transform.position;
		isLeft = playerPos.x < x;								// ������ݒ�
		SetDir();
		// �����̂��郋�[���𓾂�
		xRoom =	(int)( (transform.position.x + Define.xRoomSize	/ 2f) /	Define.xRoomSize);
		yRoom =	(int)(-(transform.position.y - Define.yRoomSize	/ 2f) /	Define.yRoomSize);
		an.Play(Define.animIdle);
	}

	void Update()
	{
		// �ꎞ��~
		if (Global.isPauseStart) { Pause(true);	}
		if (Global.isPauseEnd) { Pause(false); }
		if (Global.isPause)	{ return; }							// �ꎞ��~��?
		if (isDead)	{ return; }									// ����ł��邩?
		if (isDontMove)	{ return; }								// �����Ȃ���?
		
		if (wait > 0){ wait	-= Time.deltaTime; return;}

		// ���̃��[���̍��W(y �͐��̒l�ɂȂ�)
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
			// �ǁE�����Ȃ��E�����̒[�Ȃ�^�[������
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
			// �ǂɓ��������̂Ō�����ς���
			wait   = turnWait;
			isLeft = !isLeft;
			SetDir();
			mode = Mode.Move;
			break;
		}
	}

	// ������ݒ肷��
	void SetDir()
	{
		if(isLeft)
		{	// ��
			moveSpeed =	-moveSpeedDefault;
			sp.flipX  =	false;
			wallRadarObj.transform.localScale =	new	Vector2(1, 1);
			playerRadarObj.transform.localScale	= new Vector2(1, 1);
			groundRadarObj.transform.localScale	= new Vector2(1, 1);
		}
		else
		{	// �E
			moveSpeed =	moveSpeedDefault;
			sp.flipX  =	true;
			wallRadarObj.transform.localScale =	new	Vector2(-1,	1);
			playerRadarObj.transform.localScale	= new Vector2(-1, 1);
			groundRadarObj.transform.localScale	= new Vector2(-1, 1);
		}
	}

	// �e���΂�
	public void	AttackEnd()
	{
	
    	// �N���[�����R�s�[����B�o���o���ɂȂ�I�u�W�F�N�g�͓ǂݍ��݂ł͓��삵�Ȃ�����
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

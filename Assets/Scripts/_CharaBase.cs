using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;


public class _CharaBase	: _Base
{

	public readonly	float damageDuration = 0.1f;				// ダメージ時の変化時間
	public readonly	float coolTime_	= 0.5f;						// 硬直時間
	public readonly float cageStartTime = 1;					// 檻から出てくるときの時間


	[SerializeField]
	public bool	isDontMove = false;								// true	: 動かない
	[SerializeField]
	public bool	isDamage = false;								// true	: ダメージを受けている
	[SerializeField]
	public bool	isDisassemble = false;							// true	: 壊れた時に子オブジェクトがバラバラになる(骨など)
	[SerializeField]
	public bool	isConnectObjFlip = false;						// true	: コネクト時にも反転する
	[SerializeField]
	public TextController message;								// メッセージ表示
	[SerializeField]
    public int spriteOrder = 3;									// 登場後のスプライトのオーダー

	[SerializeField]
	public string prefabName;									// コネクト後の切り離し時のプレハブ名
	[SerializeField]
	public float radius;										// プレイヤー保持時のずらし値(持っているときに地面につかないように)
	[SerializeField]
	public string se_damage, se_dead, se_attack;
	[SerializeField]
	public int Hp =	0, maxHp = 0;								// 0 ==	壊れない
	[SerializeField]
	public bool isLeft;											// 左向きか?
	[SerializeField]
	Transform hitGroundBox;										// 地面判定用の Box


	// 起動
	public bool isThrowStart { set; get; } = false;				// true : プレイヤーに投げられて起動

	// 攻撃関係
	public float attakTime { set; get; } = 0;					// 攻撃時間
	public bool	isAttack { set;	get; } = false;

	// 変数
	public EventController ev {	set; get; }						// 接触中のイベント
	public SpriteRenderer  sp { set;	get; }					// スプライト
	public Rigidbody2D     rb {	set; get; }						// 剛体
	public Animator	       an { set; get; }						// アニメーター
	public bool	           isDead	{ set; get; }				// 死亡
	public Vector2         veloBak	{ set; get; }				// RigidBodyのベロシティバックアップ用
	public float angularVelocityBak { set; get; } 				// RigidBodyのベロシティバックアップ用
	public float animSpeedBak		{ set; get; }				// アニメーション速度バックアップ用
	public float coolTime			{ set; get; }				// 連続衝突防止用


	//
	// 地面の判定
	//
	// フラグ
	public bool	onFloor            { set; get; }				// 床の状態(レイヤーが	Wall のもの)
	public bool	onGround		   { set; get; }				// 地面に乗っているか?
	public bool onCatchObj		   { set; get; }				// キャッチオブジェに乗っているか?
	public bool	onPrevGround       { set; get; }				// 前回の地面の状態、今回の地面の状態	※地面はisFloorとonLadderを合わせたもの
	public bool	isLanding          { set; get; }
	public bool	onLadder           { set; get; }				// 足元がハシゴかのフラグ(Feet は両足)
	public bool	onLadderMid        { set; get; } 				// 足元がハシゴかのフラグ(片足でも)
	public bool	onLadderBottom	   { set; get; }
	public bool	prevonLadderMid    { set; get; }


	// レイヤーマスク
	public int	maskLadder		 { set; get; }
	public int	maskLadderMid	 { set; get; }
	public int	maskLadderBottom { set; get; }
	public int	maskFloor        { set; get; }

		   readonly float threshold	     = 0.05f;				// 地面までの閾値
	public readonly float rayDistance    = 0.05f;				// レイを飛ばして足元を調べる距離

	// ジャンプ時、地面の判定をとるレイヤー
	readonly string[] collisionFloorLayer		 = new string[]	{ "Wall", "CatchObj", "BreakingObj"};
	readonly string[] collisionLadderLayer		 = new string[]	{ "Ladder",	"LadderBottom" };
	readonly string[] collisionLadderMidLayer	 = new string[]	{ "Ladder" };
	readonly string[] collisionLadderBottomLayer = new string[]	{ "LadderBottom" };

	// 一時停止時の Rigidbody の状態保存用(※Clone は値を取っておくことにより Kinect 状態を保つ)
	RigidbodyType2D bodyTypeBak;


	void Start()
	{
		sp = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		an = GetComponent<Animator>();
		onGround   = false;
		onCatchObj = false;
		isDead     = false;
		coolTime   = 0;

		sp.flipX   = isLeft;

		Initialize();
		Restart();
	}

	// オーバーライド用の初期化関数
	public virtual void	Initialize()
	{
		onGround = false;
		onCatchObj = false;
		isDead   = false;
		coolTime = 0;
		SetAlpha(sp, 0);

		bodyTypeBak = rb.bodyType;								// Rigidbody タイプを取っておく
	}

	// リスタート用の Start 関数
	public virtual void	Restart()
	{

	}


	void Update()
	{
		if (Global.isPauseStart) { Pause(true);	}
		if (Global.isPauseEnd) { Pause(false); }
		if (Global.isPause)	{ return; }							// 一時停止か?

		if (coolTime > 0) {	coolTime -=	Time.deltaTime;	}

		UpdateSub();
	}

	// オーバーライド用の Update 関数
	public virtual void	UpdateSub()
	{
	}

	// ターゲットの向きを得る
	public void	LookTarget(GameObject target)
	{
		if(sp)
		{
			sp.flipX = x < target.transform.position.x;
		}
	}

	public void	Kill()
	{
		Destroy(gameObject);
	}

	// 一時停止
	public virtual void	Pause(bool bSw)
	{
		if(!bSw)
		{
			// 再開
			rb.bodyType	= bodyTypeBak;							// body タイプを元に戻す
			rb.gravityScale	= 1;
			rb.velocity	= veloBak;
			rb.angularVelocity = angularVelocityBak;
			coolTime = coolTime_;								// 再開時に衝突しないようにクールタイムを設定する
			//rb.Resume(gameObject);
		}
		else
		{
			//rb.Pause(gameObject);
			// 一時停止
			bodyTypeBak = rb.bodyType;							// Rigidbody タイプを取っておく
			rb.bodyType	= RigidbodyType2D.Kinematic;
			rb.gravityScale	= 0;
			veloBak	= rb.velocity;
			rb.velocity	= Vector2.zero;
			angularVelocityBak = rb.angularVelocity;
			rb.angularVelocity = 0;
		}
		// アニメーション
		PauseAnim(bSw);
	}

	// アニメーション一時停止
	public virtual void	PauseAnim(bool bSw)
	{
		if (an == null){ return;}
		if (!bSw)
		{
			an.speed = animSpeedBak;
		}
		else
		{
			animSpeedBak = an.speed;
			an.speed = 0;
		}
	}

	// アニメーション再生
	public virtual void PlayAnim(string animName)
	{
		if(an == null){ return; }
		if(an.runtimeAnimatorController == null){ return;}		// アニメーションファイルがない場合はここで終わり

		an.Play(animName);

		// 速度は保持しておく
		animSpeedBak = an.speed;
	}


	// 攻撃
	public virtual void Attack()
	{
		attakTime =	Define.attackTime;
		isAttack = true;
	}


	public virtual void	OnDamage()
	{
	}

	// 点滅
	public virtual IEnumerator DamageBlink(bool bDead)
	{
		isDamage = true;
		for	(int i = 0;	i <	3; i++)
		{
			if(sp){ sp.DOColor(new Color(1,	1, 1, 0), damageDuration);}
			yield return new WaitForSeconds(0.1f);

			if(sp){ sp.DOColor(new Color(1,	1, 1, 1), damageDuration);}
			yield return new WaitForSeconds(0.1f);
		}
		isDamage = false;
	}

	// ダメージ開始
	public virtual bool	StartDamage() // true : 壊れた
	{
		// HP が 0	のものは壊れない
		if (Hp == 0)
		{
			return false;
		}
		// HP 減らす。0	ならバラバラになる
		Hp--;
		if (Hp == 0)
		{
			return true;
		}
		else if	(Hp	<= maxHp / 2)
		{
		}
		PlaySound(SE.ID.Object, se_damage); 
		coolTime = coolTime_;
		return false;
	}

	// アルファ値
    public virtual void SetAlpha(SpriteRenderer sprite, float alpha)
    {
		if(sprite)
		{
			var c = sprite.color;
			sprite.color = new Color(c.r, c.g, c.b, alpha);
		}
    }

	// 檻から登場
	public virtual void StartFromCage()
	{
		StartFromCageInitialize();
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

	// 檻から登場時の初期化
	public virtual void StartFromCageInitialize()
	{

	}


	//
	// 以下、地面の判定
	//
	public virtual void InitCheckGround()
	{
		onPrevGround	 = false;
		onGround		 = true;
		isLanding        = false;
		onFloor			 = false;

		maskLadderMid	 = LayerMask.GetMask(collisionLadderMidLayer);		// ハシゴ
		maskLadderBottom = LayerMask.GetMask(collisionLadderBottomLayer);	// 地面に接地したハシゴ
		maskLadder		 = LayerMask.GetMask(collisionLadderLayer);			// maskLadder と maskLadderBottom
		maskFloor		 = LayerMask.GetMask(collisionFloorLayer);			// 床
	}

	// 地面を調べる
	//	bodyWidth      : 身体のサイズ ×2
	//	DistanceToFeet : 身体の中心から地面までの距離
	public virtual void CheckGround(float bodyWidth, float DistanceToFeet)
	{
		//Vector2	   boxPos  = new Vector2(hitGroundBox.position.x, hitGroundBox.position.y);
		//Vector2    boxSize = new Vector2(hitGroundBox.localScale.x, hitGroundBox.localScale.y) / 2f;

		prevonLadderMid	= onLadderMid;
		onLadderMid	    = CheckFeet(y - DistanceToFeet, bodyWidth, maskLadderMid);		// 床についていないハシゴ
		onLadderBottom  = CheckFeet(y - DistanceToFeet, bodyWidth, maskLadderBottom);	// 床についているハシゴ
		onLadder        = onLadderMid | onLadderBottom;

        // 地面判定(この方法だとジャンプ中に横の壁に当たっても床判定になるので没とする)
		//onFloor      = Physics2D.OverlapBox(boxPos, boxSize, 0f, maskFloor);

		// 下にレイを飛ばす方法だと横の壁で床判定がこないので、こうする
		onFloor      = CheckFoot(y - DistanceToFeet, bodyWidth, maskFloor);	// 床
		onPrevGround = onGround;											// 地面の前回値
		onGround     = onFloor | onLadderMid;								// 地面は床とハシゴ
		isLanding    = !onPrevGround && onGround;
	}

	// 左足、真ん中、右足の足元のどれかが mask	の上か調べる
	public virtual bool CheckFoot(float posy, float size, int mask)
	{
		// 身体の中央から足元から床までレイを飛ばす
		Vector2	pos	= new Vector2(x, posy);

		// 中央を取らないと球状のもの上に乗った時の判定が取れない
		RaycastHit2D center	= Physics2D.Raycast(pos, Vector2.down, rayDistance,	mask);
		bool onCenter = (center.collider != null && center.distance <	threshold);

		// 右端の足元調べる
		Vector2	pos_r =	new	Vector2(x +	size, pos.y);
		RaycastHit2D right = Physics2D.Raycast(pos_r, Vector2.down,	rayDistance, mask);
		bool onRight = (right.collider != null && right.distance	< threshold);

		// 左端の足元調べる
		Vector2	pos_l =	new	Vector2(x -	size, pos.y);
		RaycastHit2D left =	Physics2D.Raycast(pos_l, Vector2.down, rayDistance,	mask);
		bool onLeft	= (left.collider !=	null &&	left.distance <	threshold);

		return onCenter | onRight | onLeft;
	}


	// 身体の左右の足元がどちらも mask の上か調べる
	public virtual bool CheckFeet(float posy, float size, int mask)
	{
		RaycastHit2D r = Physics2D.Raycast(new Vector2(x + size, posy), Vector2.down, rayDistance, mask);
		RaycastHit2D l = Physics2D.Raycast(new Vector2(x - size, posy), Vector2.down, rayDistance, mask);
		return (l.collider != null && l.distance < threshold) && (r.collider !=	null &&	r.distance < threshold);
	}
}

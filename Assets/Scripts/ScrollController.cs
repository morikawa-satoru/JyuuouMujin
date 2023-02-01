using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using DG.Tweening;


public class ScrollController :	MonoBehaviour
{
/*
	// マップスクロール
	readonly float xScrollPosition = 9.1f;
	readonly float yScrollPosition = 5.0f;
	readonly float scrollTime = 1f;                     // スクール時間

	bool isScroll = false;                                  // true	: スクロール中




	void Update()
	{
		ScrollControll();
	}


	public Vector2 plxy
	{
		get
		{
			return Global.player[0].transform.position;
		}
		set
		{
			Global.player[0].transform.position = value;
		}
	}

	public float plx
	{
		get
		{
			return Global.player[0].transform.position.x;
		}
		set
		{
			Vector3 pos = Global.player[0].transform.position;
			pos.x = value;
			Global.player[0].transform.position = pos;
		}
	}

	public float ply
	{
		get
		{
			return Global.player[0].transform.position.y;
		}
		set
		{
			Vector3 pos = Global.player[0].transform.position;
			pos.y = value;
			Global.player[0].transform.position = pos;
		}
	}

	public Vector2 camxy
	{
		get
		{
			return Global.camera.transform.position;
		}
		set
		{
			Global.camera.transform.position = value;
		}
	}

	public float camx
	{
		get
		{
			return Global.camera.transform.position.x;
		}
		set
		{
			Vector3 pos = Global.camera.transform.position;
			pos.x = value;
			Global.camera.transform.position = pos;
		}
	}

	public float camy
	{
		get
		{
			return Global.camera.transform.position.y;
		}
		set
		{
			Vector3 pos = Global.camera.transform.position;
			pos.y = value;
			Global.camera.transform.position = pos;
		}
	}




	// マップスクロール
	void ScrollControll()
	{
		GameObject		 pl  = Global.player[0];
		PlayerController plc = Global.plc[0];




		if (isScroll) { return; }

		float xdir = 0, ydir = 0;
		float xx = plx - Global.mapx * Define.xRoomSize;
		float yy = ply + Global.mapy * Define.yRoomSize;
		float x_ = Global.camera.transform.position.x;
		float y_ = Global.camera.transform.position.y;
		float z_ = Global.camera.transform.position.z;

		// 右へ
		if (xx >= xScrollPosition) { xdir = 1; Global.mapx++; }
		// 左へ
		if (xx <= -xScrollPosition) { xdir = -1; Global.mapx--; }
		// 下へ
		if (yy <= -yScrollPosition) { ydir = -1; Global.mapy++; }
		// 上へ
		if (yy <= -yScrollPosition) { ydir = 1; Global.mapy--; }

		// マップスクロール
		int scroll = 0;
		Vector3 target = new Vector3(0, 0, 0);
		bool bConnectObjMove = false;
		if (xdir != 0)
		{
			scroll = 1;
			// カメラ座標
			target = new Vector3(x_ + Define.xRoomSize * xdir, y_, z_);
			// コネクトオブジェ(鎖につながっているが、手に持っていないとき)
			bConnectObjMove = (pl.isConnectObj && !isConnectObjHold);

			// プレイヤーを画面内にすこし移動
			transform.DOLocalMove(new Vector2(x + 1.5f * xdir, y), scrollTime)
				.OnUpdate(() => { rb.velocity = Vector2.zero; })
				.OnComplete(() => { isScroll = false; PlayAnimPause(false); });
		}

		if (ydir != 0)
		{
			scroll = 2;
			// カメラ座標
			target = new Vector3(x_, y_ + Define.yRoomSize * ydir, z_);
			// プレイヤーを画面内にすこし移動
			transform.DOLocalMove(new Vector2(x, y + 1.5f * ydir), scrollTime)
				.OnUpdate(() => { rb.velocity = Vector2.zero; })
				.OnComplete(() => { isScroll = false; PlayAnimPause(false); });
			// コネクトオブジェ(鎖につながっているが、手に持っていないとき)
			bConnectObjMove = (isConnectObj && !isConnectObjHold);
		}

		// スクロール開始
		if (scroll != 0)
		{
			isScroll = true;
			Global.isPause = true;                              // 全体的に一時停止
			Pause(true);
			if (isGround && !isFeetLadder) { PlayAnim(Anim.WALK); }
			PlayAnimPause(true);
			// カメラ
			Global.camera.transform.DOLocalMove(target, scrollTime)
				.OnComplete(() =>
				{
					PlayAnimPause(false);
					isScroll = false;
					Global.isPause = false;                     // 全体一時停止オフ
					Pause(false);
				});
		}

		// コネクト部分を身体へよせる
		if (bConnectObjMove)
		{
			connectPlugObj.Reset();
			connectPlug.transform.DOLocalMove(new Vector2(0, connectPlug.transform.localPosition.y), scrollTime)
				.OnUpdate(() =>
				{
					connectPlugRB.velocity = new Vector2(0, 0); // つなげている間はこうしないと鎖がどこかへいってしまう
				});
		}
	}
	*/
}

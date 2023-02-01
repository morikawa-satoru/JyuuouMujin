using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Rewired;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;


namespace Common
{
	public enum	Key	{ up, down,	left, right, jump, ok, cancel, pullThrow, disconnect, menu };
	public enum	Eff	{ Spark, Smoke,	Landing	};					// エフェクト
	public enum Dir { none = -1, up, down, left, right};		// 向き

	public class Define
	{
		public static readonly int maxStage	 = 3;
		public static readonly int maxPlayer = 2;
		public static readonly int maxItem	 = 100;

		public static readonly string normalMessage	  =	"秒で脱出せよ";
		public static readonly string nextRoomMessage =	"次の部屋に行け";
		public static readonly string LeftTimeText	  =	"秒で扉は閉まる";

		public static readonly Vector2 throwDirection =	new	Vector2(1, 0.00f).normalized;  // 投げる方向
		public static readonly float   ThrowPower			= 2000f;	// 投げる力
		public static readonly float   DropPower			= 50f;		// 落とす力
		public static readonly float   ropeDisconnectLength	= 2.4f;		// ロープが切れる長さ
		public static readonly float   ropeMaxLength		= 2.0f;		// ロープの長さ
		public static readonly float   ropeMaxLength2		= 3.2f;		// ロープがこれ以上伸びたらプレイヤーの動きを強制的に止める
		public static readonly float   attackTime			= 0.3f;		// 攻撃時間
		public static readonly float   thresholdSpeed		= 1.1f;		// この値以上ならば攻撃になる

		// キャンパス
		public static readonly Vector2 canvasSize = new Vector2(18f, 12f);

		// マップ
		public const float xRoomSize  = 19.0f;
		public const float yRoomSize  = 11.0f;
		public const float scrollTime = 1;						// マップスクロール時間
		public const float xScrollPosition   = 9.1f;			// マップスクロールエリア
		public const float yScrollPosition   = 5.0f;
		public const float mapScrollDistance = 1.5f;			// マップスクロール時にプレイヤーが進む距離(画面端よりすこし内側に移動させるため)

		// タグ
		public const string	tagPlayer		= "Player";
		public const string tagCatchObj		= "CatchObj";       // つかめるもの
		public const string	tagConnectObj	= "ConnectObj";		// コネクトオブジェ(ロープにつながっているもの)
		public const string	tagDoor			= "Door";			// ドア
		public const string	tagWall			= "Wall";			// 壁
		public const string	tagBreakingWall	= "BreakingWall";	// 壊れる壁
		public const string	tagEnemy		= "Enemy";			// エネミー
		public const string	tagEnemyBullet	= "EnemyBullet";	// エネミー弾

		// レイヤー
		public const string	layerHoldObj	   = "HoldObj";			// 手に持っているオブジェ(プレイヤーとデフォルト以外は当たらない)
		public const string	layerConnectObj	   = "ConnectObj";		// 手に持っていないがロープにつながっているオブジェ
		public const string	layerPlayer		   = "Player";			// プレイヤー
		public const string	layerPlayerThrough = "PlayerThrough";	// プレイヤー
		public const string	layerLadder		   = "Ladder";			// ハシゴ

		public const string layerHahen = "Hahen";
		public static int   iLayerHahen;

		// アニメーション
		public const string	animDamage = "Damage";				// ダメージアニメ
		public const string	animIdle   = "Idle";				// 停止
		public const string	animWalk   = "Walk";				// 歩き
		public const string	animAttack = "Attack";				// 攻撃
		public const string	animSit    = "Sit";					// 座り
		public const string	animConnect_Idle   = "Connect_Idle";	// 停止
		public const string	animConnect_Damage = "Connect_Damage";	// ダメージアニメ
		public const string	animConnect_Attack = "Connect_Attack";	// 攻撃
		//public const string	animConnect_Sit    = "Connect_Sit";		// 座り

		// リソース
		public const string	prefabFadeSurface =	"Prefabs/FadeSurface";			// フェード用
		public const string	prefabEnemyBullet =	"Prefabs/Enemy/Spore";			// 弾
		public const string fileHahenMat	  = "Materials/obj_hahen";			// 破片マテリアル

		// フォルダ
		public const string	folderCatchObj = "Prefabs/CatchObj/";
	}

	public class Global
	{
		public static GameController	game;								// ゲームコントローラー
		public static Camera			camera, cameraUI;					// カメラ・UI用カメラ
		public static CameraController	camController;						// カメラコントローラー
		public static EffectController	effect;								// エフェクトコントローラー
		public static GameObject		map;								// マップ
		public static Text				debug;								// デバッグ
		public static FadeController	fade;								// フェード

		public static float	scale	   = 1;									// 画面のスケール(1～4)
		public static bool	isOver	   = false;								// 時間オーバー
		public static bool	isPlayerSE = false;								// 時間オーバー
		public static bool	isPause,prevPause, isPauseStart, isPauseEnd;	// 一時停止
		public static bool	isMessage;										// メッセージ表示中
		public static bool	isReset;										// リセット
		public static bool	isGame;											// ゲーム中か
		public static bool	isFocus;										// プレイヤーに焦点を当てて拡大する
		public static bool	isMapScroll;									// マップスクロール中

		public static int	numStage  = 1;									// 現在のステージ数
		public static int	numPlayer = 0;									// プレイヤー数

		public static GameObject[]		 player	= new GameObject[      Define.maxPlayer];	// プレイヤーオブジェクト
		public static PlayerController[] plc	= new PlayerController[Define.maxPlayer];	// プレイヤーコントローラー

		public static Vector2Int		mapArea;							// 表示しているマップ位置
		public static PhysicsMaterial2D hahenMat;							// 破片の物理マテリアル


		public static void Initialize()
		{
			hahenMat = (PhysicsMaterial2D)Resources.Load(Define.fileHahenMat);		// 破片マテリアル
			Define.iLayerHahen = LayerMask.NameToLayer(Define.layerHahen);			// 破片のレイヤー番号
		}
		
		public static bool GetBlink(float counter)
		{
			return ((int)(counter *	16.6666f) %	5) < 2;
		}

		public static int GetStage()
		{
			return numStage;
		}

		public static void SetStage(int	st)
		{
			numStage = st;
		}

		// ステージ読み込み
		public static void LoadStage(int stage)
		{
			string str;
			if (stage <= Define.maxStage &&	stage != 0)
			{
				str	= "Stage" +	stage.ToString().PadLeft(2,	'0');
			}
			else
			{
				str	= "Title";
			}
			SceneManager.LoadScene(str);
		}

		// キーの状態を調べる
		public static	bool CheckKey(int id, Key key)
		{
			const	float threshold	= 0.1f;				// アナログスティックの閾値

			bool	bRet = false;

			//id--;
			switch (key)
			{
				case Key.up:
					bRet = (ReInput.players.GetPlayer(id).GetAxis("Move_Vertical") > threshold);
					break;
				case Key.down:
					bRet = (ReInput.players.GetPlayer(id).GetAxis("Move_Vertical") <= -threshold);
					break;
				case Key.left:
					bRet = (ReInput.players.GetPlayer(id).GetAxis("Move_Horizontal") <=	-threshold);
					break;
				case Key.right:
					bRet = (ReInput.players.GetPlayer(id).GetAxis("Move_Horizontal") > threshold);
					break;
				case Key.ok:
				case Key.jump:
					bRet = ReInput.players.GetPlayer(id).GetButton("Jump_Cancel");
					break;
				case Key.cancel:
				case Key.pullThrow:
					bRet = ReInput.players.GetPlayer(id).GetButton("Throw_Ok");
					break;
				case Key.disconnect:
					bRet = ReInput.players.GetPlayer(id).GetButton("Disconnect");
					break;
				case Key.menu:
					bRet = ReInput.players.GetPlayer(id).GetButton("Menu");
					break;
			}

			return bRet;
		}

		// キーの押した瞬間を調べる
		public static bool CheckPressKey(int id, Key key)
		{
			bool bRet =	false;
			switch (key)
			{
				case Key.up:
					bRet = (ReInput.players.GetPlayer(id).GetButtonDown("Move_Vertical"));
					break;
				case Key.down:
					bRet = (ReInput.players.GetPlayer(id).GetNegativeButtonDown("Move_Vertical"));
					break;
				case Key.left:
					bRet = (ReInput.players.GetPlayer(id).GetNegativeButtonDown("Move_Horizontal"));
					break;
				case Key.right:
					bRet = (ReInput.players.GetPlayer(id).GetButtonDown("Move_Horizontal"));
					break;
				case Key.cancel:
				case Key.jump:
					bRet = (ReInput.players.GetPlayer(id).GetButtonDown("Jump_Cancel"));
					break;
				case Key.ok:
				case Key.pullThrow:
					bRet = (ReInput.players.GetPlayer(id).GetButtonDown("Throw_Ok"));
					break;
				case Key.disconnect:
					bRet = (ReInput.players.GetPlayer(id).GetButtonDown("Disconnect"));
					break;
				case Key.menu:
					bRet = (ReInput.players.GetPlayer(id).GetButtonDown("Menu"));
					break;
			}
			return bRet;
		}

		// キーを放した瞬間を調べる
		public static bool CheckReleaseKey(int id, Key key)
		{
			bool bRet =	false;
			switch (key)
			{
				case Key.up:
					bRet = (ReInput.players.GetPlayer(id).GetButtonUp("Move_Vertical"));
					break;
				case Key.down:
					bRet = (ReInput.players.GetPlayer(id).GetNegativeButtonUp("Move_Vertical"));
					break;
				case Key.left:
					bRet = (ReInput.players.GetPlayer(id).GetNegativeButtonUp("Move_Horizontal"));
					break;
				case Key.right:
					bRet = (ReInput.players.GetPlayer(id).GetButtonUp("Move_Horizontal"));
					break;
				case Key.ok:
				case Key.jump:
					bRet = (ReInput.players.GetPlayer(id).GetButtonUp("Jump_Ok"));
					break;
				case Key.cancel:
				case Key.pullThrow:
					bRet = (ReInput.players.GetPlayer(id).GetButtonUp("Cancel"));
					break;
				case Key.disconnect:
					bRet = (ReInput.players.GetPlayer(id).GetButtonUp("Disconnect"));
					break;
			}
			return bRet;
		}

		public static async	Task WaitForAsync(float	seconds, Action	action)
		{
			await Task.Delay(TimeSpan.FromSeconds(seconds));
			action();
		}

		// ゲームオブジェクトを分解する
		public static void DisassemblyGameObject(GameObject mother, bool includeMother = false)
		{
			// 子オブジェクトをコピーしてバラバラにする
			List<GameObject> childList = new List<GameObject>();

			int max = mother.transform.childCount;				// DisassemblySub 内で親子関係を解消しているので、こうしている
			for(int i = 0; i < max; i++)
			{
				childList.Add(mother.transform.GetChild(i).gameObject);
			}
			foreach(GameObject go in childList)
			{
				DisassemblySub(go);
			}
			// マザーも飛ばす
			if(includeMother)
			{
				DisassemblySub(mother);
			}
		}

		// 飛ばす
		static void DisassemblySub(GameObject g)
		{
			g.transform.parent = null;							// 親子関係解除
			var a = g.GetComponent<HingeJoint2D>();
			if(a != null){ UnityEngine.Object.Destroy(a); }		// ヒンジは削除する
			var b = g.GetComponent<UnityEngine.Experimental.Rendering.Universal.ShadowCaster2D>();
			if(b != null){ UnityEngine.Object.Destroy(b); }		// 影も削除
			g.AddComponent<HahenController>();					// 破片コントローラーを追加(点滅後消去させるため)
		}
	}

	public class Dbg
	{
		[Conditional("UNITY_EDITOR")]
		public static void Print(string str)
		{
			Global.debug.text = str;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common;


//
// カメラコントローラー
//	画面切り替え領域を出ないように拡大・縮小する
//
public class CameraController : MonoBehaviour
{

	readonly float zoomSpeed  = 1;								// 拡大・縮小速度
	readonly float moveTime   = 0.5f;							// 拡大・縮小時間
	readonly float cameraSize = 5.343f;
	readonly float xRoomSize = Define.xRoomSize / 2;			// 部屋の横サイズ
	readonly float yRoomSize = (Define.yRoomSize - 0.3f) / 2f;	// 部屋の縦サイズ(縦だけすこし小さめにする)

	bool	whileZoomInOut = false;								// ズーム中フラグ


	void Update()
	{
		checkScaleKey();
		scaleControll();
	}

	// スケールコントロール
	void scaleControll()
	{
		if (Global.isMapScroll){ return; }

		// プレイヤー座標よりカメラ座標を設定(※画面端を計算する)
		Global.camera.transform.position = GetCameraPos(new Vector3(Global.plc[0].transform.position.x, Global.plc[0].transform.position.y, Global.camera.transform.position.z));
	}

	// キーボードによる拡大・縮小
	void checkScaleKey()
	{
		float isZoomInOut = 0;
		if(!whileZoomInOut)
		{
			if (	 Global.scale > 1 && Input.GetKeyDown(KeyCode.A)){ isZoomInOut = -zoomSpeed;}
			else if (Global.scale < 4 && Input.GetKeyDown(KeyCode.S)){ isZoomInOut =  zoomSpeed;}
			if (isZoomInOut != 0)
			{
				Global.isFocus = true;
				whileZoomInOut = true;
				DOTween.To(
					() =>  Global.scale,
					(x) => Global.scale = x,					// ラムダ式で Global.scale を変える
					Global.scale + isZoomInOut,
					moveTime)
				.OnUpdate(() =>
				{
					Global.camera.orthographicSize   = cameraSize / Global.scale;
					Global.cameraUI.orthographicSize = Global.camera.orthographicSize;
					Global.plc[0].message.SetScale(Global.scale);
				})
				.OnComplete( () =>
				{
					whileZoomInOut = false;
				});
			}
		}
	}

	// pos(カメラ座標) をもとにカメラ座標の四隅を計算し隣の画面が見えないように値を加工したカメラ座標を返す。scale は 1～4(拡大)
	Vector3 GetCameraPos(Vector3 pos)
	{
		// Step1 : カメラ座標バックアップ
		Vector3 bak = Global.camera.transform.position;

		// Step2 : 指定座標をカメラ座標とする
		Global.camera.transform.position = pos;

		// Step3 : カメラの左上と右下を求める
		Vector2 cam_rt = Global.camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
		Vector2 cam_lb = Global.camera.ScreenToWorldPoint(Vector3.zero);

		// Step4 : 描画領域を計算
		Vector3 retValue = pos;
		float mapx = Global.mapArea.x * Define.xRoomSize;
		float mapy = Global.mapArea.y * Define.yRoomSize;
		float remainderX0 = cam_lb.x - mapx;
		float remainderX1 = cam_rt.x - mapx;
		float remainderY0 = cam_rt.y + mapy;
		float remainderY1 = cam_lb.y + mapy;

		// 指定座標は左端か?
		if (remainderX0 <= -Define.xRoomSize / 2)
		{
			retValue.x -= cam_lb.x + (-mapx + xRoomSize);
		}
		// 指定座標は右端か?
		else if (remainderX1 >= Define.xRoomSize / 2)
		{
			retValue.x -= cam_rt.x - (mapx + xRoomSize);
		}
		// 指定座標は上端か?
		if (remainderY0 >= yRoomSize)
		{
			retValue.y -= cam_rt.y + (mapy - yRoomSize);
		}
		// 指定座標は下端か?
		else if (remainderY1 <= -yRoomSize)
		{
			retValue.y -= cam_lb.y + (mapy + yRoomSize);
		}

		// Step5 : カメラをもとに戻す
		Global.camera.transform.position = bak;

		// 戻り値は新たなカメラの中心座標
		return retValue;
	}

	// カメラ位置移動
	public void SetupCamera(Vector2 afterPos)
	{
		Vector3 retValue = GetCameraPos(new Vector3(afterPos.x, afterPos.y, Global.camera.transform.position.z));
		transform.position = retValue;
	}

	// マップスクロール開始
	public void StartCameraMove(Vector2 afterPos)
	{
		Global.isMapScroll = true;
		Global.isPause = true;								// 全体的に一時停止

		Vector3 retValue = GetCameraPos(new Vector3(afterPos.x, afterPos.y, Global.camera.transform.position.z));
		Global.camera.transform.DOLocalMove(retValue, Define.scrollTime)
			.OnComplete(() =>
			{
				Global.isMapScroll = false;
				Global.isPause = false;						// 全体一時停止オフ
			});
	}

	// 振動化開始
	public void	Shake( float duration, float magnitude )
	{
		StartCoroutine(	DoShake( duration, magnitude ) );
	}

	// 振動は非同期で行う
	IEnumerator	DoShake( float duration, float magnitude )
	{
		var	pos	= transform.localPosition;

		var	elapsed	= 0f;

		while (	elapsed	< duration )
		{
			var	x =	pos.x +	Random.Range( -1f, 1f )	* magnitude;
			var	y =	pos.y +	Random.Range( -1f, 1f )	* magnitude;

			transform.localPosition	= new Vector3( x, y, pos.z );

			elapsed	+= Time.deltaTime;

			yield return null;
		}

		transform.localPosition	= pos;
	}
}




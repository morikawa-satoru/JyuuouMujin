using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
	//このScriptはMainCameraにアタッチしてください

	public RenderTexture renderTexture;							// mainCameraにつけるRendertexture(アタッチしてね)
	public Texture2D	 kakunin;								// ほんとに保存されているかの確認用
	public Sprite		 spriteTest;							// Sprite確認用
	Camera mainCamera;


	void Start()
	{
		mainCamera = GetComponent<Camera>();
		kakunin	   = CreateTexture2D(renderTexture);
		spriteTest = CreateSprite(kakunin);
	}

	///	<summary>
	///	ここでTextur2Dに変換しているよ
	///	</summary>
	///	<param name="rt"></param>
	///	<returns></returns>
	Texture2D CreateTexture2D(RenderTexture	rt)
	{
		//Texture2Dを作成
		Texture2D texture2D	= new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, false);

		// subCameraにRenderTextureを入れる
		mainCamera.targetTexture = rt;

		//手動でカメラをレンダリングします
		mainCamera.Render();

		RenderTexture.active = rt;
		texture2D.ReadPixels(new Rect(0, 0,	rt.width, rt.height), 0, 0);
		texture2D.Apply();

		//元に戻す別のカメラを用意してそれをRenderTexter用にすれば下のコードはいらないです。
		mainCamera.targetTexture = null;
		RenderTexture.active = null;

		return texture2D;
	}

	Sprite CreateSprite(Texture2D tex2D)
	{
		Sprite sprite =	Sprite.Create(tex2D, new Rect(0f, 0f, tex2D.width, tex2D.height), new Vector2(0.5f,	0.5f), 100f);
		return sprite;
	}
}
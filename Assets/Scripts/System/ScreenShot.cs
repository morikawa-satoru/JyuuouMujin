using System.Collections;
using UnityEngine;


public class ScreenShot	: MonoBehaviour
{

	[SerializeField]
	Camera		   camera_;
	[SerializeField]
	SpriteRenderer sprite_;

	IEnumerator	CreateScreenShot()
	{
		// レンダリング完了まで待機
		yield return new WaitForEndOfFrame();

		RenderTexture renderTexture	= new RenderTexture(Screen.width, Screen.height, 24);
		camera_.targetTexture =	renderTexture;

		Texture2D texture =	new	Texture2D(camera_.targetTexture.width, camera_.targetTexture.height, TextureFormat.RGB24, false);

		texture.ReadPixels(new Rect(0, 0, camera_.targetTexture.width, camera_.targetTexture.height), 0, 0);
		texture.Apply();

		camera_.targetTexture =	null;

		Sprite sprite =	Sprite.Create(texture, new Rect(0f,	0f,	texture.width, texture.height),	new	Vector2(0.5f, 0.5f), 100f);
		sprite_.sprite = sprite;
	}

	public void	StartCapture()
	{
		StartCoroutine(CreateScreenShot());
	}
}
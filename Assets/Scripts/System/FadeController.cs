//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
//using Common;


public class FadeController : MonoBehaviour
{

	[SerializeField]
	GameObject	  fade;

	SpriteRenderer surface;

	bool	  isFade = false;
	string nextScene = null;


	void Start()
	{
		fade.SetActive(true);
		isFade = false;
		/*fade = GameObject.Find("FadeSurface");
		if(!fade)
		{
			GameObject a = (GameObject)Resources.Load(Define.prefabFadeSurface);
			fade = Instantiate(a, new Vector3(0, 0, 0), Quaternion.identity);
		}*/
		//fade.gameObject.transform.position = new Vector3(0, 0, 0);
		surface	  = fade.GetComponent<SpriteRenderer>();
		nextScene =	null;
	}

	//フェードイン開始
	public void In(float duration = 0.3f)
	{
		//if (surface	== null){ Init();}
		nextScene =	null;
		surface.enabled	= true;
		isFade = true;

		surface.DOFade(0, duration).OnComplete(
			() => {
				isFade = false;
				surface.enabled	= false;
			}
			);
	}

	public bool Check()
	{
		return isFade;
	}

	//　フェードアウト開始
	public void Out(float duration = 0.3f)
	{
		//if (surface	== null){ Init();}
		surface.enabled	= true;
		isFade			= true;
		surface.DOFade(1, duration).OnComplete(
			() => {
				isFade = false;
				if (nextScene != null) SceneManager.LoadScene(nextScene);
			}
			);
	}

	// ハーフフェードアウト開始
	public void HalfOut(float duration = 0.2f)
	{
		//if (surface	== null){ Init();}
		surface.color =	Color.clear;
		surface.enabled	= true;
		isFade = true;
		surface.DOFade(0.5f, duration).OnComplete(
			() => {
				isFade = false;
			}
			);
	}

	// フェードアウト後に移行するシーンを設定(nullで移行しない)
	public void SetNextScene(string n)
	{
		nextScene =	n;
	}
}


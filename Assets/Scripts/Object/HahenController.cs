using System.Collections;
using UnityEngine;
using DG.Tweening;
using Common;


public class HahenController : _CharaBase
{

	readonly float timeWait = 1f;
	float	 time_;

	// 初期化
	public override	void Initialize()
	{
		time_ = timeWait + Random.Range(0, 0.5f);
		if(sp != null)
		{
			sp.DOFade(0, time_).OnComplete(() =>
			{
				Destroy(gameObject);
			}).SetDelay(0.1f);
		}
		else
		{
			// マテリアリを得てコピーしフェードする
			MeshRenderer mr = GetComponent<MeshRenderer>();
			mr.material.DOFade(0, time_).OnComplete(() =>
			{
				Destroy(gameObject);
			});
		}

		// 飛ばす
		if(rb)
		{
			rb.sharedMaterial = Global.hahenMat;
			rb.bodyType = RigidbodyType2D.Dynamic;
			rb.AddForce(new Vector2(UnityEngine.Random.Range(-200.0f, 200.0f), UnityEngine.Random.Range(-200.0f, 200.0f)));
		}

		// レイヤーを変える
		gameObject.layer = Define.iLayerHahen;
	}
}

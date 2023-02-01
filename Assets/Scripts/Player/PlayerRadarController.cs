using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class PlayerRadarController : MonoBehaviour
{
	// 
	// プレイヤー本体は	Ray	の処理で自身のコライダーをオン・オフするため
	// OnTriigerEnter・OnTriggerExit2Dが誤動作する
	// 対策として子オブジェクトで判定を行う

	[SerializeField]
	PlayerController pl;

	GameObject[] list =	new	GameObject[10];


	void OnTriggerEnter2D(Collider2D col)
	{
		switch (col.gameObject.tag)
		{
			// つなげることができるオブジェクト
			case Define.tagCatchObj:
				SetList(col.gameObject);
				break;
		}
	}

	void OnTriggerExit2D(Collider2D	col)
	{
		switch (col.gameObject.tag)
		{
			// つなげることができるオブジェクト
			case Define.tagCatchObj:
				DelList(col.gameObject);
				break;
		}
	}


	// リストにゲームオブジェクトを登録
	void SetList(GameObject	gc)
	{
		for	(int i = 0;	i <	list.Length; i++)
		{
			if (list[i]	!= null) { continue; }							// すでに登録済みか?
			if (list[i]	== gc && list[i].name == gc.name) {	return;	}	// すでに同じ物があるならば登録しない
			list[i]	= gc;
			break;
		}
	}

	// 削除
	void DelList(GameObject	gc)
	{
		for	(int i = 0;	i <	list.Length; i++)
		{
			// ゲームオブジェクトと名前が同じなら削除
			if (list[i]	== gc && list[i].name == gc.name)
			{
				list[i]	= null;
				break;
			}
		}
	}

	// リストの中から一番近いゲームオブジェクトを返す
	public GameObject GetNearObject()
	{

		float dist = 100000;
		GameObject gc =	null;

		for	(int i = 0;	i <	list.Length; i++)
		{
			if(list[i] == null){ continue;}

			// プレイヤーからの距離を求める
			float a	= Vector2.Distance(list[i].transform.position, pl.transform.position);

			// 距離が小さいものを登録
			if(dist	> a)
			{
				dist = a;
				gc = list[i];
			}
		}
		return gc;
	}
}


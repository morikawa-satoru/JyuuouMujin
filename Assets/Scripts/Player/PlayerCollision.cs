using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class PlayerCollision : MonoBehaviour
{

	// プレイヤー本体は	Ray	の処理で自身のコライダーをオン・オフするため
	// OnTriigerEnter・OnTriggerExit2Dが誤動作する
	// 対策として子オブジェクトで判定を行う

	[SerializeField]
	PlayerController pl;


	void OnTriggerEnter2D(Collider2D col)
	{
		switch(col.gameObject.tag)
		{
			// ドア
			case Define.tagDoor:
				pl.isDoor =	true;
				pl.door	= col.gameObject.GetComponent<DoorController>();
				break;
			// コネクトオブジェ
			case Define.tagConnectObj:
				pl.isBodyHit = true;							// 身体に当たったフラグ
				col.gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				col.gameObject.transform.parent.GetComponent<Rigidbody2D>().angularVelocity	= 0;
				break;
		}
	}

	void OnTriggerExit2D(Collider2D	col)
	{
		switch (col.gameObject.tag)
		{
			// ドア
			case Define.tagDoor:
				pl.isDoor =	false;
				break;
			// コネクトオブジェ
			case Define.tagConnectObj:
				if(!pl.isConnectObjHold) { pl.isBodyHit	= false;}	// 身体に当たったフラグ
				break;
		}
	}
}


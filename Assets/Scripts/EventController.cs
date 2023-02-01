using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class EventController : _Base
{
	// プレイヤーと当たったら PlayerController の ev に このクラスを入れる


	public string message =	"#一時停止#key#end";

	int	playerId = -1;


	void Update()
	{
		if(Global.isReset)
		{
			playerId = -1;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(Global.isMessage){ return; }
		if (col.gameObject.tag == "PlayerEvent")
		{
			playerId = col.gameObject.transform.parent.gameObject.GetComponent<PlayerController>().id;
			Global.plc[playerId].ev	= this;
		}
	}

	void OnTriggerExit2D(Collider2D	col)
	{
		if (col.gameObject.tag == "PlayerEvent")
		{
			if (playerId ==	col.gameObject.transform.parent.gameObject.GetComponent<PlayerController>().id)
			{
				Global.plc[playerId].ev	= null;
			}
		}
	}
}

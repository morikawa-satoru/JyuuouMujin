using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class RadarController : MonoBehaviour
{

	[SerializeField]
	bool	enablePlayer, enableCatchObj, enableConnectObj,	enableGround, enableWall, enableCeil, enableBreakingWall;


	public bool	isHit		   { set; get; } = false;
	public bool	isPlayer	   { set; get; } = false;
	public bool	isCatchObj	   { set; get; } = false;
	public bool	isConnectObj   { set; get; } = false;
	public bool	isWall		   { set; get; } = false;
	public bool	isBreakingWall { set; get; } = false;


	void OnTriggerEnter2D(Collider2D col)
	{
		switch (col.gameObject.tag)
		{
			case Define.tagPlayer:
				if (enablePlayer){ isPlayer	= true;	}
				break;
			case Define.tagCatchObj:
				if (enableCatchObj)	{ isCatchObj = true; }
				break;
			case Define.tagConnectObj:
				if (enableConnectObj) {	isConnectObj = true; }
				break;
			case Define.tagWall:
				if (enableWall)	{ isWall = true; }
				break;
			case Define.tagBreakingWall:
				isBreakingWall = true;
				break;
		}
		isHit =	isPlayer | isCatchObj |	isConnectObj | isWall | isBreakingWall;
	}

	void OnTriggerExit2D(Collider2D	col)
	{
		switch (col.gameObject.tag)
		{
			case Define.tagPlayer:
				isPlayer = false;
				break;
			case Define.tagCatchObj:
				isCatchObj = false;
				break;
			case Define.tagConnectObj:
				isConnectObj = false;
				break;
			case Define.tagWall:
				isWall = false;
				break;
			case Define.tagBreakingWall:
				isBreakingWall = false;
				break;
		}
		isHit =	isPlayer | isCatchObj |	isConnectObj | isWall | isBreakingWall;
	}
}

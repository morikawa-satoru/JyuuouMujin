using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class PlayerCatchObjectCollisionController :	MonoBehaviour
{

	GameObject releaseObj;										// 切り離したオブジェクト

	//int   layerMask;											// レイヤー
	bool  isHit;
	float coolTime;


	void Start()
	{
		releaseObj = null;
		isHit = false;
		coolTime = 0;
		//layerMask =	LayerMask.NameToLayer("ThroughCatchObj");
	}


	void Update()
	{
		if(releaseObj == null)	{ return;}

		if(!isHit && coolTime >	0)
		{
			coolTime -=	Time.deltaTime;
			if(coolTime	<= 0)
			{
				releaseObj.layer = LayerMask.NameToLayer("CatchObj");
				releaseObj = null;
			}
		}
	}


	public void	SetReleaseObj(GameObject obj)
	{
		releaseObj = obj;
		coolTime = 0.5f;
	}


	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject == releaseObj)
		{
			isHit =	true;
			//Debug.Log("Hit : " + col.gameObject.name);
		}
	}

	/*void OnTriggerStay2D(Collider2D col)
	{
		if(releaseObj == null){	return;}

		if (col.gameObject == releaseObj)
		{
			isHit =	true;
			if(col.gameObject.layer	== layerMask)
			{
			}
			else
			{
				Debug.Log("Change : " +	col.gameObject.name);
				releaseObj.layer = LayerMask.NameToLayer("CatchObj");
				releaseObj = null;
			}
		}
	}*/

	void OnTriggerExit2D(Collider2D	col)
	{

		if (col.gameObject == releaseObj)
		{
			isHit =	false;
			//Debug.Log("Change : " +	col.gameObject.name);
			releaseObj.layer = LayerMask.NameToLayer("CatchObj");
			releaseObj = null;
		}
	}
}

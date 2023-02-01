using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;


public class EnemyShootController :	MonoBehaviour
{

	[SerializeField]
	GameObject prefabShootEnemy;								// エネミーのプレハブ
	[SerializeField]
	int	  number;												// 個数(0:無限)
	[SerializeField]
	float interval;												// 射出間隔


	float	shootTime =	0;
	bool	isEnd =	false;


	void Start()
	{
		shootTime =	interval;
	}

	void Update()
	{
		if (Global.isPause)	{ return; }							// 一時停止か?
		if (isEnd){	return;	}

		shootTime -= Time.deltaTime;
		if(shootTime <=	0)
		{
			shootTime += interval;
			if(number >	0)
			{
				number--;
				if(number == 0){ isEnd = true; }
			}

			// 敵を出す
			Instantiate(prefabShootEnemy, transform.position, transform.rotation);
		}
	}
}

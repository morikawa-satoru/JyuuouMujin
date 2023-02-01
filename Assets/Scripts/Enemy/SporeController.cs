using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class SporeController : _EnemyBase
{

	float timer = 2;

	public override void Initialize()
	{
         SetAlpha(sp, 1);
		 timer = 2;
	}


	void Update()
	{
		// 一時停止
		if (Global.isPauseStart) { Pause(true);	}
		if (Global.isPauseEnd) { Pause(false); }
		if (Global.isPause)	{ return; }							// 一時停止か?
		//if (isDead)	{ return; }									// 死んでいるか?
		if (isDontMove)	{ return; }								// 動かないか?

		timer -= Time.deltaTime;
		if(timer > 0){ return;}
		Destroy(gameObject);
	}
}

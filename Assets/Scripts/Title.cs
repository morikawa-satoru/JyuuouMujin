using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class Title : _Base
{

	void Start()
	{
		_ =	Global.WaitForAsync(0.5f, () =>	Global.fade.In());
	}

	void Update()
	{
		if (Global.fade.Check()) { return; }

		// キーチェック
		for	(int id	= 1; id	<= Define.maxPlayer; id++)
		{
			// 決定
			if (Global.CheckPressKey(id, Key.jump))
			{
				PlaySound(SE.ID.System, SE.START);
				Global.fade.SetNextScene("Stage1");
				Global.fade.Out();
			}
		}
	}
}

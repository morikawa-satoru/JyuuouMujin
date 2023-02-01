using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;


public class EnemyShootController :	MonoBehaviour
{

	[SerializeField]
	GameObject prefabShootEnemy;								// �G�l�~�[�̃v���n�u
	[SerializeField]
	int	  number;												// ��(0:����)
	[SerializeField]
	float interval;												// �ˏo�Ԋu


	float	shootTime =	0;
	bool	isEnd =	false;


	void Start()
	{
		shootTime =	interval;
	}

	void Update()
	{
		if (Global.isPause)	{ return; }							// �ꎞ��~��?
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

			// �G���o��
			Instantiate(prefabShootEnemy, transform.position, transform.rotation);
		}
	}
}

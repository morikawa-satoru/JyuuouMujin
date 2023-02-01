using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

//
// �ǂ̃`�F�b�N�悤
//
public class PlayerWallController :	MonoBehaviour
{

	[SerializeField]
	public bool	isWallHit;


	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == Define.tagWall)
		{
			isWallHit =	true;
		}
	}

	void OnTriggerExit2D(Collider2D	col)
	{
		if (col.gameObject.tag == Define.tagWall)
		{
			isWallHit =	false;
		}
	}
}

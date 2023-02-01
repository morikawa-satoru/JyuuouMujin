using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;


public class EffectController: MonoBehaviour
{

	[SerializeField]
	string SparkFile, SmokeFile, LandingFile;					// エフェクトファイル


	GameObject[] obj;


	void Start()
	{
		obj	= new GameObject[System.Enum.GetNames(typeof(Eff)).Length];

		obj[0] = (GameObject)Resources.Load(SparkFile);
		obj[1] = (GameObject)Resources.Load(SmokeFile);
		obj[2] = (GameObject)Resources.Load(LandingFile);
	}


	public void	Start(Eff id, Vector2 pos)
	{
		Instantiate(obj[(int)id], pos, Quaternion.identity);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class FanController : MonoBehaviour
{

	[SerializeField]
	float speed;


	void Start()
	{
		Vector3	localAngle = transform.localEulerAngles;
		localAngle.z = Random.Range(0.0f, 360.0f);
		transform.localEulerAngles = localAngle;
	}

	void Update()
	{
		if (Global.isPause)	{ return; }							// 一時停止か?

		Vector3	localAngle = transform.localEulerAngles;
		localAngle.z +=	speed;
		transform.localEulerAngles = localAngle;
	}
}

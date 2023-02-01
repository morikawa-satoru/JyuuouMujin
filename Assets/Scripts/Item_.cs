using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ : MonoBehaviour
{

	private	GameController game;
	private	Rigidbody2D	   rb;							// 自分の物理挙動コンポーネント

	private	bool isLost	= false;


	void Start()
	{
		game   = GameObject.Find("Main").GetComponent<GameController>();
		rb	   = GetComponent<Rigidbody2D>();
		isLost = false;
	}

	public void	ReStart()
	{
		this.enabled   = true;
		rb.isKinematic = false;
		isLost		   = false;
	}

	void Update()
	{
		//if (!game.getOver()) { return; }

		rb.velocity	   = Vector2.zero;					// プレイヤーキャラ停止
		rb.isKinematic = true;
	}

	public void	SetLost()
	{
		isLost = true;
	}

	public bool	GetLost()
	{
		return isLost;
	}
}

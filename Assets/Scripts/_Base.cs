using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class _Base : MonoBehaviour
{

	public Vector2 xy
	{
		get
		{
			return transform.position;
		}
		set
		{
			transform.position = value;
		}
	}

	public Vector3 xyz
	{
		get
		{
			return transform.position;
		}
		set
		{
			transform.position = value;
		}
	}

	public float x
	{
		get
		{
			return transform.position.x;
		}
		set
		{
			Vector3	pos	= transform.position;
			pos.x =	value;
			transform.position = pos;
		}
	}

	public float y
	{
		get
		{
			return transform.position.y;
		}
		set
		{
			Vector3	pos	= transform.position;
			pos.y =	value;
			transform.position = pos;
		}
	}

	public float scaleX
	{
		get
		{
			return transform.localScale.x;
		}
		set
		{
			Vector3	scale = transform.localScale;
			scale.x = value;
			transform.localScale = scale;
		}
	}

	public float scaleY
	{
		get
		{
			return transform.localScale.y;
		}
		set
		{
			Vector3	scale = transform.localScale;
			scale.y = value;
			transform.localScale = scale;
		}
	}

	// 効果音再生
	public virtual void	PlaySound(SE.ID num, string seName, float delay = 0.0f)
	{
		AudioManager.Instance.PlaySE(num, seName, delay);
	}

	// 効果音再生チェック
	public virtual bool	CheckSound(SE.ID num)
	{
		return AudioManager.Instance.CheckSE(num);
	}

	// 自身から target の向きを得る
	public virtual float GetAngle(Vector2 target)
	{
		Vector2 a = target - xy;
		float ans = Mathf.Atan2 (a.y, a.x) * Mathf.Rad2Deg;
		if(ans < 0){ ans += 360;}
		else if(ans >= 360){ ans %= 360;}
		return ans;
	}
}

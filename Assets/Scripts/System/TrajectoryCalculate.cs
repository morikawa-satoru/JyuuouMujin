using UnityEngine;
using System.Collections;


// @takashicompany (http://takashicompany.com)


public static class TrajectoryCalculate
{

	// time ïbå„ÇÃç¿ïWÇãÅÇﬂÇÈ
	//	start : Start Position
	//	Force.(e.g. rigidbody.AddRelativeForce(force))
	//	Mass.(e.g. rigidbody.mass)
	//	Gravity.(e.g. Physics.gravity)
	//	Gravity scale.(e.g. rigidbody2D.gravityScale)
	//	Time.
	public static Vector3 Force(
		Vector3 start,
		Vector3 force,
		float mass,
		Vector3 gravity,
		float gravityScale,
		float time
	)
	{
		var speedX = force.x / mass * Time.fixedDeltaTime;
		var speedY = force.y / mass * Time.fixedDeltaTime;
		var speedZ = force.z / mass * Time.fixedDeltaTime;

		var halfGravityX = gravity.x * 0.5f * gravityScale;
		var halfGravityY = gravity.y * 0.5f * gravityScale;
		var halfGravityZ = gravity.z * 0.5f * gravityScale;

		var positionX = speedX * time + halfGravityX * Mathf.Pow(time, 2);
		var positionY = speedY * time + halfGravityY * Mathf.Pow(time, 2);
		var positionZ = speedZ * time + halfGravityZ * Mathf.Pow(time, 2);

		return start + new Vector3(positionX, positionY, positionZ);
	}

	// time ïbå„ÇÃç¿ïWÇãÅÇﬂÇÈ
	//	Start Position.
	//	Velocity.(e.g. rigidbody.velocity)
	//	Gravity.(e.g. Physics.gravity)
	//	Gravity scale.(e.g. rigidbody2D.gravityScale)
	//	Time.
	public static Vector3 Velocity(
		Vector3 start,
		Vector3 velocity,
		Vector3 gravity,
		float gravityScale,
		float time
	)
	{
		var halfGravityX = gravity.x * 0.5f * gravityScale;
		var halfGravityY = gravity.y * 0.5f * gravityScale;
		var halfGravityZ = gravity.z * 0.5f * gravityScale;

		var positionX = velocity.x * time + halfGravityX * Mathf.Pow(time, 2);
		var positionY = velocity.y * time + halfGravityY * Mathf.Pow(time, 2);
		var positionZ = velocity.z * time + halfGravityZ * Mathf.Pow(time, 2);

		return start + new Vector3(positionX, positionY, positionZ);
	}
}



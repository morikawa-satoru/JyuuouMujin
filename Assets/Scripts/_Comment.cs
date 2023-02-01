// _Comment.cs

using UnityEngine;


#if	UNITY_EDITOR

public class _Comment :	MonoBehaviour
{
	[SerializeField]
	[TextArea(5, 10)]
	public string commentsText;
}


#endif
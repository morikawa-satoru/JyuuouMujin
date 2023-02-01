using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawBox : MonoBehaviour
{


	private void OnDrawGizmos()
	{
		Transform box   = gameObject.transform;
		Vector2 boxSize = new Vector2(box.localScale.x, box.localScale.y) / 2f;

		DrawOverlapBox(box.position, boxSize, Color.green);
	}


	public void DrawOverlapBox(Vector2 point, Vector2 size, Color color)
    {
		size /= 2;

        // 4�̒��_�̈ʒu���擾
        Vector3 rightUp   = new Vector3(point.x + size.x, point.y + size.y, 0); // �E��
        Vector3 leftUp    = new Vector3(point.x - size.x, point.y + size.y, 0); // ����
        Vector3 leftDown  = new Vector3(point.x - size.x, point.y - size.y, 0); // ����
        Vector3 rightDown = new Vector3(point.x + size.x, point.y - size.y, 0); // �E��
        // �`��
        Debug.DrawLine(rightUp,   leftUp,   color);
        Debug.DrawLine(leftUp,    leftDown, color);
        Debug.DrawLine(leftDown,  rightDown,color);
        Debug.DrawLine(rightDown, rightUp,  color);
    }
}

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

        // 4つの頂点の位置を取得
        Vector3 rightUp   = new Vector3(point.x + size.x, point.y + size.y, 0); // 右上
        Vector3 leftUp    = new Vector3(point.x - size.x, point.y + size.y, 0); // 左上
        Vector3 leftDown  = new Vector3(point.x - size.x, point.y - size.y, 0); // 左下
        Vector3 rightDown = new Vector3(point.x + size.x, point.y - size.y, 0); // 右下
        // 描画
        Debug.DrawLine(rightUp,   leftUp,   color);
        Debug.DrawLine(leftUp,    leftDown, color);
        Debug.DrawLine(leftDown,  rightDown,color);
        Debug.DrawLine(rightDown, rightUp,  color);
    }
}

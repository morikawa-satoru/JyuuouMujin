// ShowComments.cs

using UnityEngine;
using UnityEditor;

public class _ShowComments : MonoBehaviour
{

    readonly static int xOffset = 100, yOffset = 18;


   [InitializeOnLoadMethod]
   private static void HierarchyExtensionInit()
   {
       EditorApplication.hierarchyWindowItemOnGUI += AddCommentsField;
   }

   // HierarchyにMyCommentsの内容を表示する
   private static void AddCommentsField(int instanceID, Rect rc)
   {
       var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
       if (gameObject != null)
       {
           var commComp = gameObject.GetComponent<_Comment>();
           if (commComp != null)
           {
               rc.xMin += rc.width  - xOffset;
               rc.yMin += rc.height - yOffset;
               commComp.commentsText = EditorGUI.TextField(rc, commComp.commentsText);
           }
       }
   }
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 选中当前物体下的所有二级子物体
/// 使用方法：选中父物体 → 菜单栏 Tools → Select Second Children
/// </summary>
public class SelectSecondChildren : EditorWindow
{
    [MenuItem("Tools/Select Second Children")]
    public static void SelectSecondChildrenFunc()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("请先在 Hierarchy 中选中一个物体！");
            return;
        }

        List<GameObject> secondChildren = new List<GameObject>();

        foreach (GameObject root in selectedObjects)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform firstChild = root.transform.GetChild(i);

                for (int j = 0; j < firstChild.childCount; j++)
                {
                    Transform secondChild = firstChild.GetChild(j);
                    secondChildren.Add(secondChild.gameObject);
                }
            }
        }

        if (secondChildren.Count > 0)
        {
            // 批量选中所有二级子物体
            Selection.objects = secondChildren.ToArray();
            Debug.Log($"✅ 已选中 {secondChildren.Count} 个二级子物体");

            // 让 Hierarchy 滚动到第一个选中的物体（传入单个对象，而非列表）
            //EditorGUIUtility.PingObject(secondChildren);
        }
        else
        {
            Debug.LogWarning("未找到任何二级子物体，请检查物体层级结构。");
        }
    }
}

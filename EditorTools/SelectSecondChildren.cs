using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 选中当前物体下的所有二级子物体，并可对选中物体的名称进行批量字符串替换。
/// 使用方法：选中父物体 → 菜单栏 Tools → Select Second Children
/// </summary>
public class SelectSecondChildren : EditorWindow
{
    // 新增：用于在Inspector窗口输入的查找和替换字符串
    private string _findString = "";
    private string _replaceString = "";

    [MenuItem("Tools/Select Second Children")]
    public static void SelectSecondChildrenFunc()
    {
        // 获取当前编辑器窗口实例，如果不存在则创建
        SelectSecondChildren window = (SelectSecondChildren)EditorWindow.GetWindow(typeof(SelectSecondChildren));
        window.titleContent = new GUIContent("批量选择与替换");
        window.Show();
    }

    // 新增：绘制编辑器窗口的GUI
    void OnGUI()
    {
        GUILayout.Label("批量选择二级子物体", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("1. 在Hierarchy中选中一个或多个父物体。\n2. 点击下方按钮以选中它们的所有二级子物体。\n3. （可选）对选中物体的名称进行批量替换。", MessageType.Info);

        // 显示查找和替换的输入框
        GUILayout.Space(10);
        GUILayout.Label("批量重命名选中物体（可选）", EditorStyles.boldLabel);
        _findString = EditorGUILayout.TextField("查找字符串：", _findString);
        _replaceString = EditorGUILayout.TextField("替换为：", _replaceString);

        EditorGUILayout.HelpBox("留空“查找字符串”则不会执行替换操作。", MessageType.None);

        GUILayout.Space(20);

        // 执行选择的按钮
        if (GUILayout.Button("执行选择（与替换）", GUILayout.Height(30)))
        {
            ExecuteSelectionAndRename();
        }
    }

    // 整合了选择与重命名逻辑的方法
    void ExecuteSelectionAndRename()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "请先在 Hierarchy 中选中一个或多个父物体！", "确定");
            Debug.LogWarning("请先在 Hierarchy 中选中一个物体！");
            return;
        }

        List<GameObject> secondChildren = new List<GameObject>();

        // 1. 收集所有二级子物体
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

        if (secondChildren.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "未找到任何二级子物体，请检查物体层级结构。", "确定");
            Debug.LogWarning("未找到任何二级子物体，请检查物体层级结构。");
            return;
        }

        // 2. 批量选中所有二级子物体
        Selection.objects = secondChildren.ToArray();
        Debug.Log($"✅ 已选中 {secondChildren.Count} 个二级子物体");

        // 3. 如果查找字符串不为空，则执行批量重命名
        if (!string.IsNullOrEmpty(_findString))
        {
            int renameCount = 0;
            Undo.RecordObjects(secondChildren.ToArray(), "批量重命名二级子物体");

            foreach (GameObject obj in secondChildren)
            {
                if (obj.name.Contains(_findString))
                {
                    obj.name = obj.name.Replace(_findString, _replaceString);
                    renameCount++;
                }
            }
            Debug.Log($"✅ 已对 {renameCount} 个选中物体的名称完成替换（将 '{_findString}' 替换为 '{_replaceString}'）。");
            // 刷新编辑器，使重命名立即在Hierarchy中可见
            EditorApplication.RepaintHierarchyWindow();
        }
        else
        {
            Debug.Log("ℹ️ 未指定查找字符串，跳过重命名步骤。");
        }
    }
}

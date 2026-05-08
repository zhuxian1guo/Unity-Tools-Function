using UnityEngine;
using UnityEditor;

/// <summary>
/// 移除选中物体所有二级子物体上的指定脚本
/// 使用方法：选中父物体 → 菜单栏 Tools → Remove Specific Script On Second Children
/// </summary>
public class RemoveSpecificScriptOnSecondChildren : EditorWindow
{
    // 在这里修改你要移除的脚本类名（区分大小写）
    private const string TARGET_SCRIPT_NAME = "HighlightEffect";

    [MenuItem("Tools/Remove Specific Script On Second Children")]
    public static void RemoveSpecificScript()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("请先在 Hierarchy 中选中一个物体！");
            return;
        }

        int totalRemoved = 0;

        foreach (GameObject root in selectedObjects)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform firstChild = root.transform.GetChild(i);

                for (int j = 0; j < firstChild.childCount; j++)
                {
                    GameObject secondChild = firstChild.GetChild(j).gameObject;
                    Component[] components = secondChild.GetComponents<Component>();

                    for (int k = components.Length - 1; k >= 0; k--)
                    {
                        if (components[k] != null &&
                            components[k].GetType().Name == TARGET_SCRIPT_NAME)
                        {
                            Undo.RecordObject(secondChild, "Remove Script");
                            DestroyImmediate(components[k]);
                            EditorUtility.SetDirty(secondChild);
                            totalRemoved++;
                            Debug.Log($"🗑️ {secondChild.name}：移除了 {TARGET_SCRIPT_NAME}");
                        }
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"🎉 完成！共移除 {totalRemoved} 个 {TARGET_SCRIPT_NAME} 脚本");
    }
}

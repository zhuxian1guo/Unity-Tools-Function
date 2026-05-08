using UnityEngine;
using UnityEditor;

/// <summary>
/// 移除选中物体所有二级子物体上的丢失脚本（Missing Script）
/// 使用方法：选中父物体 → 菜单栏 Tools → Remove Missing Scripts On Second Children
/// </summary>
public class RemoveMissingScriptsOnSecondChildren : EditorWindow
{
    [MenuItem("Tools/Remove Missing Scripts On Second Children")]
    public static void RemoveMissingScripts()
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
            // 遍历所有一级子物体
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform firstChild = root.transform.GetChild(i);

                // 遍历该一级子物体的所有子物体（即二级子物体）
                for (int j = 0; j < firstChild.childCount; j++)
                {
                    GameObject secondChild = firstChild.GetChild(j).gameObject;

                    // 获取该二级子物体上所有 MonoBehaviour
                    Component[] components = secondChild.GetComponents<MonoBehaviour>();
                    int removedCount = 0;

                    for (int k = components.Length - 1; k >= 0; k--)
                    {
                        if (components[k] == null) // 丢失的脚本
                        {
                            Undo.RecordObject(secondChild, "Remove Missing Scripts");
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(secondChild);
                            EditorUtility.SetDirty(secondChild);
                            removedCount++;
                        }
                    }

                    if (removedCount > 0)
                    {
                        Debug.Log($"✅ {secondChild.name}：移除了 {removedCount} 个丢失脚本");
                        totalRemoved += removedCount;
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"🎉 完成！共移除 {totalRemoved} 个丢失脚本");
    }
}

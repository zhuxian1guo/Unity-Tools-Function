
using DG.Tweening;
using UnityEngine;

public enum RenderingMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent
}

public class SetMaterialRenderingMode : MonoBehaviour
{
    public Material mat;
    public RenderingMode renderingMode;

    // 记录当前是否处于透明状态
    private bool isTransparent = false;

    // 保存原始颜色，以便恢复
    private Color originalColor;

    // 用于存储当前的 Tweener，以便在切换时杀死之前的动画，防止冲突
    private Tweener currentTweener;

    void Start()
    {
        if (mat != null)
        {
            // 保存初始颜色
            originalColor = mat.color;

            // 确保初始状态为不透明
            SetRenderingMode(mat, RenderingMode.Opaque);
            mat.color = originalColor;
        }
        else
        {
            Debug.LogWarning("Material is not assigned in Inspector.");
        }
    }


    public void ChangeColormode(Material mat1,bool isTransparent1) {
        mat = mat1;
        if (mat == null) return;

        // 如果当前有正在进行的动画，先停止它，避免状态混乱
        if (currentTweener != null && currentTweener.IsActive())
        {
            currentTweener.Kill();
        }

        // 切换状态
        isTransparent1 = !isTransparent1;

        if (isTransparent1)
        {
            // 1. 先切换到支持透明的渲染模式 (Fade)
            SetRenderingMode(mat, RenderingMode.Fade);

            // 2. 执行 Alpha 从当前值到 0.2f 的渐变
            // 注意：DOTween.To 需要直接操作 mat.color，或者使用自定义 setter
            currentTweener = DOTween.To(
                () => mat.color,
                x => mat.color = x,
                new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f),
                2f
            );
        }
        else
        {
            // 1. 执行 Alpha 从当前值到 1f 的渐变
            currentTweener = DOTween.To(
                () => mat.color,
                x => mat.color = x,
                new Color(mat.color.r, mat.color.g, mat.color.b, 1f),
                2f
            ).OnComplete(() => {
                // 2. 动画完成后，切换回不透明渲染模式 (Opaque)
                // 放在 OnComplete 中是为了避免在完全变不透明之前关闭混合模式导致视觉闪烁
                SetRenderingMode(mat, RenderingMode.Opaque);
            });
        }
    }



    /// <summary>
    /// 绑定到 Button 的 OnClick 事件
    /// </summary>
    public void OnButtonClick()
    {
        if (mat == null) return;

        // 如果当前有正在进行的动画，先停止它，避免状态混乱
        if (currentTweener != null && currentTweener.IsActive())
        {
            currentTweener.Kill();
        }

        // 切换状态
        isTransparent = !isTransparent;

        if (isTransparent)
        {
            // 1. 先切换到支持透明的渲染模式 (Fade)
            SetRenderingMode(mat, RenderingMode.Fade);

            // 2. 执行 Alpha 从当前值到 0.2f 的渐变
            // 注意：DOTween.To 需要直接操作 mat.color，或者使用自定义 setter
            currentTweener = DOTween.To(
                () => mat.color,
                x => mat.color = x,
                new Color(mat.color.r, mat.color.g, mat.color.b, 0.2f),
                2f
            );
        }
        else
        {
            // 1. 执行 Alpha 从当前值到 1f 的渐变
            currentTweener = DOTween.To(
                () => mat.color,
                x => mat.color = x,
                new Color(mat.color.r, mat.color.g, mat.color.b, 1f),
                2f
            ).OnComplete(() => {
                // 2. 动画完成后，切换回不透明渲染模式 (Opaque)
                // 放在 OnComplete 中是为了避免在完全变不透明之前关闭混合模式导致视觉闪烁
                SetRenderingMode(mat, RenderingMode.Opaque);
            });
        }
    }

    public void ChangeMode()
    {
        SetRenderingMode(mat, renderingMode);
    }

    /// <summary>
    /// 设置材质的渲染模式
    /// </summary>
    private void SetRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }


}

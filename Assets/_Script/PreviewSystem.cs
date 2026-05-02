using System;
using UnityEngine;

/// <summary>
/// 预览系统：负责显示建筑放置或拆除时的视觉反馈（鬼影和光标）。
/// </summary>
public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f; // 鬼影物体离地高度，防止深度冲突

    [SerializeField] private GameObject cellIndicator; // 地面光标物体（用于显示占地范围）
    private GameObject previewObject; // 建筑的“鬼影”物体

    [SerializeField] private Material previewMaterialPrefab; // 预览材质的原型（通常是半透明材质）
    private Material previewMaterialInstance; // 运行时创建的材质实例

    private Renderer cellIndicatorRenderer; // 光标的渲染组件

    private void Start()
    {
        // 创建材质实例，确保修改颜色不会影响原始资源
        previewMaterialInstance = new Material(previewMaterialPrefab);
        
        cellIndicator.SetActive(false); // 初始隐藏光标
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    /// <summary>
    /// 开始显示放置预览。
    /// </summary>
    /// <param name="prefab">要放置的建筑预制体</param>
    /// <param name="size">建筑占用的网格尺寸（如 2x2）</param>
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        // 1. 生成鬼影物体
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject); // 替换材质为半透明
        
        // 2. 调整光标大小以匹配建筑尺寸
        PrepareCursor(size);
        
        // 3. 显示光标
        cellIndicator.SetActive(true);
    }

    // 调整光标的大小和纹理缩放
    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            // 缩放光标物体
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            // 缩放贴图，防止纹理拉伸
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    // 将鬼影物体的所有材质替换为预览材质（半透明）
    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance; // 使用共享的预览材质
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject); // 销毁鬼影
    }

    /// <summary>
    /// 每一帧更新预览位置。
    /// </summary>
    /// <param name="position">世界坐标位置</param>
    /// <param name="validity">位置是否合法（true=绿/白，false=红）</param>
    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position); // 移动鬼影
            ApplyFeedbackToPreview(validity); // 更新鬼影颜色
        }
        
        MoveCursor(position); // 移动光标
        ApplyFeedbackToCursor(validity); // 更新光标颜色
    }

    private void MovePreview(Vector3 position)
    {
        // 增加 Y 轴偏移，防止与地面重叠闪烁
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }
    
    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    // 根据合法性改变鬼影颜色
    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red; // 合法为白，非法为红
        c.a = 0.5f; // 保持半透明
        previewMaterialInstance.color = c;
    }

    // 根据合法性改变光标颜色
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    /// <summary>
    /// 开始显示拆除预览。
    /// 拆除模式不需要显示建筑鬼影，只需要显示红色光标。
    /// </summary>
    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one); // 拆除通常只针对单个格子
        ApplyFeedbackToCursor(false); // 默认为红色，提示这是破坏性操作
    }
}
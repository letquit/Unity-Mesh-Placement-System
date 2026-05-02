using System; // 引入 System 命名空间以使用 Action 事件
using UnityEngine;
using UnityEngine.EventSystems; // 必须引入，用于处理 UI 事件系统

/// <summary>
/// 输入管理器：负责处理鼠标点击、键盘输入以及地图坐标的射线检测。
/// 采用旧版输入系统 (Input.GetAxis 等)。
/// </summary>
public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera; // 主摄像机引用，用于计算射线

    private Vector3 lastPosition; // 缓存上一次射线命中的有效坐标

    [SerializeField] private LayerMask placementLayerMask; // 放置层遮罩，只检测特定层（如地面）

    // 定义事件：当发生点击或按下退出键时通知订阅者
    public event Action OnClicked, OnExit;

    private void Update()
    {
        // 检测鼠标左键按下 (0 代表左键)
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke(); // 触发点击事件（如果由订阅者）

        // 检测 Esc 键按下
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke(); // 触发退出事件
    }

    /// <summary>
    /// 检查鼠标指针是否悬停在 Unity UI 元素上。
    /// 用于防止点击 UI 时误触游戏逻辑。
    /// </summary>
    /// <returns>如果在 UI 上返回 true，否则返回 false</returns>
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary>
    /// 获取鼠标在地图上的选中位置（世界坐标）。
    /// 使用射线检测将屏幕坐标转换为世界坐标。
    /// </summary>
    /// <returns>命中的 3D 坐标，若未命中则返回上一次有效坐标</returns>
    public Vector3 GetSelectedMapPosition()
    {
        // 获取鼠标在屏幕上的像素坐标
        Vector3 mousePos = Input.mousePosition;
        
        // 设置 Z 轴为近裁剪面距离，确保射线起点在摄像机前方
        mousePos.z = sceneCamera.nearClipPlane;
        
        // 从摄像机向鼠标位置发射射线
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        
        RaycastHit hit;
        // 执行射线检测
        // 100 是最大距离
        // placementLayerMask 确保只检测地面，忽略角色或建筑物
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            // 如果击中，更新缓存位置
            lastPosition = hit.point;
        }

        // 返回位置（无论是否击中，都返回一个有效值）
        return lastPosition;
    }
}
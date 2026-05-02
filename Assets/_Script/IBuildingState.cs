using UnityEngine;

/// <summary>
/// 建造状态接口：定义了所有建造模式（放置、拆除、升级等）必须遵循的契约。
/// 它是状态模式的核心，使得主控制器可以统一处理不同的建造行为。
/// </summary>
public interface IBuildingState
{
    /// <summary>
    /// 结束状态：用于清理资源和重置状态。
    /// 触发时机：当玩家按下 ESC 退出建造模式，或切换工具时调用。
    /// 职责：
    /// 1. 销毁预览物体（鬼影），防止残留。
    /// 2. 取消事件订阅。
    /// 3. 重置临时变量。
    /// </summary>
    void EndState();

    /// <summary>
    /// 执行操作：处理玩家的确认输入（如鼠标左键点击）。
    /// 触发时机：当玩家在地图上点击鼠标时调用。
    /// 职责：
    /// 1. 执行真正的业务逻辑（生成物体、销毁物体或修改数据）。
    /// 2. 更新 GridData 数据层。
    /// 3. 播放确认音效（如“放置成功”或“拆除成功”）。
    /// </summary>
    /// <param name="gridPosition">当前鼠标选中的网格坐标（世界坐标转换后的整数坐标）</param>
    void OnAction(Vector3Int gridPosition);

    /// <summary>
    /// 更新状态：处理每一帧的逻辑更新。
    /// 触发时机：在主控制器的 Update() 循环中被调用。
    /// 职责：
    /// 1. 移动预览物体（鬼影）到新的网格位置。
    /// 2. 计算当前位置是否合法（例如：显示绿色表示可建造，红色表示不可建造）。
    /// 3. 仅负责视觉反馈，不修改实际数据。
    /// </summary>
    /// <param name="gridPosition">当前鼠标所在的网格坐标</param>
    void UpdateState(Vector3Int gridPosition);
}
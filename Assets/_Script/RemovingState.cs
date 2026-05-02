using UnityEngine;

/// <summary>
/// 拆除状态：处理建筑系统中的拆除逻辑。
/// 实现了 IBuildingState 接口。
/// </summary>
public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1; // 用于存储要移除物体的索引
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData floorData; // 地板数据层
    private GridData furnitureData; // 家具数据层
    private ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;

    public RemovingState(Grid grid,
        PreviewSystem previewSystem,
        GridData floorData,
        GridData furnitureData,
        ObjectPlacer objectPlacer,
        SoundFeedback soundFeedback)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        // 初始化时，告诉预览系统显示“拆除模式”的视觉效果（例如红色高亮）
        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        // 状态结束时，停止显示预览
        previewSystem.StopShowingPreview();
    }

    /// <summary>
    /// 执行拆除操作（鼠标点击时调用）。
    /// </summary>
    /// <param name="gridPosition">网格坐标</param>
    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        
        // 检查家具层是否有物体
        // 注意：这里的逻辑假设 CanPlaceObjectAt 返回 false 表示“有物体，不能放置”
        if (furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }
        // 如果家具层没有，再检查地板层
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        // 如果两层都是空的，播放错误音效
        if (selectedData == null)
        {
            soundFeedback.PlaySound(SoundType.WrongPlacement);
        }
        else
        {
            soundFeedback.PlaySound(SoundType.Remove); // 播放拆除音效
            
            // 获取物体在 ObjectPlacer 中的索引
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1) return; // 安全检查
            
            // 1. 更新数据：从 GridData 中移除记录
            selectedData.RemoveObjectAt(gridPosition);
            // 2. 更新表现：从场景中移除物体
            objectPlacer.RemoveObject(gameObjectIndex);
        }

        // 更新预览位置
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    /// <summary>
    /// 检查当前鼠标位置是否有物体可拆。
    /// 如果家具或地板任意一个不为空，则返回 true（可拆）。
    /// </summary>
    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        // 注意：CanPlaceObjectAt 返回 false 意味着该位置被占用
        return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
                floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    /// <summary>
    /// 每一帧更新预览位置（移动“拆除光标”）。
    /// </summary>
    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
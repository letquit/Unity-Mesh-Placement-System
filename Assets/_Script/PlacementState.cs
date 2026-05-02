using System.Net.NetworkInformation; // 注意：这个引用似乎是多余的，通常不需要
using UnityEngine;

/// <summary>
/// 放置状态：处理建筑系统中的放置逻辑。
/// 实现了 IBuildingState 接口。
/// </summary>
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1; // 当前选中物体在数据库中的索引
    private int ID; // 物体的唯一标识符
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectsDatabaseSO database;
    private GridData floorData; // 地板数据层
    private GridData furnitureData; // 家具数据层
    private ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;

    public PlacementState(int id,
        Grid grid,
        PreviewSystem previewSystem,
        ObjectsDatabaseSO database,
        GridData floorData,
        GridData furnitureData,
        ObjectPlacer objectPlacer,
        SoundFeedback soundFeedback)
    {
        ID = id;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        
        // 在数据库中查找当前 ID 对应的物体索引
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        
        if (selectedObjectIndex > -1)
        {
            // 启动预览，传入预制体和尺寸
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
            // 如果 ID 无效，抛出异常，防止后续逻辑错误
            throw new System.Exception($"No object with ID {id}");
    }

    public void EndState()
    {
        // 状态结束时，停止显示预览
        previewSystem.StopShowingPreview();
    }

    /// <summary>
    /// 执行放置操作（鼠标点击时调用）。
    /// </summary>
    /// <param name="gridPosition">网格坐标</param>
    public void OnAction(Vector3Int gridPosition)
    {
        // 1. 检查放置是否合法（是否重叠、是否越界）
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        
        if (placementValidity == false)
        {
            soundFeedback.PlaySound(SoundType.WrongPlacement); // 播放错误音效
            return;
        }
        
        soundFeedback.PlaySound(SoundType.Place); // 播放放置音效
        
        // 2. 生成实际物体，并获取其索引
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
        
        // 3. 根据 ID 判断是地板还是家具，选择对应的数据层
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        
        // 4. 更新数据层：记录物体的位置、尺寸、ID 和物体索引
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID, index);
        
        // 5. 更新预览状态为“无效”，提示玩家该位置已被占用
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    /// <summary>
    /// 检查在指定位置放置当前物体是否合法。
    /// </summary>
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        // 选择对应的数据层（地板或家具）
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;

        // 调用数据层的方法检查是否可以放置
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    /// <summary>
    /// 每一帧更新预览位置（移动“鬼影”物体）。
    /// </summary>
    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        // 更新预览物体的位置和颜色（有效为绿，无效为红）
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
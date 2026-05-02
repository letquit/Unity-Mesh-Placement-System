using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网格数据：维护内存中的网格占用情况。
/// 它不管理 GameObject，只管理“格子是否被占用”的逻辑数据。
/// </summary>
public class GridData
{
    // 核心数据库：键是网格坐标，值是该格子所属的物体数据
    private Dictionary<Vector3Int, PlacementData> placedObjects = new();

    /// <summary>
    /// 在指定位置添加物体。
    /// 注意：这会占用物体尺寸范围内的所有格子。
    /// </summary>
    /// <param name="gridPosition">起始网格坐标（通常是左下角）</param>
    /// <param name="objectSize">物体尺寸（如 2x2）</param>
    /// <param name="ID">物体 ID（用于区分地板、墙壁等）</param>
    /// <param name="placedObjectIndex">在 ObjectPlacer 中的索引（用于后续销毁）</param>
    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        // 1. 计算这个物体覆盖了哪些格子（例如 2x2 的床覆盖 4 个格子）
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        
        // 2. 创建数据块，记录这个物体的信息
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        
        // 3. 将覆盖的每一个格子都指向这个数据块
        foreach (var pos in positionToOccupy)
        {
            // 安全检查：防止覆盖已有数据（这应该在上层逻辑被拦截）
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position in {pos}");
                
            placedObjects[pos] = data;
        }
    }

    // 计算物体占据的所有网格坐标
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        // 遍历物体的长和宽
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                // 将相对坐标加到世界网格坐标上
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    /// <summary>
    /// 检查在指定位置是否可以放置物体。
    /// 核心逻辑：只要物体覆盖的任意一个格子已被占用，就不能放置。
    /// </summary>
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionsToOccupy)
        {
            // 如果字典里已经有这个坐标，说明被占了
            if (placedObjects.ContainsKey(pos))
                return false;
        }

        return true; // 所有格子都是空的，可以放置
    }

    /// <summary>
    /// 获取该位置物体在 ObjectPlacer 中的索引。
    /// 用于在拆除时告诉 ObjectPlacer 销毁哪个 GameObject。
    /// </summary>
    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1; // 该位置没有物体
            
        // 返回该格子所属物体的索引
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    /// <summary>
    /// 移除物体。
    /// 关键点：通过点击任意一个格子，找到整个物体并清除其所有占地。
    /// </summary>
    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        // 1. 获取该格子所属的物体数据（包含该物体占用的所有坐标列表）
        // 2. 遍历并删除字典中属于该物体的所有坐标记录
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

/// <summary>
/// 放置数据：记录一个已放置物体的完整信息。
/// 它是“格子”与“物体”之间的纽带。
/// </summary>
public class PlacementData
{
    public List<Vector3Int> occupiedPositions; // 该物体占据的所有格子坐标列表
    public int ID { get; private set; }        // 物体 ID
    public int PlacedObjectIndex { get; private set; } // 在 ObjectPlacer 中的索引

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
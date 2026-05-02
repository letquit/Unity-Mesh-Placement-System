using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物体放置器：负责在场景中实例化和销毁实际的 GameObject。
/// 它是数据（GridData）与视觉表现（GameObject）之间的桥梁。
/// </summary>
public class ObjectPlacer : MonoBehaviour
{
    // 存储所有已生成物体的列表。
    // 注意：列表中的某些位置可能是 null（如果对应的物体被销毁了）。
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    /// <summary>
    /// 在指定位置生成一个预制体。
    /// </summary>
    /// <param name="prefab">要生成的预制体</param>
    /// <param name="position">世界坐标位置</param>
    /// <returns>返回该物体在列表中的索引，用于后续引用</returns>
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        // 1. 实例化预制体
        GameObject newObject = Instantiate(prefab);
        
        // 2. 设置位置
        newObject.transform.position = position;
        
        // 3. 加入列表进行追踪
        placedGameObjects.Add(newObject);
        
        // 4. 返回索引。这个索引会被 GridData 存储，作为该格子上物体的唯一标识
        return placedGameObjects.Count - 1;
    }

    /// <summary>
    /// 根据索引移除物体。
    /// </summary>
    /// <param name="gameObjectIndex">物体在列表中的索引</param>
    internal void RemoveObject(int gameObjectIndex)
    {
        // 安全检查：防止索引越界或物体已经被销毁
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
            return;
            
        // 1. 销毁场景中的 GameObject
        Destroy(placedGameObjects[gameObjectIndex]);
        
        // 2. 将列表中的引用置空。
        // 注意：这里不使用 RemoveAt，是为了保持其他物体的索引不变，
        // 确保 GridData 中存储的索引依然有效。
        placedGameObjects[gameObjectIndex] = null;
    }
}
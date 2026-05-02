using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物体数据库：使用 ScriptableObject 存储所有可建造物体的配置数据。
/// 这是一个独立的资源文件，可以在 Unity 编辑器中直接配置。
/// </summary>
[CreateAssetMenu] // 允许在右键菜单中创建此资源
public class ObjectsDatabaseSO : ScriptableObject
{
    // 存储所有物体数据的列表。
    // 在 Inspector 中，你可以看到所有 ObjectData 的折叠列表，方便管理。
    public List<ObjectData> objectsData;
}

/// <summary>
/// 单个物体的数据结构：定义了建造系统中一个物体的所有必要信息。
/// </summary>
[Serializable] // 使得这个类可以在 Inspector 面板中折叠显示
public class ObjectData
{
    // 物体名称（仅用于编辑器备注，方便策划识别）
    [field: SerializeField] // 强制序列化属性，使其在 Inspector 中可见
    public string Name { get; private set; }
    
    // 物体的唯一标识符。
    // 逻辑代码（如 PlacementState）通过 ID 来查找对应的物体。
    // 注意：ID 为 0 通常被约定为“地板”。
    [field: SerializeField]
    public int ID { get; private set; }
    
    // 物体占用的网格尺寸（例如 1x1, 2x2）。
    // 默认为 Vector2Int.one (1x1)。
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;
    
    // 实际要生成的预制体引用。
    // 这是连接“数据”与“场景表现”的桥梁。
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
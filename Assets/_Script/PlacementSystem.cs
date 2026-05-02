using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 放置系统：建造模式的核心控制器。
/// 负责协调输入、网格数据、预览显示和状态切换。
/// </summary>
public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager; // 输入管理器，提供鼠标事件和坐标
    [SerializeField] private Grid grid; // Unity 的 Grid 组件，用于坐标转换

    [SerializeField] private ObjectsDatabaseSO database; // 物品数据库，存储所有可建造物体的信息

    [SerializeField] private GameObject gridVisualization; // 网格可视化物体（如高亮显示的格子）

    // 数据存储：分离地板和家具数据，支持分层建造逻辑
    private GridData floorData;
    private GridData furnitureData;

    [SerializeField] private PreviewSystem preview; // 预览系统，负责显示“鬼影”物体

    private Vector3Int lastDetectedPosition = Vector3Int.zero; // 缓存上一帧的网格位置，用于优化

    [SerializeField] private ObjectPlacer objectPlacer; // 物体放置器，负责实际的实例化

    private IBuildingState buildingState; // 当前状态（放置或拆除）
    
    [SerializeField] private SoundFeedback soundFeedback; // 音效反馈

    private void Start()
    {
        StopPlacement(); // 初始化时确保处于停止状态
        floorData = new(); // 初始化地板数据层
        furnitureData = new(); // 初始化家具数据层
        gridVisualization.SetActive(false); // 初始隐藏网格
    }

    /// <summary>
    /// 开始放置模式。
    /// </summary>
    /// <param name="ID">要放置的物品 ID</param>
    public void StartPlacement(int ID)
    {
        bool wasActive = buildingState != null; 
        
        StopPlacement(); // 先停止当前任何正在进行的模式
        
        if (!wasActive)
        {
            soundFeedback.PlaySound(SoundType.Click); // 播放进入模式的音效
        }
        
        gridVisualization.SetActive(true); // 显示网格
        // 实例化“放置状态”，传入所需依赖
        buildingState = new PlacementState(ID, grid, preview, database, floorData, furnitureData, objectPlacer, soundFeedback);
        
        // 订阅输入事件
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    /// <summary>
    /// 开始拆除模式。
    /// </summary>
    public void StartRemoving()
    {
        bool wasActive = buildingState != null; 
        
        StopPlacement();
        
        if (!wasActive)
        {
            soundFeedback.PlaySound(SoundType.Click);
        }
        
        gridVisualization.SetActive(true);
        // 实例化“拆除状态”
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer, soundFeedback);
        
        // 订阅输入事件
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    /// <summary>
    /// 处理点击放置/拆除逻辑。
    /// </summary>
    private void PlaceStructure()
    {
        // 防止点击 UI 时误操作
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        
        // 获取鼠标位置并转换为网格坐标
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // 委托给当前状态处理具体逻辑
        buildingState.OnAction(gridPosition);
    }

    /// <summary>
    /// 停止当前的建造/拆除模式。
    /// </summary>
    private void StopPlacement()
    {
        if (buildingState == null) 
            return;
        
        soundFeedback.PlaySound(SoundType.Click); // 播放退出音效
        
        gridVisualization.SetActive(false); // 隐藏网格
        
        if (buildingState != null)
        {
            buildingState.EndState(); // 通知状态类清理资源（如销毁预览物体）
        }
        
        // 取消事件订阅，防止内存泄漏和逻辑错误
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        
        lastDetectedPosition = Vector3Int.zero; // 重置缓存
        buildingState = null; // 清空状态
    }

    private void Update()
    {
        if (buildingState == null)
            return;
            
        // 获取当前鼠标所在的网格位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        // 只有当网格位置发生变化时，才更新预览（性能优化）
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition); // 更新“鬼影”位置或颜色
            lastDetectedPosition = gridPosition;
        }
    }
}
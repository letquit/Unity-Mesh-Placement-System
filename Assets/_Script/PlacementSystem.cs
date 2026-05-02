using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;

    [SerializeField] private GameObject gridVisualization;

    private GridData floorData;
    private GridData furnitureData;

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    private IBuildingState buildingState;
    
    [SerializeField] private SoundFeedback soundFeedback;
    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        gridVisualization.SetActive(false);
    }

    public void StartPlacement(int ID)
    {
        bool wasActive = buildingState != null; 
        
        StopPlacement();
        
        if (!wasActive)
        {
            soundFeedback.PlaySound(SoundType.Click);
        }
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, grid, preview, database, floorData, furnitureData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        bool wasActive = buildingState != null; 
        
        StopPlacement();
        
        if (!wasActive)
        {
            soundFeedback.PlaySound(SoundType.Click);
        }
        
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    private void StopPlacement()
    {
        if (buildingState == null) 
            return;
        
        soundFeedback.PlaySound(SoundType.Click);
        
        gridVisualization.SetActive(false);
        if (buildingState != null)
        {
            buildingState.EndState();
        }
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}

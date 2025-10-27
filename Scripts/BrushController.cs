using Unity.Mathematics;
using UnityEngine;

public enum BrushMode
{
    // Different brush modes
    Select,
    Pan,
    Text,
    Fill,
    Single,
}

public class BrushController : MonoBehaviour
{
    // Reference to BuildingCreator script
    public BuildingCreator buildCreator;

    // Brush settings
    [Header("Rows and Columns")]
    public int maxRows = 20;
    public int maxCol = 20;

    // Brush mode
    [Header("Brush Mode")]
    public BrushMode brushMode = BrushMode.Single;
    public BuildingObject eraser;

    // Is the user currently adjusting the brush size?
    public bool isAdjustingBrush = false;

    // Anchor position for brush resizing
    private Vector3Int anchorGridPos;

    private void Update()
    {
        // Handle mouse input
        HandleMouseInput();

        // Update brush size if adjusting
        if (isAdjustingBrush)
            UpdateBrushSize();
    }

    private void HandleMouseInput() // my nightmare
    {
        switch (brushMode) // Check brush mode
        {
            // Handle each mode accordingly
            case BrushMode.Single: 
                HandleSingleMode();
                break;

            case BrushMode.Fill:
                HandleFillMode();
                break;

            case BrushMode.Select:
                if (Input.GetMouseButtonDown(0))
                {

                }
                if (Input.GetMouseButtonUp(0))
                {

                }
                break;

            case BrushMode.Pan:
                if (Input.GetMouseButtonDown(0))
                {

                }

                if (Input.GetMouseButtonUp(0))
                {

                }
                break;
            case BrushMode.Text:
                if (Input.GetMouseButtonDown(0))
                {

                }

                if (Input.GetMouseButtonUp(0))
                {

                }
                break;
        }
    }


    // Function for Single mode
    private void HandleSingleMode()
    {
        // Right click to adjust brush size
        if (Input.GetMouseButtonDown(1))
            StartBrush();
        
        // Release right click to finalize brush size
        if (Input.GetMouseButtonUp(1))
        {
            NormalizeBrushSize(); // Ensure positive sizes
            EndBrush(); // End the brush
        }
    }

    // Function for Fill mode
    private void HandleFillMode()
    {
        // Left click to start selecting size
        if (Input.GetMouseButtonDown(0))
        {
            StartBrush();
        }

        // Release left click to fill area
        if (Input.GetMouseButtonUp(0))
        {
            Fill();        // place tiles here
            ResetBrushSize(0,0); // reset brush size
            EndBrush();    // clear preview and stop dragging
        }
    }

    private void HandleSelectMode()
    {

    }

    private void HandlePanMode()
    {

    }

    private void HandleTextMode()
    {

    }

    // Reset brush size to specific dimensions (x, y)
    public void ResetBrushSize(int x, int y)
    {
        buildCreator.brushColumns = x;
        buildCreator.brushRows = y;

        buildCreator.UpdatePreview();
    }


    private void StartBrush()
    {
        // Reset to 1x1
        buildCreator.brushColumns = 1;
        buildCreator.brushRows = 1;

        isAdjustingBrush = true; // Set adjusting brush to true

        // Set anchor position to current mouse grid position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Converts world position to grid position for the preview map
        anchorGridPos = buildCreator.previewMap.WorldToCell(worldPos);

        // Enable anchor preview mode boolean
        buildCreator.useAnchorPreview = true;
        // Set anchor position in BuildingCreator
        buildCreator.anchorGridPosition = anchorGridPos; 

        // Update preview
        buildCreator.UpdatePreview();
    }

    // End brush
    public void EndBrush()
    {
        isAdjustingBrush = false;
        buildCreator.useAnchorPreview = false; 
        buildCreator.ClearPreview(); 
    }

    // Update brush size based on mouse position
    private void UpdateBrushSize()
    {
        // Get world position from mouse and the current grid position from world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentGridPos = buildCreator.previewMap.WorldToCell(worldPos);

        // Calculate delta from anchor position
        int deltaX = currentGridPos.x - anchorGridPos.x;
        int deltaY = anchorGridPos.y - currentGridPos.y; // flipped Y so up is up and down is down

        // Clamp brush size 
        buildCreator.brushColumns = Mathf.Clamp(deltaX + (deltaX >= 0 ? 1 : -1), -maxCol, maxCol);
        buildCreator.brushRows = Mathf.Clamp(deltaY + (deltaY >= 0 ? 1 : -1), -maxRows, maxRows);

        // Update preview based on brush mode
        if (brushMode == BrushMode.Fill)
        {
            // Clear preview first
            buildCreator.ClearPreview();

            int cols = Mathf.Abs(buildCreator.brushColumns);
            int rows = Mathf.Abs(buildCreator.brushRows);

            // Draw preview tiles only
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector3Int previewPos = new Vector3Int(
                        anchorGridPos.x + (buildCreator.brushColumns >= 0 ? x : -x),
                        anchorGridPos.y - (buildCreator.brushRows >= 0 ? y : -y),
                        0
                    );

                    buildCreator.previewMap.SetTile(previewPos, buildCreator.cursorTile); // only preview
                }
            }
        }
        else
        {
            // Single brush mode
            buildCreator.UpdatePreview();
        }
    }

    public void setCurrentTile(BuildingObject obj)
    {
        buildCreator.ObjectSelected(obj);
    }

    private void NormalizeBrushSize()
    {
        // Ensure brush sizes are positive using absolute values
        buildCreator.brushColumns = Mathf.Abs(buildCreator.brushColumns);
        buildCreator.brushRows = Mathf.Abs(buildCreator.brushRows);
    }

    public void Fill()
    {
        int cols = buildCreator.brushColumns;
        int rows = buildCreator.brushRows;
        int absCols = Mathf.Abs(cols);
        int absRows = Mathf.Abs(rows);
        for (int x = 0; x < absCols; x++)
        {
            for (int y = 0; y < absRows; y++)
            {
                Vector3Int tilePos = new Vector3Int(
                    anchorGridPos.x + (cols >= 0 ? x : -x),
                    anchorGridPos.y - (rows >= 0 ? y : -y),
                    0
                );

                buildCreator.PlaceTile(tilePos);
            }
        }
        buildCreator.ClearPreview(); // remove preview after placement
    }
}
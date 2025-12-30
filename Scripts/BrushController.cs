using UnityEngine;
using Unity.Mathematics;
public enum BrushMode
{
    Select,
    Pan,
    Text,
    Fill,
    Single,
}

public class BrushController : MonoBehaviour
{
    public BuildingCreator buildCreator;

    [Header("Brush Limits")]
    public int maxRows = 20;
    public int maxCol = 20;

    [Header("Brush Mode")]
    public BrushMode brushMode = BrushMode.Single;
    public BuildingObject eraser;
    public Camera cam;

    bool isAdjustingBrush = false;
    private Vector3Int anchorGridPos;
    private Vector3 lastMousePos;

    private void Update()
    {
        HandleMouseInput();

        if (isAdjustingBrush)
            UpdateBrushSize();
    }

    private void HandleMouseInput()
    {
        switch (brushMode)
        {
            case BrushMode.Single:
                HandleSingleMode();
                break;

            case BrushMode.Fill:
                HandleFillMode();
                break;

            case BrushMode.Pan:
                HandlePanMode();
                break;
        }
    }

    private void HandleSingleMode()
    {
        if (Input.GetMouseButtonDown(1))
            StartBrush();

        if (Input.GetMouseButtonUp(1))
        {
            NormalizeBrushSize();
            EndBrush();
        }
    }

    private void HandleFillMode()
    {
        if (Input.GetMouseButtonDown(0))
            StartBrush();

        if (Input.GetMouseButtonUp(0))
        {
            Fill();
            ResetBrushSize(0, 0);
            EndBrush();
        }
    }

    private void HandlePanMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = lastMousePos - currentMousePosition;

            cam.transform.position += difference;
            lastMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void StartBrush()
    {
        buildCreator.brushColumns = 1;
        buildCreator.brushRows = 1;

        isAdjustingBrush = true;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        anchorGridPos = buildCreator.previewMap.WorldToCell(worldPos);

        buildCreator.useAnchorPreview = true;
        buildCreator.anchorGridPosition = anchorGridPos;

        buildCreator.UpdatePreview();
    }

    public void EndBrush()
    {
        isAdjustingBrush = false;
        buildCreator.useAnchorPreview = false;
        buildCreator.ClearPreview();
    }

    void UpdateBrushSize()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentGridPos = buildCreator.previewMap.WorldToCell(worldPos);

        int deltaX = currentGridPos.x - anchorGridPos.x;
        int deltaY = anchorGridPos.y - currentGridPos.y;

        buildCreator.brushColumns = (int)Mathf.Clamp(deltaX + Mathf.Sign(deltaX), -maxCol, maxCol);
        buildCreator.brushRows = (int)Mathf.Clamp(deltaY + Mathf.Sign(deltaY), -maxRows, maxRows);

        buildCreator.UpdatePreview();
    }

    void NormalizeBrushSize()
    {
        buildCreator.brushColumns = Mathf.Abs(buildCreator.brushColumns);
        buildCreator.brushRows = Mathf.Abs(buildCreator.brushRows);
    }

    public void ResetBrushSize(int x, int y)
    {
        buildCreator.brushColumns = x;
        buildCreator.brushRows = y;
        buildCreator.UpdatePreview();
    }

    public void setCurrentTile(BuildingObject obj)
    {
        buildCreator.ObjectSelected(obj);
    }

    public void Fill()
    {
        foreach (var pos in buildCreator.GetBrushArea())
            buildCreator.PlaceTile(pos);

        buildCreator.ClearPreview();
    }
}
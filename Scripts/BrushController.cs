using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Tilemaps;
public enum BrushMode
{
    Select,
    Pan,
    Text,
    TextEdit,
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
    public TextObject textObject;
    private TextObject activeTextObject;
    public Canvas textCanvas;
    
    public Tilemap tilemap;
    public GameObject editTile;
    public TileEdit tileEdit;

    bool isAdjustingBrush = false;
    bool isClicking = false;
    private Vector3Int anchorGridPos;
    private Vector3 lastMousePos;

    private void Update()
    {
        tilemap = buildCreator.defaultMap;
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
                buildCreator.ClearSelectedTile();
                HandlePanMode();
                break;

            case BrushMode.Text:
                buildCreator.ClearSelectedTile();
                HandleTextMode();
                break;

            case BrushMode.TextEdit:
                HandleTextEditMode();
                break;

            case BrushMode.Select:
                buildCreator.ClearSelectedTile();
                HandleSelectMode();
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

    private void HandleTextMode()
    {
        // Create text object
        if (Input.GetMouseButtonUp(0) && activeTextObject == null)
        {
            activeTextObject = Instantiate(textObject, textCanvas.transform);
        }

        if (activeTextObject != null)
        {
            RectTransform rectTransform = activeTextObject.GetComponent<RectTransform>();

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                textCanvas.transform as RectTransform,
                Input.mousePosition,
                textCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : textCanvas.worldCamera,
                out localPoint
            );

            rectTransform.anchoredPosition = localPoint;

            // Final placement
            if (Input.GetMouseButtonDown(0))
            {
                activeTextObject = null;
                brushMode = BrushMode.TextEdit;
            }

            // Cancel placement with right click
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(activeTextObject.gameObject);
                activeTextObject = null;
            }
        }
    }

    private void HandleTextEditMode()
    {
        CameraMovement camMove = cam.GetComponent<CameraMovement>();
        camMove.enabled = false;

        if(Input.GetKeyUp(KeyCode.Return))
        {
            camMove.enabled = true;
            brushMode = BrushMode.Pan;
        }
    }


    private void HandleSelectMode()
    {
        // Mouse button pressed
        if (Input.GetMouseButtonUp(0))
        {
            if (editTile.activeSelf) return;

            isClicking = true;
        }

        // Mouse button released
        if (Input.GetMouseButtonDown(0) && isClicking)
        {
            isClicking = false;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if clicking on Visual
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            
            if (hit.collider != null && hit.collider.CompareTag("Tile_Visual"))
            {
                tileEdit.OpenEditor();
                Vector3 visualWorldPos = hit.transform.position;
                
                editTile.transform.position = new Vector3(
                    visualWorldPos.x,
                    visualWorldPos.y,
                    -0.02f
                );

                editTile.transform.localScale = hit.transform.localScale;
                tileEdit.visualInstance = hit.transform.gameObject;

                return;
            }


            // Clicking on set Tile
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);

            if (!tilemap.HasTile(cellPos)) return;

            Vector3 tileWorldPos = tilemap.GetCellCenterWorld(cellPos);
            editTile.transform.position = new Vector3(
                tileWorldPos.x,
                tileWorldPos.y,
                -0.02f
            );

            editTile.SetActive(true);
            tileEdit.AssignTile(tilemap, cellPos);
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
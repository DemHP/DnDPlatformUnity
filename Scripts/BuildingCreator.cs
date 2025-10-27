using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.WSA;
using UnityEngine.EventSystems;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] public Tilemap previewMap, defaultMap;
    [SerializeField] public TileBase cursorTile; // assign in Inspector

    [SerializeField] public int brushRows = 1;     // number of rows (signed allowed)
    [SerializeField] public int brushColumns = 1;  // number of cols (signed allowed)

    public BrushController brushSize;

    PlayerInput playerInput;
    TileBase tileBase;
    BuildingObject selectedObj;

    Camera cam;

    Vector2 mousePos;
    public Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    bool isDrawing = false; // drawing left click

    // Anchor mode used by BrushSizeController when resizing the brush (right-drag)
    [HideInInspector] public bool useAnchorPreview = false;
    [HideInInspector] public Vector3Int anchorGridPosition;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        cam = Camera.main;
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.Gameplay.MousePosition.performed += OnMouseMove;
        playerInput.Gameplay.MouseLeftClick.started += OnLeftClickDown;
        playerInput.Gameplay.MouseLeftClick.canceled += OnLeftClickUp;
        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Gameplay.MousePosition.performed -= OnMouseMove;
        playerInput.Gameplay.MouseLeftClick.started -= OnLeftClickDown;
        playerInput.Gameplay.MouseLeftClick.canceled -= OnLeftClickUp;
        playerInput.Gameplay.MouseRightClick.performed -= OnRightClick;
    }

    private BuildingObject SelectedObj
    {
        set
        {
            selectedObj = value;
            tileBase = selectedObj != null ? selectedObj.TileBase : null;
            UpdatePreview();
        }
    }

    private void Update()
    {
        // Convert mouse to grid directly
        Vector3 screen = new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane);
        Vector3 worldPos = cam.ScreenToWorldPoint(screen);
        Vector3Int gridPos = previewMap.WorldToCell(worldPos);



        if (gridPos != currentGridPosition)
        {
            lastGridPosition = currentGridPosition;
            currentGridPosition = gridPos;

            if (selectedObj != null || useAnchorPreview)
                UpdatePreview();
        }

        if (isDrawing && selectedObj != null && brushSize.brushMode == BrushMode.Single)
            DrawItem();
    }

    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClickDown(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (selectedObj != null)
        {
            isDrawing = true;
            DrawItem(); // immediate
        }
    }

    private void OnLeftClickUp(InputAction.CallbackContext ctx)
    {
        isDrawing = false;
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        SelectedObj = null; // deselect
    }

    public void ObjectSelected(BuildingObject obj)
    {
        SelectedObj = obj;
    }


    public bool HasSelectedObject()
    {
        return selectedObj != null && tileBase != null;
    }

    public IEnumerable<Vector3Int> GetBrushArea()
    {
        int cols = Mathf.Abs(brushColumns);
        int rows = Mathf.Abs(brushRows);
        if (cols == 0 || rows == 0) yield break;

        int dirX = brushColumns >= 0 ? 1 : -1;
        int dirY = brushRows >= 0 ? -1 : 1; // note flipped Y

        if (useAnchorPreview)
        {
            // Anchor-based expansion
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    yield return new Vector3Int(
                        anchorGridPosition.x + x * dirX,
                        anchorGridPosition.y + y * dirY,
                        anchorGridPosition.z
                    );
                }
            }
        }
        else
        {
            // Centered brush
            int startX = currentGridPosition.x - (cols - 1) / 2;
            int startY = currentGridPosition.y - (rows - 1) / 2;

            if (dirX < 0) startX = currentGridPosition.x + (cols - 1) / 2;
            if (dirY < 0) startY = currentGridPosition.y + (rows - 1) / 2;

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int offsetX = dirX >= 0 ? x : -x;
                    int offsetY = dirY >= 0 ? y : -y;

                    yield return new Vector3Int(
                        startX + offsetX,
                        startY + offsetY,
                        currentGridPosition.z
                    );
                }
            }
        }
    }

    public void UpdatePreview()
    {
        previewMap.ClearAllTiles();
        TileBase previewTile = tileBase != null ? tileBase : cursorTile;
        if (previewTile == null) return;

        foreach (var pos in GetBrushArea())
            previewMap.SetTile(pos, previewTile);
    }

    public void ClearPreview()
    {
        previewMap.ClearAllTiles();
    }

    public void DrawItem()
    {
        if (tileBase == null) return;

        foreach (var pos in GetBrushArea())
            defaultMap.SetTile(pos, tileBase);
    }

    public void PlaceTile(Vector3Int position)
    {
        // Set the tile at the given grid position
        defaultMap.SetTile(position, tileBase);
    }
}

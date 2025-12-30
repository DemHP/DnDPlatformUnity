using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [Header("Tilemaps")]
    [SerializeField] public Tilemap previewMap;
    [SerializeField] public Tilemap defaultMap;

    [Header("Tiles")]
    [SerializeField] public TileBase cursorTile;

    [Header("Brush Size")]
    [SerializeField] public int brushRows = 1;
    [SerializeField] public int brushColumns = 1;

    public BrushController brushSize;

    PlayerInput playerInput;
    Camera cam;

    TileBase tileBase;
    BuildingObject selectedObj;

    Vector2 mousePos;
    public Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    bool isDrawing = false;

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

    private void Update()
    {
        UpdateMouseGridPosition();

        if (isDrawing && selectedObj != null && brushSize.brushMode == BrushMode.Single)
            DrawItem();
    }



    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClickDown(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (selectedObj != null)
        {
            isDrawing = true;
            DrawItem();
        }
    }

    private void OnLeftClickUp(InputAction.CallbackContext ctx)
    {
        isDrawing = false;
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        SelectedObj = null;
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

    public void ObjectSelected(BuildingObject obj)
    {
        SelectedObj = obj;
    }

    public bool HasSelectedObject()
    {
        return selectedObj != null && tileBase != null;
    }



    void UpdateMouseGridPosition()
    {
        Vector3 screen = new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane);
        Vector3 worldPos = cam.ScreenToWorldPoint(screen);
        Vector3Int gridPos = previewMap.WorldToCell(worldPos);

        if (gridPos == currentGridPosition) return;

        lastGridPosition = currentGridPosition;
        currentGridPosition = gridPos;

        if (selectedObj != null || useAnchorPreview)
            UpdatePreview();
    }

    public IEnumerable<Vector3Int> GetBrushArea()
    {
        int cols = Mathf.Abs(brushColumns);
        int rows = Mathf.Abs(brushRows);
        if (cols == 0 || rows == 0) yield break;

        int dirX = brushColumns >= 0 ? 1 : -1;
        int dirY = brushRows >= 0 ? -1 : 1;

        Vector3Int origin = useAnchorPreview ? anchorGridPosition : currentGridPosition;

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                yield return new Vector3Int(
                    origin.x + x * dirX,
                    origin.y + y * dirY,
                    origin.z
                );
            }
        }
    }



    public void UpdatePreview()
    {
        if(brushSize.brushMode != BrushMode.Single && 
            brushSize.brushMode != BrushMode.Fill)
        {
            ClearPreview();
            return;
        }
        
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
        defaultMap.SetTile(position, tileBase);
    }

}
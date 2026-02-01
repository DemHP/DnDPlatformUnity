using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEdit : MonoBehaviour
{
    [Header("Tile Data")]
    public Tilemap tilemap;
    public Vector3Int cellPosition;
    public TileBase currentTile;

    [Header("Visuals")]
    public GameObject tileVisualPrefab;
    public Transform grid;
    public GameObject visualInstance;

    [Header("Buttons")]
    public GameObject increaseButton;
    public GameObject decreaseButton;
    public GameObject exitButton;
    public GameObject deleteButton;
    
    private SpriteRenderer spriteRenderer;

    private Vector3 startingScale;
    private bool visualSetUp;

    private const float MinScale = 0.1f;

    void Start()
    {
        startingScale = transform.localScale;
        SetupButtonCallbacks();
        CloseEditor();
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            CloseEditor();
        }
    }

    public void AssignTile(Tilemap map, Vector3Int cellPos)
    {
        CloseEditor();

        tilemap = map;
        cellPosition = cellPos;
        currentTile = tilemap.GetTile(cellPos);

        transform.localScale = startingScale;
        visualSetUp = false;

        OpenEditor();
    }

    private void SetUpVisual()
    {
        if (tilemap == null || tileVisualPrefab == null || grid == null)
        {
            Debug.LogWarning("TileEdit: Missing required references.");
            return;
        }

        if (currentTile is not Tile tileWithSprite || tileWithSprite.sprite == null)
        {
            Debug.LogWarning("Tile has no sprite.");
            return;
        }

        visualSetUp = true;

        // Instantiate
        visualInstance = Instantiate(tileVisualPrefab, grid);
        spriteRenderer = visualInstance.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = tileWithSprite.sprite;
        visualInstance.GetComponent<TileVisualData>().ChangeTileData(currentTile);

        // Position it at the tile's world position
        Vector3 worldPos = tilemap.GetCellCenterWorld(cellPosition);
        visualInstance.transform.position = worldPos;

        // Reset scale after parenting
        visualInstance.transform.localScale = Vector3.one;
    }

    private void SetupButtonCallbacks()
    {
        if (increaseButton)
            increaseButton.GetComponent<ButtonHandler>().onClick = () => ChangeTileSize(1f);

        if (decreaseButton)
            decreaseButton.GetComponent<ButtonHandler>().onClick = () => ChangeTileSize(-1f);

        if (exitButton)
            exitButton.GetComponent<ButtonHandler>().onClick = CloseEditor;

        if (deleteButton)
            deleteButton.GetComponent<ButtonHandler>().onClick = DeleteVisual;
    }

    public void ChangeTileSize(float amount)
    {
        if (!visualSetUp)
            SetUpVisual();

        if (!visualInstance)
            return;

        Vector3 newScale = visualInstance.transform.localScale + Vector3.one * amount;
        newScale = Vector3.Max(newScale, Vector3.one * MinScale);

        visualInstance.transform.localScale = newScale;

        transform.localScale = newScale;
    }

    public void DeleteVisual()
    {
        CleanupVisual();
        CloseEditor();
    }

    private void CleanupVisual()
    {
        if (visualInstance)
            Destroy(visualInstance);

        visualInstance = null;
        spriteRenderer = null;
        visualSetUp = false;
    }

    public void OpenEditor()
    {
        gameObject.SetActive(true);
    }

    public void CloseEditor()
    {
        gameObject.SetActive(false);
    }
}
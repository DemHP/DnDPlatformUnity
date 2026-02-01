using UnityEngine;
using UnityEngine.Tilemaps;

public class TileVisualData : MonoBehaviour
{
    public TileBase visualTileData;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeTileData(TileBase tile)
    {
        visualTileData = tile;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (tile is Tile tileWithSprite)
        {
            spriteRenderer.sprite = tileWithSprite.sprite;
        }
        else
        {
            Debug.LogWarning($"Tile {tile.name} has no sprite.");
            spriteRenderer.sprite = null;
        }
    }
}
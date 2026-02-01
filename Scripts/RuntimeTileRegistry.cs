using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class RuntimeTileRegistry
{
    private static readonly Dictionary<string, TileBase> tiles = new();

    public static void Register(string tileId, TileBase tile)
    {
        tiles[tileId] = tile;
    }

    public static TileBase Get(string tileId)
    {
        tiles.TryGetValue(tileId, out TileBase tile);
        return tile;
    }

    public static bool Has(string tileId)
    {
        return tiles.ContainsKey(tileId);
    }

    public static void Clear()
    {
        tiles.Clear();
    }
}
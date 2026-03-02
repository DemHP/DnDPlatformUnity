using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RuntimeArtRegistry
{
    private static readonly Dictionary<string, TileBase> arts = new();
    
    public static void Register(string artId, TileBase art)
    {
        arts[artId] = art;
    }

    public static TileBase Get(string artId)
    {
        arts.TryGetValue(artId, out TileBase art);
        return art;
    }

    public static bool Has(string artId)
    {
        return arts.ContainsKey(artId);
    }

    public static void Clear()
    {
        arts.Clear();
    }
}

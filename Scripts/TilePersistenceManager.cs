using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TilePersistenceManager : MonoBehaviour
{

    public string type = "";

    public void Start()
    {
        LoadAll(type);
    }


    public TileUIManager uiManager;
    public void SaveTile(
        string imagePath,
        Texture2D texture,
        Sprite sprite,
        Category category,
        string type
        )
    {
        string tileID = Guid.NewGuid().ToString();

        string tileFolder = Path.Combine(UnityEngine.Application.persistentDataPath, "Tiles");
        Directory.CreateDirectory(tileFolder);

        TileDataJson tileJson = new TileDataJson
        {
            tileId = tileID,
            imagePath = imagePath,
            width = texture.width,
            height = texture.height,
            pixelsPerUnit = 100f,
            type = type
        };
        string imageName = Path.GetFileNameWithoutExtension(imagePath);

        File.WriteAllText(
            Path.Combine(tileFolder, imageName + ".json"),
            JsonUtility.ToJson(tileJson, true)
        );

        string buildFolder = Path.Combine(UnityEngine.Application.persistentDataPath, "BuildingObjects");
        Directory.CreateDirectory(buildFolder);

        BuildingObjectJson boJson = new BuildingObjectJson
        {
            buildingId = Guid.NewGuid().ToString(),
            tileId = tileID,
            category = category
        };

        File.WriteAllText(
            Path.Combine(buildFolder, imageName + ".json"),
            JsonUtility.ToJson(boJson, true)
        );

        // Build runtime objects immediately
        UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
        tile.sprite = sprite;

        BuildingObject buildable = ScriptableObject.CreateInstance<BuildingObject>();
        typeof(BuildingObject)
            .GetField("category", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(buildable, category);

        typeof(BuildingObject)
            .GetField("tileBase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(buildable, tile);

        uiManager.Create(buildable, sprite);
    }

    public void LoadAll(string typeFilter)
    {
        string tilesFolder = Path.Combine(UnityEngine.Application.persistentDataPath, "Tiles");
        string buildFolder = Path.Combine(UnityEngine.Application.persistentDataPath, "BuildingObjects");

        if (!Directory.Exists(tilesFolder)) return;

        Dictionary<string, UnityEngine.Tilemaps.Tile> tileCache = new();

        foreach (string file in Directory.GetFiles(tilesFolder, "*.json"))
        {
            TileDataJson tileJson = JsonUtility.FromJson<TileDataJson>(File.ReadAllText(file));

            if (tileJson.type != typeFilter)
                continue;

            byte[] bytes = File.ReadAllBytes(tileJson.imagePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                tileJson.pixelsPerUnit
            );

            UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            tile.sprite = sprite;

            tileCache[tileJson.tileId] = tile;
        }

        foreach (string file in Directory.GetFiles(buildFolder, "*.json"))
        {
            BuildingObjectJson boJson = JsonUtility.FromJson<BuildingObjectJson>(File.ReadAllText(file));

            if (!tileCache.TryGetValue(boJson.tileId, out UnityEngine.Tilemaps.Tile tile))
                continue;

            BuildingObject buildable = ScriptableObject.CreateInstance<BuildingObject>();

            typeof(BuildingObject)
                .GetField("category", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(buildable, boJson.category);

            typeof(BuildingObject)
                .GetField("tileBase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(buildable, tile);

            uiManager.Create(buildable, tile.sprite);
        }
    }
}
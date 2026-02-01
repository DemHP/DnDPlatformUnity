using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TilePersistenceManager : MonoBehaviour
{

    public string type = "";
    public TileUIManager uiManager;


    public void Start()
    {
        RuntimeTileRegistry.Clear();
        LoadAll(type);
    }


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

        // ===== BUILD RUNTIME TILE =====
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.name = tileID;

        RuntimeTileRegistry.Register(tileID, tile);

        // ===== BUILD UI OBJECT =====
        BuildingObject buildable = ScriptableObject.CreateInstance<BuildingObject>();
        buildable.Initialize(tile, category);

        uiManager.Create(buildable, sprite);
    }

    // ================= LOAD ALL TILES =================
    public void LoadAll(string typeFilter)
    {
        string tilesFolder = Path.Combine(Application.persistentDataPath, "Tiles");
        string buildFolder = Path.Combine(Application.persistentDataPath, "BuildingObjects");

        if (!Directory.Exists(tilesFolder))
            return;

        foreach (string file in Directory.GetFiles(tilesFolder, "*.json"))
        {
            TileDataJson tileJson =
                JsonUtility.FromJson<TileDataJson>(File.ReadAllText(file));

            if (tileJson.type != typeFilter)
                continue;

            if (!File.Exists(tileJson.imagePath))
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

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.name = tileJson.tileId;

            RuntimeTileRegistry.Register(tileJson.tileId, tile);
        }

        if (!Directory.Exists(buildFolder))
            return;

        foreach (string file in Directory.GetFiles(buildFolder, "*.json"))
        {
            BuildingObjectJson boJson =
                JsonUtility.FromJson<BuildingObjectJson>(File.ReadAllText(file));

            TileBase tile = RuntimeTileRegistry.Get(boJson.tileId);
            if (tile == null)
                continue;

            BuildingObject buildable = ScriptableObject.CreateInstance<BuildingObject>();
            buildable.Initialize(tile, boJson.category);

            uiManager.Create(buildable, ((Tile)tile).sprite);
        }
    }
}
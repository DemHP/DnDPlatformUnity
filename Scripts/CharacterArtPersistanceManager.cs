using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


public class CharacterArtPersistanceManager
{

    public string type = "";
    public CharacterArtUIManager uiManager;

    public void Start()
    {
        RuntimeTileRegistry.Clear();
        LoadAll(type);
    }

    public void SaveArt(
        string imagePath,
        Texture2D texture,
        Sprite sprite,
        Category category,
        string type
    )
    {
        string artId = Guid.NewGuid().ToString();

        string artFolder = Path.Combine(UnityEngine.Application.persistentDataPath, "CharacterArt");
        Directory.CreateDirectory(artFolder);

        ArtDataJson artJson = new ArtDataJson
        {
            artId = artId,
            imagePath = imagePath,
            width = texture.width,
            height = texture.height,
            pixelsPerUnit = 1000f,
            type = type
        };

        string imageName = Path.GetFileNameWithoutExtension(imagePath);

        File.WriteAllText(
            Path.Combine(artFolder, imageName + ".jon"),
            JsonUtility.ToJson(artJson, true)
        );

        string buildFolder = Path.Combine(UnityEngine.Application.persistentDataPath, "CharacterSheets");
        Directory.CreateDirectory(buildFolder);

        BuildingArtJson boJson = new BuildingArtJson
        {
            buildingId = Guid.NewGuid().ToString(),
            artId = artId,
            category = category
        };

        File.WriteAllText(
            Path.Combine(buildFolder, imageName + ".json"),
            JsonUtility.ToJson(boJson, true)
        );

        // ===== BUILD RUNTIME ART =====
        Tile art = ScriptableObject.CreateInstance<Tile>();
        art.sprite = sprite;
        art.name = artId;

        RuntimeArtRegistry.Register(artId, art);

        // ===== BUILD UI OBJECT =====
        BuildingObject buildable = ScriptableObject.CreateInstance<BuildingObject>();

        uiManager.Create(buildable, sprite);

    }

    // ================= LOAD ALL ART =================
    public void LoadAll(string typeFilter)
    {
        string artFolder = Path.Combine(Application.persistentDataPath, "Tiles");
        string buildFolder = Path.Combine(Application.persistentDataPath, "BuildingArt");

        if (!Directory.Exists(artFolder))
            return;

        foreach (string file in Directory.GetFiles(artFolder, "*.json"))
        {
            ArtDataJson artJson = JsonUtility.FromJson<ArtDataJson>(File.ReadAllText(file));

            if (artJson.type != typeFilter)
                continue;

            if (!File.Exists(artJson.imagePath))
                continue;

            byte[] bytes = File.ReadAllBytes(artJson.imagePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                artJson.pixelsPerUnit
            );

            Tile art = ScriptableObject.CreateInstance<Tile>();
            art.sprite = sprite;
            art.name = artJson.artId;

            RuntimeArtRegistry.Register(artJson.artId, art);

        }

        if (!Directory.Exists(buildFolder))
            return;

        foreach (string file in Directory.GetFiles(buildFolder, "*.json"))
        {
            BuildingArtJson baJson =
                JsonUtility.FromJson<BuildingArtJson>(File.ReadAllText(file));

            TileBase art = RuntimeArtRegistry.Get(baJson.artId);
            if (art == null)
                continue;

            BuildingObject buildable = ScriptableObject.CreateInstance<BuildingObject>();
            uiManager.Create(buildable, ((Tile)art).sprite);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Linq;
using TMPro;
using System.Threading;
using System.Windows.Forms;
using Application = UnityEngine.Application;



[Serializable]
public class TileData
{
    public int x;
    public int y;
    public string tileId;
}

[Serializable]
public class TilemapLayerData
{
    public List<TileData> tiles = new();
}

[Serializable]
public class MapSaveData
{
    public TilemapLayerData layer1;
    public TilemapLayerData layer2;
    public TilemapLayerData layer3;

    public List<PrefabData> prefabs = new();
}

[Serializable]
public class PrefabData
{
    public string prefabId;
    public float x;
    public float y;
    public float z;

    public float scaleX;
    public float scaleY;
    public float scaleZ;

    public string tileId;
}

public class SaveManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap layer1;
    public Tilemap layer2;
    public Tilemap layer3;

    [Header("Tile Library")]
    public TileBase[] tileLibrary;

    [Header("Prefabs")]
    public GameObject grid;
    public GameObject tileVisualPrefab;

    [Header("Save Settings")]
    public string saveName = "New_Map";
    public TMP_InputField saveNameInputField;
    private string MapsFolder => Path.Combine(Application.persistentDataPath, "Maps");

    private string SavePath => Path.Combine(mapsFolderCached, $"{saveName}.json");

    private static readonly Queue<Action> mainThreadQueue = new();

    private string mapsFolderCached;

    private void Awake()
    {
        mapsFolderCached = Path.Combine(Application.persistentDataPath, "Maps");

        if (!Directory.Exists(mapsFolderCached))
            Directory.CreateDirectory(mapsFolderCached);
    }

    private void Update()
    {
        lock (mainThreadQueue)
        {
            while (mainThreadQueue.Count > 0)
                mainThreadQueue.Dequeue().Invoke();
        }
    }

    private void RunOnMainThread(Action action)
    {
        lock (mainThreadQueue)
        {
            mainThreadQueue.Enqueue(action);
        }
    }

    // Save
    public void SaveMap()
    {
        saveName = saveNameInputField.text;

        if (!Directory.Exists(MapsFolder))
            Directory.CreateDirectory(MapsFolder);

        saveName = string.Concat(saveName.Split(Path.GetInvalidFileNameChars()));

        if (string.IsNullOrWhiteSpace(saveName))
            saveName = "NewMap";

        MapSaveData saveData = new()
        {
            layer1 = SaveLayer(layer1),
            layer2 = SaveLayer(layer2),
            layer3 = SaveLayer(layer3),
            prefabs = SavePrefabs()
        };

        File.WriteAllText(SavePath, JsonUtility.ToJson(saveData, true));
        Debug.Log($"Map saved: {SavePath}");
    }


    private TilemapLayerData SaveLayer(Tilemap tilemap)
    {
        TilemapLayerData layerData = new();

        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            layerData.tiles.Add(new TileData
            {
                x = pos.x,
                y = pos.y,
                tileId = tile.name
            });
        }

        return layerData;
    }

    private List<PrefabData> SavePrefabs()
    {
        List<PrefabData> prefabDataList = new();

        if (grid == null) return prefabDataList;

        foreach (Transform child in grid.transform)
        {
            TileVisualData tileVisual = child.GetComponent<TileVisualData>();
            if (tileVisual == null) continue;

            Vector3 pos = child.position;
            Vector3 scale = child.localScale;

            prefabDataList.Add(new PrefabData
            {
                prefabId = "Tile_Template",
                x = pos.x,
                y = pos.y,
                z = pos.z,
                scaleX = scale.x,
                scaleY = scale.y,
                scaleZ = scale.z,
                tileId = tileVisual.visualTileData != null
                    ? tileVisual.visualTileData.name
                    : ""
            });
        }

        return prefabDataList;
    }


    // Load
    public void LoadMap()
    {
        string initialDir = mapsFolderCached; // capture main-thread value

        Thread thread = new Thread(() =>
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Map JSON",
                Filter = "JSON files (*.json)|*.json",
                InitialDirectory = initialDir,
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;

                RunOnMainThread(() =>
                {
                    LoadMapFromPath(path);
                });
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }


    private void LoadMapFromPath(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Map save not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        MapSaveData saveData = JsonUtility.FromJson<MapSaveData>(json);

        ClearAllLayers();
        ClearAllPrefabs();

        LoadLayer(layer1, saveData.layer1);
        LoadLayer(layer2, saveData.layer2);
        LoadLayer(layer3, saveData.layer3);
        LoadPrefabs(saveData.prefabs);

        Debug.Log($"Map loaded: {path}");
    }

    private void LoadLayer(Tilemap tilemap, TilemapLayerData layerData)
    {
        if (layerData == null) return;

        foreach (TileData data in layerData.tiles)
        {
            // Check runtime registry first
            TileBase tile = RuntimeTileRegistry.Get(data.tileId);
            if (tile == null)
            {
                // Fall back to tile library
                tile = tileLibrary.FirstOrDefault(t => t.name == data.tileId);
            }

            if (tile != null)
            {
                tilemap.SetTile(new Vector3Int(data.x, data.y, 0), tile);
            }
            else
            {
                Debug.LogWarning($"Tile not found: {data.tileId}");
            }
        }
    }

    private void LoadPrefabs(List<PrefabData> prefabs)
    {
        if (prefabs == null || grid == null) return;

        foreach (PrefabData data in prefabs)
        {
            if (data.prefabId != "Tile_Template") continue;

            GameObject obj = Instantiate(
                tileVisualPrefab,
                new Vector3(data.x, data.y, data.z),
                Quaternion.identity,
                grid.transform
            );

            obj.transform.localScale = new Vector3(data.scaleX, data.scaleY, data.scaleZ);

            TileVisualData tileVisual = obj.GetComponent<TileVisualData>();
            if (tileVisual != null && !string.IsNullOrEmpty(data.tileId))
            {
                TileBase tile = RuntimeTileRegistry.Get(data.tileId) ??
                                tileLibrary.FirstOrDefault(t => t.name == data.tileId);

                if (tile != null)
                    tileVisual.ChangeTileData(tile);
                else
                    Debug.LogWarning($"Missing TileBase for prefab: {data.tileId}");
            }
        }
    }

    private void ClearAllLayers()
    {
        layer1.ClearAllTiles();
        layer2.ClearAllTiles();
        layer3.ClearAllTiles();
    }

    private void ClearAllPrefabs()
    {
        if (grid == null) return;

        List<Transform> children = new();
        foreach (Transform child in grid.transform)
        {
            if (child.GetComponent<TileVisualData>() != null)
                children.Add(child);
        }

        foreach (Transform child in children)
            DestroyImmediate(child.gameObject);
    }
}
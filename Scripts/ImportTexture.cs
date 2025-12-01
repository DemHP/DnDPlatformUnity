using UnityEngine;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System;
using System.Collections.Generic;
using Application = UnityEngine.Application;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class ImportTexture : MonoBehaviour
{
    public Texture2D LoadedTexture;
    public Sprite LoadedSprite;

    public GameObject tileTemplate;       // Your prefab
    public GameObject generalManager;     // GeneralManager with BuildingCreator
    public Transform parent;              // Parent container for UI tiles

    // ===== MAIN THREAD QUEUE =====
    private static readonly Queue<Action> mainThreadQueue = new Queue<Action>();

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

    // Called from a UI button
    public void Import()
    {
        PickImage();
    }

    // =====================================
    // Pick an image using Windows File Dialog
    // =====================================
    private void PickImage()
    {
        Thread thread = new Thread(() =>
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select an Image";
            dialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;

                RunOnMainThread(() =>
                {
                    SaveAndConvert(path);
                });
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    // =====================================
    // Save -> Resize -> Create Tile -> BuildingObject -> Tile_Template
    // =====================================
    private void SaveAndConvert(string originalPath)
    {
        // Save Image
        string imagesFolder = Path.Combine(Application.persistentDataPath, "ImportedImages");
        Directory.CreateDirectory(imagesFolder);

        string fileName = Path.GetFileName(originalPath);
        string copiedPath = Path.Combine(imagesFolder, fileName);
        File.Copy(originalPath, copiedPath, true);

        Debug.Log("Saved Image To: " + copiedPath);

        // Load Texture
        byte[] bytes = File.ReadAllBytes(copiedPath);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        // Resize to 500x500
        LoadedTexture = ResizeTexture(tex, 500, 500);

        // Create Sprite
        LoadedSprite = Sprite.Create(
            LoadedTexture,
            new Rect(0, 0, LoadedTexture.width, LoadedTexture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        Debug.Log("Resized Texture: " + LoadedTexture.width + "x" + LoadedTexture.height);

        // =====================================
        // Create runtime TileBase
        // =====================================
        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = LoadedSprite;

        // =====================================
        // Create BuildingObject ScriptableObject
        // =====================================
        BuildingObject buildable = ScriptableObject.CreateInstance<BuildingObject>();

        // Assign private fields via reflection
        typeof(BuildingObject)
            .GetField("category", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(buildable, Category.Tile);

        typeof(BuildingObject)
            .GetField("tileBase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(buildable, newTile);

        // =====================================
        // Create Tile_Template instance
        // =====================================
        CreateTileTemplate(buildable);
    }

    // =====================================
    // Resize utility
    // =====================================
    Texture2D ResizeTexture(Texture2D src, int newW, int newH)
    {
        Texture2D dst = new Texture2D(newW, newH);

        float stepX = 1f / newW;
        float stepY = 1f / newH;

        for (int y = 0; y < newH; y++)
        {
            for (int x = 0; x < newW; x++)
            {
                Color newColor = src.GetPixelBilinear(x * stepX, y * stepY);
                dst.SetPixel(x, y, newColor);
            }
        }

        dst.Apply();
        return dst;
    }

    // =====================================
    // Create Tile_Template and assign Button
    // =====================================
    private void CreateTileTemplate(BuildingObject buildingObject)
    {
        if (tileTemplate == null)
        {
            Debug.LogError("Tile Template is NOT assigned!");
            return;
        }

        GameObject newTile = Instantiate(tileTemplate, parent);

        // Assign the visible Image
        TileButtonHandler handler = newTile.GetComponent<TileButtonHandler>();
        if (handler == null)
        {
            Debug.LogError("Tile_Template missing TileButtonHandler component!");
            return;
        }

        handler.assignedBuildingObject = buildingObject;
        handler.generalManager = generalManager;

        UnityEngine.UI.Button btn = newTile.GetComponentInChildren<UnityEngine.UI.Button>();
        if (btn != null)
        {
            handler.AssignButton(btn);
        }

        // Assign the sprite for the UI image
        UnityEngine.UI.Image img = newTile.GetComponentInChildren<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.sprite = LoadedSprite;
        }

        Debug.Log("Tile Template created and button assigned!");
    }
}

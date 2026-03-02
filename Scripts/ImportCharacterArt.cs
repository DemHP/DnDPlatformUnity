using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.XR;
using Application = UnityEngine.Application;


public class ImportCharacterArt : MonoBehaviour
{
    public TilePersistenceManager persistenceManager;

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

    // Called by UI button
    public void Import(string type)
    {
        PickImage(type);
    }

    private void PickImage(string type)
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
                    LoadAndSend(path, type);
                });
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();        
    }

    private void LoadAndSend(string originalPath, string type)
    {
        // Save image
        string characterArtFolder = Path.Combine(Application.persistentDataPath, "ImportedImages");
        Directory.CreateDirectory(characterArtFolder);

        string fileName = Path.GetFileName(originalPath);
        string copiedPath = Path.Combine(characterArtFolder, fileName);
        File.Copy(originalPath, copiedPath, true);

        // Load image
        byte[] bytes = File.ReadAllBytes(copiedPath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        // Resize
        Texture2D resized = ResizedTexture(tex, 1000, 1000);

        // Encode resized image to PNG and save
        byte[] pngBytes = resized.EncodeToPNG();
        File.WriteAllBytes(copiedPath, pngBytes);
        Debug.Log($"Resized image saved to: {copiedPath}");

        // Create sprite
        Sprite sprite = Sprite.Create(
            resized,
            new Rect(0, 0, resized.width, resized.height),
            new Vector2(0.5f, 05f),
            100f
        );

        // Hand off to persistence manager
        persistenceManager.SaveTile(
            copiedPath,
            resized,
            sprite,
            Category.Tile,
            type
        );

    }

    private Texture2D ResizedTexture(Texture2D src, int newW, int newH)
    {
        Texture2D dst = new Texture2D(newW, newH);

        float stepX = 1f / newW;
        float stepY = 1f / newH;

        for (int y = 0; y < newH; y++)
        {
            for (int x = 0; x < newW; x++)
            {
                Color c = src.GetPixelBilinear(x * stepX, y * stepY);
                dst.SetPixel(x, y, c);
            }
        }

        dst.Apply();
        return dst;
    }
}
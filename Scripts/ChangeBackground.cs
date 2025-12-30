using System.Collections;
using UnityEngine;
using System.Windows.Forms;
using System.IO;

public class ChangeBackground : MonoBehaviour
{
    public SpriteRenderer targetRenderer;

    public void PickSpriteImage()
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            dialog.Title = "Select an Image";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ApplySpriteImage(dialog.FileName);
            }
        }
    }

    private void ApplySpriteImage(string path)
    {
        // Load file to Texture2D
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        // Resize the texture to 2048x2048
        Texture2D resized = ResizeTexture(tex, 2048, 2048);

        // Create a sprite from the resized texture
        Sprite sprite = Sprite.Create(
            resized,
            new Rect(0, 0, resized.width, resized.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        targetRenderer.sprite = sprite;
    }

   
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        RenderTexture.active = rt;

        // Copy source texture to render texture
        Graphics.Blit(source, rt);

        // Create new Texture2D with the desired size
        Texture2D newTex = new Texture2D(newWidth, newHeight, source.format, false);
        newTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTex.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return newTex;
    }
}
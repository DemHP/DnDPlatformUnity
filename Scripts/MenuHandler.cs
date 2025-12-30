using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO.Compression;
using UnityEngine.Tilemaps;
using System.Windows.Forms;
using UnityEditor.Rendering;
public class MenuHandler : MonoBehaviour
{
    public BrushController brushController;
    public GameObject bookmark;
    public float tweenSpeed = 0.3f;
    public float menuOffsetX = 200f;
    public void OpenMenu(GameObject menu)
    {
        if (!menu.activeSelf)
        {
            menu.SetActive(true);
        }
        else CloseMenu(menu);
    }

    public void MenuSlideIn(GameObject menu)
    {
        menu.transform.DOLocalMoveX(menuOffsetX, tweenSpeed).SetEase(Ease.OutCubic);
    }

    public void MenuSlideOut(GameObject menu)
    {
        menu.transform.DOLocalMoveX(550, tweenSpeed).SetEase(Ease.OutCubic);
    }

    public void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void MoveBookmark(int posX)
    {
        // Animate the bookmark to the new position
        bookmark.transform.DOLocalMoveX(posX, tweenSpeed).SetEase(Ease.OutCubic);
    }

    public void ChangeBrushMode(string mode)
    {
        brushController.ResetBrushSize(1, 1);
        brushController.EndBrush();
        brushController.brushMode = (BrushMode)System.Enum.Parse(typeof(BrushMode), mode);
    }

    public void ChangeScenes(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void ChangeLayers(Tilemap newLayer)
    {
        brushController.buildCreator.defaultMap = newLayer;
    }
}

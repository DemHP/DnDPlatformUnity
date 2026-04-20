using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Application = UnityEngine.Application;


// This is a simple class to hold the settings data for serialization
[System.Serializable]

public class SideMenuHandler : MonoBehaviour
{
    public GameObject bookmark; // The bookmark is the little highlight that moves to show which menu is selected
    public float tweenSpeed = 0.3f;
    public float menuOffsetX = 200f;

    private List<Vector2Int> resolutions = new List<Vector2Int>()
    {
        new Vector2Int(3840, 2160),
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1366, 768)
    };

    private bool isFullScreen;

    public void OpenMenu(GameObject menu)
    {
        if (!menu.activeSelf)
        {
            menu.SetActive(true);
        }
        else CloseMenu(menu);
    }
    public void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    // This is what causes the menus in Map Maker to slide in and out of frame. The button in the UI will call these functions.
    public void MenuSlideIn(GameObject menu)
    {
        menu.transform.DOLocalMoveX(menuOffsetX, tweenSpeed).SetEase(Ease.OutCubic);
    }

    public void MenuSlideOut(GameObject menu)
    {
        menu.transform.DOLocalMoveX(550, tweenSpeed).SetEase(Ease.OutCubic);
    }

    // This is what causes the bookmark to move to the selected menu. The button in the UI will call these functions.
    public void MoveBookmark(int posX)
    {
        // Animate the bookmark to the new position
        bookmark.transform.DOLocalMoveX(posX, tweenSpeed).SetEase(Ease.OutCubic);
    }

    // This was made but doesn't work properly...Not entirely sure why and hadn't gotten around to finding out.
    public void MoveBookmarkY(int posY)
    {
        // Animate the bookmark to the new position
        bookmark.transform.DOLocalMoveY(posY, tweenSpeed).SetEase(Ease.OutCubic);
    }

    public void ChangeResolution(int index)
    {
        Vector2Int res = resolutions[index];
        UnityEngine.Screen.SetResolution(res.x, res.y, UnityEngine.Screen.fullScreen = isFullScreen);
    }

}



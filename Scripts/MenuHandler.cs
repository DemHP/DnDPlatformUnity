using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public BrushController brushController;
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
}

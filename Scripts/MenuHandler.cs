using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO.Compression;
using UnityEngine.Tilemaps;
using System.Windows.Forms;
using Application = UnityEngine.Application;
using System.IO;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// This is a simple class to hold the settings data for serialization
[System.Serializable]
public class Settings
{
    public int dropdownValue;
    public float slider1;
    public float slider2;
    public float slider3;
    public float slider4;
    public bool toggleValue;
    public string playerNameText;
}

// This is the main class that handles the menu interactions
public class MenuHandler : MonoBehaviour
{

    public BrushController brushController; // I'm bad at my job and made brushController's menu stuff a part of this script
    public GameObject bookmark; // The bookmark is the little highlight that moves to show which menu is selected
    public float tweenSpeed = 0.3f;
    public float menuOffsetX = 200f;

    // UI Elements for saving and loading settings. Header just keeps them organized in the Unity inspector and doesn't affect functionality
    [Header("Save Settings")]
    public TMP_Dropdown dropdown;
    public Slider slider1, slider2, slider3, slider4;
    public Toggle toggle;
    public TMP_Text playerName;

    private string MapsFolder => Path.Combine(Application.persistentDataPath, "Maps");
    private string filePath;

    private string _pendingMapPath;
    private List<Vector2Int> resolutions = new List<Vector2Int>()
    {
        new Vector2Int(3840, 2160),
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1366, 768)
    };

    private bool isFullScreen;

    // This is called when the script is first loaded.
    void Awake()
    {
        // persistentDataPath is a special folder that Unity provides for saving data.
        filePath = Path.Combine(Application.persistentDataPath, "settings.json");
        LoadSettings(); // Just a function to load the settings when the application starts
    }

    // This function is called to make a menu visible. A button in the UI will call this function.
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

    // This function changes the brush mode in the BrushController script. The button in the UI will call this function.
    public void ChangeBrushMode(string mode)
    {
        brushController.ResetBrushSize(1, 1);
        brushController.EndBrush();
        brushController.brushMode = (BrushMode)System.Enum.Parse(typeof(BrushMode), mode);
    }

    // Changes the current Unity scene to the one specified by the button in the UI. The button in the UI will call this function.
    public void ChangeScenes(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    // This function changes the tilemap that the BrushController script is using. The button in the UI will call this function.
    public void ChangeLayers(Tilemap newLayer)
    {
        brushController.buildCreator.defaultMap = newLayer;
    }

    // This function saves the settings to a JSON file. The button in the UI will call this function.
    public void SaveSettings()
    {
        Settings settings = new Settings
        {
            dropdownValue = dropdown.value,
            slider1 = slider1.value,
            slider2 = slider2.value,
            slider3 = slider3.value,
            slider4 = slider4.value,
            toggleValue = toggle.isOn,
            playerNameText = playerName.text
        };

        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Settings saved to: " + filePath);
    }

    // This function loads the settings from a JSON file. This is only called in Awake.
    public void LoadSettings()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Settings settings = JsonUtility.FromJson<Settings>(json);

            dropdown.value = settings.dropdownValue;
            slider1.value = settings.slider1;
            slider2.value = settings.slider2;
            slider3.value = settings.slider3;
            slider4.value = settings.slider4;
            toggle.isOn = settings.toggleValue;
            playerName.text = settings.playerNameText;

            Debug.Log("Settings loaded.");
        }
        else
        {
            Debug.Log("Settings file not found.");
        }
    }

    public void ChangeResolution(int index)
    {
        Vector2Int res = resolutions[index];
        UnityEngine.Screen.SetResolution(res.x, res.y, UnityEngine.Screen.fullScreen = isFullScreen);
    }

    public void SetFullScreen()
    {
        isFullScreen = toggle.isOn;

        UnityEngine.Screen.fullScreen = isFullScreen;
    }

    public void LoadLatestMapByValue(int latest)
    {
        // Get all JSON files
        string[] files = Directory.GetFiles(MapsFolder, "*.json");

        // Check if any files exist
        if (files == null || files.Length == 0)
        {
            Debug.LogWarning("No maps found in Maps folder.");
            return;
        }

        // Validate index
        if (latest < 0 || latest >= files.Length)
        {
            Debug.LogError($"Invalid map index: {latest}. Valid range is 0 to {files.Length - 1}");
            return;
        }

        // Store selected file path
        _pendingMapPath = files[latest];
        Debug.Log($"Selected Map File: {_pendingMapPath}");

        // Prevent duplicate subscriptions
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the target scene
        SceneManager.LoadScene("MapMaker");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only run for the correct scene
        if (scene.name != "MapMaker")
            return;

        // Unsubscribe immediately to avoid multiple calls
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Find SaveManager in the newly loaded scene
        SaveManager saveManager = FindFirstObjectByType<SaveManager>();

        if (saveManager == null)
        {
            Debug.LogError("SaveManager not found in MapMaker scene.");
            return;
        }

        if (string.IsNullOrEmpty(_pendingMapPath))
        {
            Debug.LogError("No pending map path set.");
            return;
        }

        // Load the map
        saveManager.LoadMapFromPath(_pendingMapPath);

        // Clear after use (optional but good practice)
        _pendingMapPath = null;
    }
}

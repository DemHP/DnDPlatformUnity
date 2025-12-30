using UnityEngine;
using UnityEngine.UI;
public class TileUIManager : MonoBehaviour
{

    [Header("UI References")]
    public GameObject tileTemplate;
    public Transform parent;
    public GameObject generalManager;

    public void Create(BuildingObject buildingObject, Sprite sprite)
    {
        if (tileTemplate == null)
        {
            Debug.LogError("TileUIManager: Tile_Template is not assigned!");
            return;
        }

        if (buildingObject == null)
        {
            Debug.LogError("TileUIManager: BuildingObject is null!");
            return;
        }

        GameObject tileInstance = Instantiate(tileTemplate, parent);

        // ---- Assign image ----
        Image img = tileInstance.GetComponentInChildren<Image>();
        if (img != null)
        {
            img.sprite = sprite;
            RectTransform rt = img.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100); 
        }
        else
        {
            Debug.LogError("TileUIManager: No Image component found in Tile_Template!");
        }

        // ---- Assign button ----
        Button btn = tileInstance.GetComponentInChildren<Button>();
        TileButtonHandler handler = tileInstance.GetComponent<TileButtonHandler>();

        if (btn == null || handler == null)
        {
            Debug.LogError("TileUIManager: Missing Button or TileButtonHandler on Tile_Template!");
            return;
        }

        handler.assignedBuildingObject = buildingObject;
        handler.generalManager = generalManager;
        handler.AssignButton(btn);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class CharacterArtUIManager : MonoBehaviour
{
    [Header("UI Referemces")]
    public GameObject artTemplate;
    public Transform parent;
    public GameObject generalManager;

    public void Create(BuildingObject buildingArt, Sprite sprite)
    {
        if (artTemplate == null)
        {
            Debug.LogError("CharacerArtUIManager: Art_Template is not assigned!");
            return;
        }

        if (buildingArt == null)
        {
            Debug.LogError("CharacerArtUIManager: BuildingObject is null!");
            return;
        }

        GameObject artInstance = Instantiate(artTemplate, parent);

        // ---- Assign image ----
        Image img = artInstance.GetComponentInChildren<Image>();

        if (img != null)
        {
            img.sprite = sprite;
            RectTransform rt = img.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1000, 1000);
        }
        else
        {
            Debug.LogError("CharacerArtUIManager: No Image component found in Art_Template!");

        }

        // ---- Assign button ----
        Button btn = artInstance.GetComponentInChildren<Button>();
        CharacterArtButtonHandler handler = artInstance.GetComponentInChildren<CharacterArtButtonHandler>();

        if (btn == null || handler == null)
        {
            Debug.LogError("CharacerArtUIManager: Missing Button or CharacterArtButtonHandler on Art_Template!");
            return;
        }

        handler.assignedBuildingObject = buildingArt;
        handler.generalManager = generalManager;
        handler.AssignButton(btn);
    }
}


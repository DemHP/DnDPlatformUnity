using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectionHighlight : MonoBehaviour
{
    private static Button selectedButton; // shared across all buttons

    private Button button;
    private ColorBlock originalColors;

    void Awake()
    {
        button = GetComponent<Button>();
        originalColors = button.colors;

        button.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        // Reset previously selected button
        if (selectedButton != null && selectedButton != button)
        {
            SetMultiplier(selectedButton, 1f);
        }

        // Set this button as selected
        selectedButton = button;
        SetMultiplier(button, 1.5f);
    }

    void SetMultiplier(Button target, float value)
    {
        ColorBlock cb = target.colors;
        cb.colorMultiplier = value;
        target.colors = cb;
    }
}
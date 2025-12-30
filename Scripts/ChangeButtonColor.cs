using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    public Button mainButton;
    public Button[] otherButtons;

    void Start()
    {
        mainButton.GetComponent<Button>();
    }

    public void ChangeColor()
    {
        var colorBlock = mainButton.colors;

        colorBlock.colorMultiplier = 4f;

        mainButton.colors = colorBlock;
        TurnOffOtherButtons();
    }

    public void TurnOffOtherButtons()
    {
        foreach (Button button in otherButtons)
        {
            var colorBlock = button.colors;

            colorBlock.colorMultiplier = 1f;

            button.colors = colorBlock;
        }
    }
}

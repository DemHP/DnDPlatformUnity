using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideBaCommands : MonoBehaviour
{
    public TMP_Dropdown diceDropDown;
    public TMP_Text diceRollText;

    public int selectedIndex;
    int randomNum = 20;

    void Start()
    {
        diceDropDown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });
    }

    void OnDropdownChanged()
    {
        selectedIndex = diceDropDown.value;
    }

    public void updateDiceRollText()
    {
        diceRollText.text = randomNum.ToString();
    }

    public void RollDice()
    {
        switch (selectedIndex){
            case 0:
                randomNum = Random.Range(1, 4);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;

            case 1:
                randomNum = Random.Range(1, 5);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;

            case 2:
                randomNum = Random.Range(1, 7);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;

            case 3:
                randomNum = Random.Range(1, 9);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;

            case 4:
                randomNum = Random.Range(1, 11);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;

            case 5:
                randomNum = Random.Range(1, 13);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;

            case 6:
                randomNum = Random.Range(1, 21);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;

            case 7:
                randomNum = Random.Range(1, 101);
                updateDiceRollText();
                Debug.Log(randomNum);
                break;
        }
    }
}
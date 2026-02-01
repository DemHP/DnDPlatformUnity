using TMPro;
using UnityEngine;

public class TextObject : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TMP_InputField input;
    public GameObject inputObject;
    public GameObject editBackground;
    public bool writeMode = true;

    void Update()
    {
        if(writeMode)
        {
            text.text = input.text;

            if(Input.GetKey(KeyCode.Return)) {
                writeMode = false;

                Destroy(inputObject);
            }
        }
    }

    public void IncreaseHeight(int amount)
    {
        text.fontSize += amount;
    }

    public void EditVisibility(GameObject background)
    {
        background.SetActive(!background.activeSelf);
    }
}

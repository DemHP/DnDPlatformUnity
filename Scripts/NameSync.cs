using TMPro;
using UnityEngine;

public class NameSync : MonoBehaviour
{
    public TMP_Text display;
    public TMP_Text name1;

    void Start()
    {
        updateName();
    }

    public void updateName()
    {
        display.text = name1.text;
    }
}

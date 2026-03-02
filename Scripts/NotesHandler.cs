using System.Collections.Generic;
using UnityEngine;

public class NotesHandler : MonoBehaviour
{
    public GameObject notesButton;
    public GameObject notesInputField;

    public List<GameObject> notesButtonKeeper = new List<GameObject>();
    public List<GameObject> notesInputKeeper = new List<GameObject>();

    public Transform notesParent, notesInputFieldParent;

    private int i = 1;

    public void AddNote()
    {
        // Disable last note
        if (notesInputKeeper.Count > 0)
        {
            notesInputKeeper[^1].SetActive(false);  // disables last element
        }

        // Instantiate
        GameObject nButton = Instantiate(notesButton, notesParent);
        GameObject nInput = Instantiate(notesInputField, notesInputFieldParent);

        // Store
        notesButtonKeeper.Add(nButton);
        notesInputKeeper.Add(nInput);

        // Set Field
        NotesButtonHandler handler = nButton.GetComponent<NotesButtonHandler>();
        handler.NotesInputField = nInput;
        handler.notesHandler = this;
        handler.numberText.text = i.ToString();
        i++;
    }

    public void CloseAllInputs()
    {
        foreach (var input in notesInputKeeper)
        {
            input.SetActive(false);
        }
    }

    public void OpenInput(GameObject inputToOpen)
    {
        CloseAllInputs(); // close everything first
        inputToOpen.SetActive(true);
    }

    public void RemoveNote(GameObject button, GameObject input)
    {
        bool wasActive = input.activeSelf;

        notesButtonKeeper.Remove(button);
        notesInputKeeper.Remove(input);

        Destroy(button);
        Destroy(input);

        if (wasActive)
        {
            CloseAllInputs(); 
        }
        i--;
    }
}
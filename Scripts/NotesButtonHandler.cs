using TMPro;
using UnityEngine;

public class NotesButtonHandler : MonoBehaviour
{
    public GameObject NotesInputField;
    public TMP_Text numberText;
    public NotesHandler notesHandler;

    public void EnableNotesField()
    {
        notesHandler.OpenInput(NotesInputField);
    }

    public void DeleteNotes()
    {
        notesHandler.RemoveNote(this.gameObject, NotesInputField);
    }
}

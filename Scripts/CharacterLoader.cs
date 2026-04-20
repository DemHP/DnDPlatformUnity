using UnityEngine;
using System.IO;
using Unity.Netcode;

public class CharacterLoader : NetworkBehaviour
{
    public CharacterStats stats;

    public void RequestLoadCharacter()
    {
        if (!IsOwner)
        {
            Debug.LogWarning("Only the owning client can load a character.");
            return;
        }

        OpenFileDialog();
    }

    private void OpenFileDialog()
    {
        var dialog = new System.Windows.Forms.OpenFileDialog();
        dialog.Filter = "JSON files (*.json)|*.json";

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            Load(dialog.FileName);
        }
    }

    private void Load(string path)
    {
        string json = File.ReadAllText(path);

        // Local apply
        stats = JsonUtility.FromJson<CharacterStats>(json);
        GetComponent<PlayerCharacterStats>().Initialize(stats);

        // Network sync (server authority)
        GetComponent<NetworkCharacterState>().SetCharacterJson(json);
    }
}
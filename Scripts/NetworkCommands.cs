using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkCommands : NetworkBehaviour
{
    [Header("Hide/Show")]
    public GameObject[] showToHostOnly;
    
    [Header("Player Info")]
    public GameObject[] players;
    public GameObject playerDataPrefab;
    public Transform playerDataParent;

    public void Update()
    {
        ShowOnlyHost(showToHostOnly);
        players = GameObject.FindGameObjectsWithTag("PC");
    }

    public void ShowOnlyHost(GameObject[] showHostOnly)
    {
        foreach (GameObject go in showHostOnly)
        {
            go.SetActive(IsHost);
        }
    }

    public void ChangeSceneForAllPlayers(string sceneName)
    {
        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void SwitchCurrentSelectedNPC(GameObject npc)
    {
        GameObject dmObject = GameObject.FindWithTag("DM");

        PlayerController dm = dmObject.GetComponent<PlayerController>();
        dm.npcPrefabs.Add(npc);
        dm.selectedNPCPrefabIndex = dm.npcPrefabs.IndexOf(npc);
    }

    public void GetPlayerOnClick()
    {
        // Search players[] for the one where IsOwner is true
        foreach (GameObject go in players)
        {
            NetworkObject networkObject = go.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsOwner)
            {
                playerDataPrefab = go;
                Debug.Log($"Player Character found as:{go.name}");
                break; 
            }
        }

        playerDataPrefab.GetComponent<CharacterLoader>().RequestLoadCharacter();
    }

    // needed this for debugging purposes i'm so tired
    public void PrintPlayerName(NetworkObject targetPlayer)
    {
        if (targetPlayer == null)
        {
            Debug.Log("Target player is null");
            return;
        }

        var stats = targetPlayer.GetComponent<PlayerCharacterStats>();

        if (stats == null)
        {
            Debug.Log("No PlayerCharacterStats found on target");
            return;
        }

        Debug.Log($"PLAYER NAME: {stats.Stats.charName}");
    }
}
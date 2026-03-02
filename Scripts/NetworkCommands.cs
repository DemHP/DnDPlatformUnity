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

}
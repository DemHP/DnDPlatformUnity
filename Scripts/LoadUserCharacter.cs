using Unity.Netcode;
using UnityEngine;

public class LoadUserCharacter : MonoBehaviour
{
    public void OnClickLoadCharacter(NetworkObject targetPlayer)
    {
        if (targetPlayer == null)
        {
            Debug.LogError("Target player is null");
            return;
        }

        var loader = targetPlayer.GetComponent<CharacterLoader>();

        if (loader == null)
        {
            Debug.LogError("No CharacterLoader found on target player");
            return;
        }

        loader.RequestLoadCharacter();
    }
}

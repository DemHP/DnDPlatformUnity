using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class NetworkCharacterState : NetworkBehaviour
{
    public NetworkVariable<FixedString4096Bytes> CharacterJson =
        new NetworkVariable<FixedString4096Bytes>(
            new FixedString4096Bytes(),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        CharacterJson.OnValueChanged += OnCharacterChanged;

        // Apply initial state on spawn
        ApplyFromJson(CharacterJson.Value.ToString());
    }

    public override void OnNetworkDespawn()
    {
        CharacterJson.OnValueChanged -= OnCharacterChanged;
    }

    private void OnCharacterChanged(FixedString4096Bytes previousValue, FixedString4096Bytes newValue)
    {
        ApplyFromJson(newValue.ToString());
    }

    private void ApplyFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        CharacterStats stats = JsonUtility.FromJson<CharacterStats>(json);
        Apply(stats);
    }

    private void Apply(CharacterStats stats)
    {
        GetComponent<PlayerCharacterStats>().Initialize(stats);
    }

    public void SetCharacterJson(string json)
    {
        if (!IsOwner) return;

        SubmitCharacterServerRpc(json);
    }

    [ServerRpc]
    private void SubmitCharacterServerRpc(string json)
    {
        CharacterJson.Value = new FixedString4096Bytes(json);
    }
}
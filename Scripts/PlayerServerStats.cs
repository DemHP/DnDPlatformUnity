using Unity.Netcode;
using UnityEngine;

public class PlayerServerStats : NetworkBehaviour
{
    // When Character Creator is done, we'll have the server grab those stats
    public int localStr;
    public int localDex;
    public int localCon;
    public int localInt;
    public int localWis;
    public int localChar;
    public int localMaxHP;
    public int localCurrentHP;
    public int localAC;

    // DnD Stats
    public NetworkVariable<int> str = new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> dex= new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> con= new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> intelligence = new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> wis= new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> charisma = new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> maxHP = new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> currentHP = new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> armorClass = new NetworkVariable<int>(
        10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InitializeStats();
        }
    }

    private void InitializeStats()
    {
        // Example defaults
        str.Value = localStr;
        dex.Value = localDex;
        con.Value = localCon;
        intelligence.Value = localInt;
        wis.Value = localWis;
        charisma.Value = localChar;

        maxHP.Value = localMaxHP;

        // currentHP.Value = maxHP.Value;

        armorClass.Value = localAC;
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        currentHP.Value = Mathf.Max(currentHP.Value - amount, 0);
    }
    
    public void Heal(int amount)
    {
        if (!IsServer) return;

        currentHP.Value = Mathf.Min(currentHP.Value + amount, maxHP.Value);
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int amount)
    {
        TakeDamage(amount);
    }
}

using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour
{ 
    // This needs to be completely redone 

    public TMP_Text str, dex, con, intell, wis, cha, maxHp, currHp, ac;
    public GameObject menu;
    public bool isOpen = true;
}

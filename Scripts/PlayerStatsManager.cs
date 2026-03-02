using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour
{ 
    public TMP_Text str, dex, con, intell, wis, cha, maxHp, currHp, ac;
    public GameObject menu;
    public bool isOpen = true;
    public void FetchPlayerStats(GameObject player)
    {
        PlayerServerStats stats = player.GetComponent<PlayerServerStats>();

        str.text = stats.localStr.ToString();
        dex.text = stats.localDex.ToString();
        con.text = stats.localCon.ToString();
        intell.text = stats.localInt.ToString();
        wis.text = stats.localWis.ToString();
        cha.text = stats.localChar.ToString();
        maxHp.text = stats.localMaxHP.ToString();
        currHp.text = stats.localCurrentHP.ToString();
        ac.text = stats.localAC.ToString();
    }

    public void OpenPlayerStatsMenu()
    {
        menu.SetActive(isOpen);
        isOpen = !isOpen;
    }
}

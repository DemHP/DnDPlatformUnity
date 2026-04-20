using UnityEngine;

public class PlayerCharacterStats : MonoBehaviour
{
    public CharacterStats Stats { get; private set; }

    public void Initialize(CharacterStats stats)
    {
        Stats = stats;

        Debug.Log($"Loaded character: {Stats.charName}");
    }
}
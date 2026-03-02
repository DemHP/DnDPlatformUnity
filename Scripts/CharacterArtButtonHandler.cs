using UnityEngine;
using UnityEngine.UI;

public class CharacterArtButtonHandler : MonoBehaviour
{
    public BuildingObject assignedBuildingObject;
    public GameObject generalManager;

    public void AssignButton(Button btn)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            BuildingCreator creator = generalManager.GetComponent<BuildingCreator>();
            if (creator != null)
            {
                creator.ObjectSelected(assignedBuildingObject);
            }
            else
            {
                Debug.LogError("GeralManager does not have BuildingCreator!");
            }
        });
    }
}

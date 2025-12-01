using UnityEngine;
using UnityEngine.UI;

public class TileButtonHandler : MonoBehaviour
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
                creator.ObjectSelected(assignedBuildingObject);
            else
                Debug.LogError("GeneralManager does not have BuildingCreator!");
        });
    }
}

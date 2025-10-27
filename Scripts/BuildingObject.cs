using UnityEngine;
using UnityEngine.Tilemaps;

public enum Category
{
    Wall,
    Floor
    //Roof,
    //Furniture,
    //Decoration
}

[CreateAssetMenu(fileName = "Buildable", menuName = "BuildingObjects/Create Buildable")]
public class BuildingObject : ScriptableObject {
    [SerializeField] Category category;
    [SerializeField] TileBase tileBase;

    public TileBase TileBase
    {
        get {
            return tileBase;
        }
    }

    public Category Category
    {
        get
        {
            return category;
        }
    }
}

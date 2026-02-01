using UnityEngine;
using UnityEngine.Tilemaps;

public enum Category
{
    Tile,
    //Floor
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

    public void Initialize(TileBase tile, Category category)
    {
        this.tileBase = tile;
        this.category = category;
    }
}

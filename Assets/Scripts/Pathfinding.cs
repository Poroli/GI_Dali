using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public Vector3 FloatDestination;

    [SerializeField] private string NameOfGroundTile;
    private List<Vector3Int> groundTilepositions;
    private Vector3Int intDestination;
    private Vector3Int intPosition;
    private Tilemap tilemap;

    public void SetPath()
    {
        GetIntDestination();
    }
    private Vector3Int GetIntDestination()
    {
        intDestination.x = Mathf.RoundToInt(FloatDestination.x);
        intDestination.y = Mathf.RoundToInt(FloatDestination.y);
        intDestination.z = Mathf.RoundToInt(FloatDestination.z);
        return intDestination;
    }
    private void GetIntPosition()
    {
        intPosition.x = Mathf.RoundToInt(gameObject.transform.position.x);
        intPosition.y = Mathf.RoundToInt(gameObject.transform.position.y);
        intPosition.z = Mathf.RoundToInt(gameObject.transform.position.z);
    }

    private void CalculatePath()
    {
        if(tilemap.GetTile(GetIntDestination()).name == NameOfGroundTile)
        {

        }
    }
    private void Path()
    {

        
    }

    private void Start()
    {
        for (int i = 0; i <)
        if (tilemap.GetTile(GetIntDestination()).name == NameOfGroundTile)
        {

        }
    }
}

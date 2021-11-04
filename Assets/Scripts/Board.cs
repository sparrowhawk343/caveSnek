using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Board : MonoBehaviour
{
    private Vector2Int gridSize = new Vector2Int();

    private int gridXLength = 20;
    private int gridYLength = 20;
    public Tile[,] tileGrid;
    public Tile tilePrefab;

    private void Awake()
    {
        gridSize.x = gridXLength;
        gridSize.y = gridYLength;
        
    }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        tileGrid = new Tile[gridSize.x, gridSize.y];
        float gridOffset = 0.5f;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 tilePosition = new Vector3(x + gridOffset, y + gridOffset, 0);
                tileGrid[x, y] = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tileGrid[x, y].position = new Vector2Int(x, y);
                tileGrid[x, y].transform.SetParent(gameObject.transform, true);
            }
        }
    }
}

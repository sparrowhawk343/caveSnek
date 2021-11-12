using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Cinemachine;

public class Board : MonoBehaviour
{
    public static Board instance;
    
    private int gridXLength = 20;
    private int gridYLength = 20;

    public HashSet<Vector3> fruitPositions = new HashSet<Vector3>();

    public Vector2Int gridSize = new Vector2Int();
    public Tile[,] tileGrid;
    public Tile tilePrefab;
    public Wall wallPrefab;

    private void Awake()
    {
        gridSize.x = gridXLength;
        gridSize.y = gridYLength;
        
        if (instance == null)
        {
            instance = this;
        }
        else
        { 
            Destroy(gameObject);
        }
        CreateGrid();
        CreateWalls();
    }


    private void CreateGrid()
    {
        tileGrid = new Tile[gridSize.x, gridSize.y];
        // use this if needed to offset from/to unity's displayed units
        float gridOffset = 0f;

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

    private void CreateWalls()
    {
        Wall[,] walls;
        walls = new Wall[gridSize.x, gridSize.y];
        float wallOffset = 0f;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int wallPercentage = Random.Range(0, 100);
                if (wallPercentage <= 7)
                {
                    Vector3 wallPosition = new Vector3(x + wallOffset, y + wallOffset, 0);
                    walls[x, y] = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                    walls[x, y].position = new Vector2Int(x, y);
                    walls[x, y].transform.SetParent(gameObject.transform, true);
                    tileGrid[x, y].hasWall = true;
                }
            }
        }
    }

}

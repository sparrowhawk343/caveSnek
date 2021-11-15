using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Cinemachine;

public class Board : MonoBehaviour
{
    public static Board instance;

    public HashSet<Vector3> fruitPositions = new HashSet<Vector3>();

    public Vector2Int gridSize = new Vector2Int();
    [Range(0, 100)] public int randomFillPercent = 45;
    public string seed;
    public bool useRandomSeed = false;
    public Tile[,] tileGrid;
    public Tile tilePrefab;
    public Wall wallPrefab;

    private int gridXLength = 100;
    private int gridYLength = 100;

    private Wall[,] walls;
    private int[,] booleanGrid;

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

        CreateTiles();
        CreateBooleanGrid();
        GenerateCaves();
        CreateWalls();
    }

    private void CreateBooleanGrid()
    {
        booleanGrid = new int[gridSize.x, gridSize.y];
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                booleanGrid[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    private void CreateTiles()
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
        walls = new Wall[gridSize.x, gridSize.y];
        float wallOffset = 0f;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (booleanGrid[x, y] == 1)
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

    // private void CreateWallsOld()
    // {
    //     walls = new Wall[gridSize.x, gridSize.y];
    //     float wallOffset = 0f;
    //
    //     for (int x = 0; x < gridSize.x; x++)
    //     {
    //         for (int y = 0; y < gridSize.y; y++)
    //         {
    //             int wallPercentage = Random.Range(0, 100);
    //             if (wallPercentage <= 45)
    //             {
    //                 Vector3 wallPosition = new Vector3(x + wallOffset, y + wallOffset, 0);
    //                 walls[x, y] = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
    //                 walls[x, y].position = new Vector2Int(x, y);
    //                 walls[x, y].transform.SetParent(gameObject.transform, true);
    //                 tileGrid[x, y].hasWall = true;
    //             }
    //         }
    //     }
    // }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = tile.position.x + x;
                int checkY = tile.position.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbors.Add(tileGrid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }


    private void GenerateCaves()
    {
        int timesToRun = 1;
        for (int i = 0; i < timesToRun; i++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Tile currentTile = tileGrid[x, y];
                    int wallCounter = 0;
                    List<Tile> neighbors = new List<Tile>();
                    neighbors = GetNeighbors(currentTile);

                    foreach (Tile tile in neighbors)
                    {
                        if (booleanGrid[tile.position.x, tile.position.y] == 1)
                        {
                            wallCounter++;
                        }
                    }

                    if (booleanGrid[currentTile.position.x, currentTile.position.y] == 1 && wallCounter >= 4)
                    {
                        continue;
                    }

                    if (booleanGrid[currentTile.position.x, currentTile.position.y] == 1 && wallCounter < 4)
                    {
                        booleanGrid[currentTile.position.x, currentTile.position.y] = 0;
                    }
                }
            }
        }
    }
}
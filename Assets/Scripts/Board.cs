using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

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
    public int wallThresholdSize = 10;
    public int roomThresholdSize = 10;
    [FormerlySerializedAs("timesToRun")] public int amountOfSmoothingPasses = 1;

    private int gridXLength = 100;
    private int gridYLength = 100;

    private Wall[,] walls;
    private int[,] wallPlacementGrid;

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
        CreateWallPlacementGrid();
        GenerateCave();
        CreateWalls();
    }

    private void CreateWallPlacementGrid()
    {
        wallPlacementGrid = new int[gridSize.x, gridSize.y];
        if (useRandomSeed)
        {
            seed = DateTime.Now.Ticks.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                wallPlacementGrid[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
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
                if (wallPlacementGrid[x, y] == 1)
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

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
    }

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

                int neighborX = tile.position.x + x;
                int neighborY = tile.position.y + y;

                if (IsInMapRange(neighborX, neighborY))
                {
                    neighbors.Add(tileGrid[neighborX, neighborY]);
                }
            }
        }

        return neighbors;
    }


    private void GenerateCave()
    {
        for (int i = 0; i < amountOfSmoothingPasses; i++)
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
                        if (wallPlacementGrid[tile.position.x, tile.position.y] == 1)
                        {
                            wallCounter++;
                        }
                    }

                    if (wallCounter > 4)
                    {
                        continue;
                    }

                    if (wallCounter < 4)
                    {
                        wallPlacementGrid[currentTile.position.x, currentTile.position.y] = 0;
                    }
                }
            }
        }
        ProcessMap();
    }

    List<Tile> GetRegionTiles(int startX, int startY)
    {
        List<Tile> tiles = new List<Tile>();
        int[,] mapFlags = new int[gridSize.x, gridSize.y];
        int tileType = wallPlacementGrid[startX, startY];

        Queue<Tile> queue = new Queue<Tile>();
        Tile tile = tileGrid[startX, startY];
        queue.Enqueue(tile);
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.position.x - 1; x <= tile.position.x + 1; x++)
            {
                for (int y = tile.position.y - 1; y <= tile.position.y + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.position.y || x == tile.position.x))
                    {
                        if (mapFlags[x, y] == 0 && wallPlacementGrid[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(tileGrid[x, y]);
                        }
                    }
                }
            }
        }

        return tiles;
    }

    List<List<Tile>> GetRegions(int tileType)
    {
        List<List<Tile>> regions = new List<List<Tile>>();
        int[,] mapFlags = new int[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (mapFlags[x, y] == 0 && wallPlacementGrid[x, y] == tileType)
                {
                    List<Tile> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Tile tile in newRegion)
                    {
                        mapFlags[tile.position.x, tile.position.y] = 1;
                    }
                }
            }   
        }

        return regions;
    }

    private void ProcessMap()
    {
        List<List<Tile>> wallRegions = GetRegions(1);
        foreach (List<Tile> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Tile tile in wallRegion)
                {
                    wallPlacementGrid[tile.position.x, tile.position.y] = 0;
                }
            }
        }
        
        List<List<Tile>> roomRegions = GetRegions(0);
        foreach (List<Tile> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Tile tile in roomRegion)
                {
                    wallPlacementGrid[tile.position.x, tile.position.y] = 1;
                }
            }
        }
    }
}
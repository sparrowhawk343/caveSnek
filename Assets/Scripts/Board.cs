using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.Analytics;
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
    public int[,] wallPlacementMap;

    private void Awake()
    {
        gridSize.x = gridXLength;
        gridSize.y = gridYLength;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        CreateTiles();
        CreateWallPlacementMap();
        GenerateWallPlacement();
        CreateWalls();
    }

    private void CreateWallPlacementMap()
    {
        wallPlacementMap = new int[gridSize.x, gridSize.y];
        if (useRandomSeed)
        {
            seed = DateTime.Now.Ticks.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                wallPlacementMap[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    private void CreateTiles()
    {
        tileGrid = new Tile[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 tilePosition = new Vector3(x, y, 0);
                tileGrid[x, y] = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tileGrid[x, y].position = new Vector2Int(x, y);
                tileGrid[x, y].transform.SetParent(gameObject.transform, true);
            }
        }
    }

    private void CreateWalls()
    {
        walls = new Wall[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (wallPlacementMap[x, y] == 1)
                {
                    Vector3 wallPosition = new Vector3(x, y, 0);
                    walls[x, y] = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                    walls[x, y].position = new Vector2Int(x, y);
                    walls[x, y].transform.SetParent(gameObject.transform, true);
                }
            }
        }
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
    }

    public List<Tile> GetNeighbors(Tile tile)
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


    private void GenerateWallPlacement()
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
                        if (wallPlacementMap[tile.position.x, tile.position.y] == 1)
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
                        wallPlacementMap[currentTile.position.x, currentTile.position.y] = 0;
                    }
                }
            }
        }

        RemoveTooSmallWallsAndRooms();
    }

    List<Tile> GetRegionTiles(int startX, int startY)
    {
        List<Tile> tiles = new List<Tile>();
        int[,] mapFlags = new int[gridSize.x, gridSize.y];
        int tileType = wallPlacementMap[startX, startY];

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
                        if (mapFlags[x, y] == 0 && wallPlacementMap[x, y] == tileType)
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
                if (mapFlags[x, y] == 0 && wallPlacementMap[x, y] == tileType)
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

    private void RemoveTooSmallWallsAndRooms()
    {
        List<List<Tile>> wallRegions = GetRegions(1);
        foreach (List<Tile> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Tile tile in wallRegion)
                {
                    wallPlacementMap[tile.position.x, tile.position.y] = 0;
                }
            }
        }

        List<List<Tile>> roomRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();
        foreach (List<Tile> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Tile tile in roomRegion)
                {
                    wallPlacementMap[tile.position.x, tile.position.y] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, wallPlacementMap));
            }
        }

        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;
        ConnectClosestRooms(survivingRooms);
    }

    private void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomsNotAccessibleFromMainRoom = new List<Room>();
        List<Room> roomsAccessibleFromMainRoom = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomsAccessibleFromMainRoom.Add(room);
                }
                else
                {
                    roomsNotAccessibleFromMainRoom.Add(room);
                }
            }
        }
        else
        {
            roomsNotAccessibleFromMainRoom = allRooms;
            roomsAccessibleFromMainRoom = allRooms;
        }

        int bestDistance = 0;
        Tile bestTileA = null;
        Tile bestTileB = null;
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomsNotAccessibleFromMainRoom)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomsAccessibleFromMainRoom)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Tile tileA = roomA.edgeTiles[tileIndexA];
                        Tile tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (tileA.position.x - tileB.position.x) *
                                                   (tileA.position.x - tileB.position.x) +
                                                   (tileA.position.y - tileB.position.y) *
                                                   (tileA.position.y - tileB.position.y);
                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }
        
        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Tile tileA, Tile tileB)
    {
        List<Tile> line = GetPassageLine(tileA, tileB);
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(tileA.transform.position, tileB.transform.position, Color.green, 100f);

        foreach (Tile tile in line)
        {
            DrawPassageDiameter(tile, 2);
        }
    }

    private void DrawPassageDiameter(Tile tile, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int drawX = tile.position.x + x;
                    int drawY = tile.position.y + y;

                    if (IsInMapRange(drawX, drawY))
                    {
                        wallPlacementMap[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    private List<Tile> GetPassageLine(Tile from, Tile to)
    {
        List<Tile> line = new List<Tile>();
        int x = from.position.x;
        int y = from.position.y;

        int dx = to.position.x - x;
        int dy = to.position.y - y;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(tileGrid[x, y]);

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;

            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }

                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    public Vector3 GetScreenWrapPosition(Vector3 currentPosition)
    {
        if (currentPosition.x > gridSize.x - 1)
        {
            return new Vector3(0f, currentPosition.y, currentPosition.z);
        }
        if (currentPosition.x < 0f)
        {
            return new Vector3(gridSize.x - 1, currentPosition.y, currentPosition.z);
        }
        if (currentPosition.y > gridSize.y - 1)
        {
            return new Vector3(currentPosition.x, 0f, currentPosition.z);
        }
        if (currentPosition.y < 0f)
        {
            return new Vector3(currentPosition.x, gridSize.y - 1, currentPosition.z);
        }

        return currentPosition;
    }


}
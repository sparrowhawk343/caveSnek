using System;
using System.Collections.Generic;
using UnityEngine;

// linked list pruning
// add comments to room generation methods (outside and inline where needed)

public class Board : MonoBehaviour
{
    public static Board instance;

    public HashSet<Vector3> fruitPositions { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public Vector2Int[,] tileGrid { get; private set; }
    public int[,] wallPlacement { get; private set; }
    
    [SerializeField] 
    [Range(0, 100)] 
    private int randomFillPercent = 45;
    [SerializeField]
    private bool useRandomSeed = false;
    [SerializeField]
    private Wall wallPrefab;
    [SerializeField]
    private int wallThresholdSize = 10;
    [SerializeField]
    private int roomThresholdSize = 10;
    [SerializeField]
    private int amountOfSmoothingPasses = 5;
    [SerializeField]
    private string seed;
    
    private int gridXLength = 100;
    private int gridYLength = 100;

    private void Awake()
    {
        fruitPositions = new HashSet<Vector3>();
        gridSize = new Vector2Int(gridXLength, gridYLength);

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        CreateTiles();
        wallPlacement = CreateWallPlacementMap();
    }

    public bool IsWall(int x, int y)
    {
        return wallPlacement[x, y] == 1;
    }
    
    
    private int[,] CreateWallPlacementMap()
    {
        int[,] wallPlacementMap = new int[gridSize.x, gridSize.y];
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

        wallPlacementMap = GenerateWallPlacement(wallPlacementMap);
        CreateWalls(wallPlacementMap);
        return wallPlacementMap;
    }

    private void CreateTiles()
    {
        tileGrid = new Vector2Int[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                tileGrid[x, y] = new Vector2Int(x, y);
            }
        }
    }

    private void CreateWalls(int[,] wallPlacementMap)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (wallPlacementMap[x, y] == 1)
                {
                    Vector3 wallPosition = new Vector3(x, y, 0);
                    Wall wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                    wall.transform.SetParent(gameObject.transform, true);
                }
            }
        }
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
    }

    public List<Vector2Int> GetNeighbors(Vector2Int tile)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int neighborX = tile.x + x;
                int neighborY = tile.y + y;

                if (IsInMapRange(neighborX, neighborY))
                {
                    neighbors.Add(tileGrid[neighborX, neighborY]);
                }
            }
        }

        return neighbors;
    }


    private int[,] GenerateWallPlacement(int[,] wallPlacementMap)
    {
        for (int i = 0; i < amountOfSmoothingPasses; i++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2Int currentTile = tileGrid[x, y];
                    int wallCounter = 0;
                    List<Vector2Int> neighbors = new List<Vector2Int>();
                    neighbors = GetNeighbors(currentTile);

                    foreach (Vector2Int tile in neighbors)
                    {
                        if (wallPlacementMap[tile.x, tile.y] == 1)
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
                        wallPlacementMap[currentTile.x, currentTile.y] = 0;
                    }
                }
            }
        }

        return RemoveTooSmallWallsAndRooms(wallPlacementMap);
    }

    List<Vector2Int> GetRegionTiles(int[,] wallPlacementMap, int startX, int startY)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        int[,] mapFlags = new int[gridSize.x, gridSize.y];
        int tileType = wallPlacementMap[startX, startY];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Vector2Int tile = tileGrid[startX, startY];
        queue.Enqueue(tile);
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.y || x == tile.x))
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

    List<List<Vector2Int>> GetRegions(int[,] wallPlacementMap, int tileType)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        int[,] mapFlags = new int[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (mapFlags[x, y] == 0 && wallPlacementMap[x, y] == tileType)
                {
                    List<Vector2Int> newRegion = GetRegionTiles(wallPlacementMap, x, y);
                    regions.Add(newRegion);

                    foreach (Vector2Int tile in newRegion)
                    {
                        mapFlags[tile.x, tile.y] = 1;
                    }
                }
            }
        }

        return regions;
    }

    private int[,] RemoveTooSmallWallsAndRooms(int[,] wallPlacementMap)
    {
        List<List<Vector2Int>> wallRegions = GetRegions(wallPlacementMap,1);
        foreach (List<Vector2Int> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Vector2Int tile in wallRegion)
                {
                    wallPlacementMap[tile.x, tile.y] = 0;
                }
            }
        }

        List<List<Vector2Int>> roomRegions = GetRegions(wallPlacementMap, 0);
        List<Room> survivingRooms = new List<Room>();
        foreach (List<Vector2Int> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Vector2Int tile in roomRegion)
                {
                    wallPlacementMap[tile.x, tile.y] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, wallPlacementMap));
            }
        }

        survivingRooms.Sort();
        // fix this guy 
        // survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;
        return ConnectClosestRooms(wallPlacementMap, survivingRooms); 
    }

    private int[,] ConnectClosestRooms(int[,] wallPlacementMap, List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
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
        Vector2Int bestTileA = new Vector2Int();
        Vector2Int bestTileB = new Vector2Int();
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
                        Vector2Int tileA = roomA.edgeTiles[tileIndexA];
                        Vector2Int tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (tileA.x - tileB.x) *
                                                   (tileA.x - tileB.x) +
                                                   (tileA.y - tileB.y) *
                                                   (tileA.y - tileB.y);
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
                CreatePassage(wallPlacementMap, bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(wallPlacementMap, bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(wallPlacementMap, allRooms, true);
        }
        
        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(wallPlacementMap, allRooms, true);
        }

        return wallPlacementMap;
    }

    private void CreatePassage(int[,] wallPlacementMap, Room roomA, Room roomB, Vector2Int tileA, Vector2Int tileB)
    {
        List<Vector2Int> line = GetPassageLine(tileA, tileB);
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(new Vector3(tileA.x, tileA.y, 0f), new Vector3(tileB.x, tileB.y, 0f), Color.green, 100f);

        foreach (Vector2Int tile in line)
        {
            DrawPassageDiameter(wallPlacementMap, tile, 2);
        }
    }

    private void DrawPassageDiameter(int[,] wallPlacementMap, Vector2Int tile, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int drawX = tile.x + x;
                    int drawY = tile.y + y;

                    if (IsInMapRange(drawX, drawY))
                    {
                        wallPlacementMap[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    private List<Vector2Int> GetPassageLine(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> line = new List<Vector2Int>();
        int x = from.x;
        int y = from.y;

        int dx = to.x - x;
        int dy = to.y - y;

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
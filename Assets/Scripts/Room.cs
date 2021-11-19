using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : IComparable<Room>
{
    public List<Vector2Int> edgeTiles { get; }
    public List<Room> connectedRooms { get; }

    internal bool isAccessibleFromMainRoom;

    private List<Vector2Int> tiles;
    private int roomSize;
    private Vector2Int boardSize;

    // empty constructor for when creating an empty room is needed
    public Room()
    {
    }

    public Room(List<Vector2Int> roomTiles, int[,] map)
    {
        tiles = roomTiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();
        edgeTiles = new List<Vector2Int>();

        // go through the tiles in the room and find the edge tiles
        foreach (Vector2Int tile in tiles)
        {
            for (int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if (x == tile.x || y == tile.y)
                    {
                        if (IsOutOfMapRange(x, y, map) == false)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }
    }

    private bool IsOutOfMapRange(int x, int y, int[,] targetMap)
    {
        return x < 0 || x > targetMap.GetLength(0) - 1 || y < 0 || y > targetMap.GetLength(1) - 1;
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA.isAccessibleFromMainRoom)
        {
            roomB.SetAccessibleFromMainRoom();
        }
        else if (roomB.isAccessibleFromMainRoom)
        {
            roomA.SetAccessibleFromMainRoom();
        }

        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }

    public bool IsConnected(Room otherRoom)
    {
        return connectedRooms.Contains(otherRoom);
    }

    public void SetAccessibleFromMainRoom()
    {
        if (!isAccessibleFromMainRoom)
        {
            isAccessibleFromMainRoom = true;
            foreach (Room connectedRoom in connectedRooms)
            {
                connectedRoom.SetAccessibleFromMainRoom();
            }
        }
    }

    public int CompareTo(Room otherRoom)
    {
        return otherRoom.roomSize.CompareTo(roomSize);
    }
}
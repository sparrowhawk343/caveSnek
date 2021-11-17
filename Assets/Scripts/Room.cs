using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : IComparable<Room>
{
    public List<Tile> edgeTiles;
    public List<Room> connectedRooms;

    private List<Tile> tiles;
    private int roomSize;
    private Vector2Int boardSize;

    public bool isAccessibleFromMainRoom;
    public bool isMainRoom;


    public Room()
    {
        // empty constructor for when creating an empty room is needed
    }

    public Room(List<Tile> roomTiles, int[,] map)
    {
        tiles = roomTiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();
        edgeTiles = new List<Tile>();

        foreach (Tile tile in tiles)
        {
            for (int x = tile.position.x - 1; x <= tile.position.x + 1; x++)
            {
                for (int y = tile.position.y - 1; y <= tile.position.y + 1; y++)
                {
                    if (x == tile.position.x || y == tile.position.y)
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
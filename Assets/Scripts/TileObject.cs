using System;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public Board board;

    private void Awake()
    {
        board = Board.instance;
    }

    protected bool IsTileSpawnable(Tile tile)
    {
        List<Tile> neighbors = board.GetNeighbors(tile);
        bool isSpawnable = true;

        foreach (Tile neighbor in neighbors)
        {
            if (board.wallPlacementMap[neighbor.position.x, neighbor.position.y] == 1)
            {
                isSpawnable = false;
                return isSpawnable;
            }
        }

        return isSpawnable;
        
    }
}

using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    protected bool IsTileSpawnable(Vector2Int tile)
    {
        List<Vector2Int> neighbors = Board.instance.GetNeighbors(tile);

        foreach (Vector2Int neighbor in neighbors)
        {
            if (Board.instance.IsWall(neighbor.x, neighbor.y))
            {
                return false;
            }
        }

        return true;

    }
}

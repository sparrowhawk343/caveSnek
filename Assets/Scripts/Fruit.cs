using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour, ITileObject
{
    private Board board;
    private Tile tile;

    private void Start()
    {
        board = Board.instance;
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        tile = board.tileGrid[(int) transform.position.x, (int) transform.position.y];
        
        tile.objects.Remove(this);
        board.fruitPositions.Remove(transform.position);
        
        int x = Random.Range(0, board.gridSize.x - 1);
        int y = Random.Range(0, board.gridSize.y - 1);

        transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0f);
        tile = board.tileGrid[x, y];
        board.fruitPositions.Add(transform.position);
        tile.objects.Add(this);
    }

}
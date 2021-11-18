using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : TileObject
{
    private void Start()
    {
        RandomizePosition();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RandomizePosition();
        }
    }
    public void RandomizePosition()
    {
        
        int x = Random.Range(0, Board.instance.gridSize.x - 1);
        int y = Random.Range(0, Board.instance.gridSize.y - 1);

        if (IsTileSpawnable(Board.instance.tileGrid[x, y]))
        {
            Board.instance.fruitPositions.Remove(transform.position);
            transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0f);
            Board.instance.fruitPositions.Add(transform.position);
        }
        else
        {
            RandomizePosition();
        }
    }
}
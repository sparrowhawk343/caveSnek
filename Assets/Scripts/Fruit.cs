using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    private Board board;

    private void Start()
    {
        board = Board.instance;
        RandomizePosition();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            RandomizePosition();
        }
    }
    public void RandomizePosition()
    {
        board.fruitPositions.Remove(transform.position);
        
        int x = Random.Range(0, board.gridSize.x - 1);
        int y = Random.Range(0, board.gridSize.y - 1);

        transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0f);
        board.fruitPositions.Add(transform.position);
    }
}
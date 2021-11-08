using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    Board board;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        RandomizePosition();
    }

    // TODO: make fruits spawn!

    private void RandomizePosition()
    {
        float x = Random.Range(0f, board.gridSize.x - 1);
        float y = Random.Range(0f, board.gridSize.y - 1);

        transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ADT;

public class Snake : MonoBehaviour
{
    // grid reference
    Board board;

    // data structure fields
    private ADT.LinkedList<SnakeNode> segments;
    private SnakeNode headNode;

    // movement fields
    private float tickTime = 0.5f;
    private Vector2 currentDirection;


    private void Start()
    {
        board = FindObjectOfType<Board>();
        currentDirection = Vector2.right;
        StartCoroutine(MovementTick());
    }

    private void Update()
    {
        Turn();
    }

    private void Move()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x) + currentDirection.x, Mathf.Round(transform.position.y) + currentDirection.y, -1f);

        if (transform.position.x > board.gridSize.x - 1)
        {
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < 0f)
        {
            transform.position = new Vector3(board.gridSize.x - 1, transform.position.y, transform.position.z);
        }
        else if (transform.position.y > board.gridSize.y - 1)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
        else if (transform.position.y < 0f)
        {
            transform.position = new Vector3(transform.position.x, board.gridSize.y - 1, transform.position.z);
        }
    }

    private void Turn()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            currentDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentDirection = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentDirection = Vector2.right;
        }
    }

    private void AddNodeToSnek()
    {
        segments.Add(new SnakeNode());
    }

    private IEnumerator MovementTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime);
            Move();
        }
    }
}

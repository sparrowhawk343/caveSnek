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
    private ADT.LinkedList<SnakeNode> segments = new ADT.LinkedList<SnakeNode>();
    private SnakeNode headNode;
    public SnakeNode nodePrefab;

    // movement fields
    private float tickTime = 0.25f;
    private Vector2 currentDirection;
    private Tile currentTile;
    private Tile previouslySpawnedTile;


    private void Start()
    {
        headNode = GetComponent<SnakeNode>();
        segments.Add(headNode);
        board = FindObjectOfType<Board>();
        SetCurrentTile();
        currentDirection = Vector2.right;
        StartCoroutine(MovementTick());
    }

    private void Update()
    {
        Turn();
    }

    private void SetCurrentTile()
    {
        currentTile = board.tileGrid[(int) transform.position.x, (int) transform.position.y];
    }

    // break out transform calculations and put them in the grid class instead
    // when snake wants to move, it can ask the grid for its next position
    // same for tiles
    private void Move()
    {
        previouslySpawnedTile = board.tileGrid[(int) segments[segments.Count - 1].transform.position.x,
            (int) segments[segments.Count - 1].transform.position.y];

        for (int i = 0; i < segments.Count; i++)
        {
            if (i == 0)
            {
                Vector3 newHeadPosition = new Vector3(Mathf.Round(transform.position.x) + currentDirection.x,
                    Mathf.Round(transform.position.y) + currentDirection.y, 0f);
                segments[i].Move(newHeadPosition);
                continue;
            }
            
            segments[i].Move(segments[i - 1].previousPosition);
        }

        // screen wrap when moving over the side of the grid
        // extract this to its own method and call it when setting head position, return a legal vec3
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

        SetCurrentTile();
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

    private void AddNodeToSnake()
    {
        SnakeNode segment = Instantiate(nodePrefab);
        segment.transform.SetParent(transform);
        segment.Move(previouslySpawnedTile.transform.position);
        segments.Add(segment);
    }

    // break out into tile collision class that the snake can ask for stuff
    // get rid of the type checks and use an enum in the tile object instead
    private void CheckTile(Tile tile)
    {
        if (tile.objects.Count == 0)
        {
            return;
        }

        for (int i = 0; i < tile.objects.Count; i++)
        {
            var type = tile.objects[i].GetType();

            if (type == typeof(Fruit))
            {
                AddNodeToSnake();
                ((Fruit) tile.objects[i]).RandomizePosition();
                i--;
            }
            else if (type == typeof(Wall))
            {
                // do wall stuff here
            }
            else if (type == typeof(SnakeNode))
            {
                // do this in move instead, check if head has same position as another element in the linked list
            }
        }
    }

    // move this to update using a timer instead later
    private IEnumerator MovementTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime);
            Move();
            CheckTile(currentTile);
        }
    }
}
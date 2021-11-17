using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ADT;
using Random = UnityEngine.Random;

public class Snake : TileObject
{
    // scene manager reference
    private SceneManager sceneManager;

    // data structure fields
    private ADT.LinkedList<SnakeNode> segments = new ADT.LinkedList<SnakeNode>();
    private SnakeNode headNode;
    public SnakeNode nodePrefab;

    // movement fields
    private float tickTime = 0.25f;
    private Vector2 currentDirection;
    private Tile currentTile;
    private Tile previouslySpawnedTile;

    // make starting position of snek be random tile without a wall

    private void Start()
    {
        sceneManager = FindObjectOfType<SceneManager>();
        headNode = GetComponent<SnakeNode>();
        segments.Add(headNode);
        SetStartingPosition();
        // currentDirection = Vector2.right;
        StartCoroutine(MovementTick());
    }

    private void Update()
    {
        Turn();
    }

    private void SetStartingPosition()
    {
        int x = Random.Range(0, board.gridSize.x - 1);
        int y = Random.Range(0, board.gridSize.y - 1);

        if (IsTileSpawnable(board.tileGrid[x, y]))
        {
            transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0f);
        }
        else
        {
            SetStartingPosition();
        }
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
        if (previouslySpawnedTile != null)
        {
            SnakeNode segment = Instantiate(nodePrefab, previouslySpawnedTile.transform);
            segments.Add(segment);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fruit"))
        {
            AddNodeToSnake();
        }

        if (other.CompareTag("Wall") || other.CompareTag("Snake"))
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("u ded bruh");
        sceneManager.LoadScene(1);
    }

    // move this to update using a timer instead later
    private IEnumerator MovementTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime);
            Move();
        }
    }
}
using UnityEngine;
using Random = UnityEngine.Random;

public class Snake : TileObject
{
    // data structure fields
    private ADT.LinkedList<SnakeNode> segments = new ADT.LinkedList<SnakeNode>();
    private SnakeNode headNode;
    public SnakeNode nodePrefab;

    // movement fields
    private float tickTime = 0.25f;
    private float timer;
    private Vector2 currentDirection;

    public bool isDead;

    private void Start()
    {
        isDead = false;
        headNode = GetComponent<SnakeNode>();
        segments.Add(headNode);
        SetStartingPosition();
    }

    private void Update()
    {
        Tick();
        SetDirection();
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
    
    private void Move()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newHeadPosition = new Vector3(Mathf.RoundToInt(currentPosition.x) + currentDirection.x,
            Mathf.RoundToInt(currentPosition.y) + currentDirection.y, 0f);
        newHeadPosition = Board.instance.GetScreenWrapPosition(newHeadPosition);
        
        segments.GetFirst().Move(newHeadPosition);
        
        // this for loop starts at the first node that is not the head
        for (int i = 1; i < segments.Count; i++)
        {
            segments[i].Move(segments[i - 1].previousPosition);
        }
    }

    private void SetDirection()
    {
        if (Input.GetKeyDown(KeyCode.W) && currentDirection != Vector2.down)
        {
            currentDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentDirection != Vector2.up)
        {
            currentDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentDirection != Vector2.right)
        {
            currentDirection = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentDirection != Vector2.left)
        {
            currentDirection = Vector2.right;
        }
    }

    private void AddNodeToSnake()
    {
        Vector3 tailNodePreviousPosition = segments.GetLast().previousPosition;
        Tile tileBehindTail = board.tileGrid[Mathf.RoundToInt(tailNodePreviousPosition.x),
            Mathf.RoundToInt(tailNodePreviousPosition.y)];
        SnakeNode segment = Instantiate(nodePrefab, tileBehindTail.transform);
        segments.Add(segment);
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
        isDead = true;
    }


    private void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= tickTime)
        {
            Move();
            timer = 0f;
        }
    }
}
using UnityEngine;
using Random = UnityEngine.Random;

public class Snake : TileObject
{
    public bool isDead { get; private set; }
    
    [SerializeField]
    private SnakeNode nodePrefab;
    
    // data structure fields
    private ADT.LinkedList<SnakeNode> segments = new ADT.LinkedList<SnakeNode>();
    private SnakeNode headNode;

    // movement fields
    private float tickTime = 0.25f;
    private float timer;
    private Vector2Int currentDirection;
    private Vector2Int gridSpaceHeadCoordinate;
    
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
        Vector2Int gridSize = Board.instance.gridSize;
        gridSpaceHeadCoordinate.x = Random.Range(0, gridSize.x - 1);
        gridSpaceHeadCoordinate.y = Random.Range(0, gridSize.y - 1);

        if (IsTileSpawnable(Board.instance.tileGrid[gridSpaceHeadCoordinate.x, gridSpaceHeadCoordinate.y]))
        {
            transform.position = Board.instance.GridToWorldPosition(gridSpaceHeadCoordinate);
        }
        else
        {
            SetStartingPosition();
        }
    }
    
    private void Move()
    {
        gridSpaceHeadCoordinate += currentDirection;
        gridSpaceHeadCoordinate = Board.instance.GetWrappedGridPosition(gridSpaceHeadCoordinate);
        Vector3 worldPosition = Board.instance.GridToWorldPosition(gridSpaceHeadCoordinate);
        segments.GetFirst().Move(worldPosition);
        
        // this for loop starts at the first node that is not the head
        ADT.LinkedList<SnakeNode>.Iterator iterator = segments.GetIterator();
        (SnakeNode previous, SnakeNode current) = iterator.GetNext();
        while (current != null)
        {
            current.Move(previous.previousPosition);
            (previous, current) = iterator.GetNext();
        }
    }

    private void SetDirection()
    {
        if (Input.GetKeyDown(KeyCode.W) && currentDirection != Vector2.down)
        {
            currentDirection = Vector2Int.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentDirection != Vector2.up)
        {
            currentDirection = Vector2Int.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentDirection != Vector2.right)
        {
            currentDirection = Vector2Int.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentDirection != Vector2.left)
        {
            currentDirection = Vector2Int.right;
        }
    }

    private void AddNodeToSnake()
    {
        Vector3 tailNodePreviousPosition = segments.GetLast().previousPosition;
        Vector2Int tileBehindTail = Board.instance.tileGrid[Mathf.RoundToInt(tailNodePreviousPosition.x),
            Mathf.RoundToInt(tailNodePreviousPosition.y)];
        SnakeNode segment = Instantiate(nodePrefab, new Vector3(tileBehindTail.x, tileBehindTail.y, 0f), Quaternion.identity);
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